using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Identity.Contracts;
using SummitCms.Modules.SiteContent.Contracts;
using SummitCms.Modules.SiteContent.Domain;
using SummitCms.Shared.Infrastructure.Auditing;
using SummitCms.Shared.Infrastructure.Endpoints;
using SummitCms.Shared.Infrastructure.Security;

namespace SummitCms.Modules.SiteContent.Api;

public static class SiteContentEndpoints
{
    public sealed record PageDto(Guid Id, string Slug, string Title, string MetaDescription, string HeroHeading, string HeroSubheading, Guid? HeroMediaId, Guid? SecondaryMediaId, Guid? PublishedVersionId, DateTimeOffset? PublishedAt);
    public sealed record PageUpdateDto(string Title, string MetaDescription, string HeroHeading, string HeroSubheading, Guid? HeroMediaId, Guid? SecondaryMediaId);

    public sealed record PageVersionSummaryDto(Guid Id, int VersionNumber, bool IsPublished, DateTimeOffset CreatedAt, Guid? CreatedByUserId, string? CreatedByName);
    public sealed record PageVersionDetailDto(Guid Id, Guid PageId, int VersionNumber, string Title, string MetaDescription, string HeroHeading, string HeroSubheading, Guid? HeroMediaId, Guid? SecondaryMediaId, bool IsPublished, DateTimeOffset CreatedAt, Guid? CreatedByUserId);

    public sealed record SettingDto(Guid Id, string Key, string Value, string ValueType);
    public sealed record SettingCreateDto(string Key, string Value, string ValueType);
    public sealed record SettingUpdateDto(string Value, string ValueType);

    public sealed record EnquiryTypeDto(Guid Id, string Label, int DisplayOrder, bool IsActive);
    public sealed record EnquiryTypeCreateDto(string Label, int DisplayOrder, bool IsActive);
    public sealed record EnquiryTypeUpdateDto(string Label, int DisplayOrder, bool IsActive);

    public sealed record MetricStatDto(Guid Id, Guid PageId, string GroupKey, string Label, decimal Value, string? Prefix, string? Suffix, string? Note, int DisplayOrder);
    public sealed record MetricStatCreateDto(Guid PageId, string GroupKey, string Label, decimal Value, string? Prefix, string? Suffix, string? Note, int DisplayOrder);
    public sealed record MetricStatUpdateDto(string GroupKey, string Label, decimal Value, string? Prefix, string? Suffix, string? Note, int DisplayOrder);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin").WithTags("Admin.SiteContent");

        // Pages don't support Create/Delete - the seven slugs are fixed - only Update via generic PUT semantics.
        var pages = group.MapGroup("/pages").RequirePermission(SiteContentPermissions.ManagePages);
        pages.MapGet("", async (Infrastructure.SiteContentDbContext db, CancellationToken ct) =>
            Results.Ok(await db.Pages
                .Select(p => new PageDto(p.Id, p.Slug, p.Title, p.MetaDescription, p.HeroHeading, p.HeroSubheading, p.HeroMediaId, p.SecondaryMediaId, p.PublishedVersionId, p.PublishedAt))
                .ToListAsync(ct)));

        // PUT no longer mutates the live Page row directly - it creates a new draft PageVersion.
        // Publishing (making a version live) is a separate explicit step below.
        pages.MapPut("/{id:guid}", async (
            Guid id, PageUpdateDto dto, Infrastructure.SiteContentDbContext db, ICurrentUser user, IPublisher publisher,
            HttpContext http, CancellationToken ct) =>
        {
            var pageExists = await db.Pages.AnyAsync(p => p.Id == id, ct);
            if (!pageExists) return Results.NotFound();

            var nextVersionNumber = await db.PageVersions.Where(v => v.PageId == id)
                .Select(v => v.VersionNumber).DefaultIfEmpty(0).MaxAsync(ct) + 1;

            var version = new PageVersion
            {
                PageId = id,
                VersionNumber = nextVersionNumber,
                Title = dto.Title,
                MetaDescription = dto.MetaDescription,
                HeroHeading = dto.HeroHeading,
                HeroSubheading = dto.HeroSubheading,
                HeroMediaId = dto.HeroMediaId,
                SecondaryMediaId = dto.SecondaryMediaId,
                IsPublished = false,
                CreatedByUserId = user.UserId,
                CreatedAt = DateTimeOffset.UtcNow
            };
            db.PageVersions.Add(version);
            await db.SaveChangesAsync(ct);

            await publisher.Publish(new EntityAuditEvent(
                user.UserId, "Updated", "PageVersion", version.Id.ToString(), null,
                JsonSerializer.Serialize(version), http.Connection.RemoteIpAddress?.ToString()));

            return Results.Ok(new { id = version.Id, versionNumber = version.VersionNumber });
        });

        pages.MapGet("/{id:guid}/versions", async (Guid id, Infrastructure.SiteContentDbContext db, IUserCatalog users, CancellationToken ct) =>
        {
            var pageExists = await db.Pages.AnyAsync(p => p.Id == id, ct);
            if (!pageExists) return Results.NotFound();

            var versions = await db.PageVersions
                .Where(v => v.PageId == id)
                .OrderByDescending(v => v.VersionNumber)
                .ToListAsync(ct);

            var items = new List<PageVersionSummaryDto>(versions.Count);
            foreach (var v in versions)
            {
                string? createdByName = null;
                if (v.CreatedByUserId is Guid userId)
                {
                    var summary = await users.GetAsync(userId, ct);
                    createdByName = summary is null ? null : (string.IsNullOrWhiteSpace(summary.FullName) ? summary.Email : summary.FullName);
                }
                items.Add(new PageVersionSummaryDto(v.Id, v.VersionNumber, v.IsPublished, v.CreatedAt, v.CreatedByUserId, createdByName));
            }

            return Results.Ok(items);
        });

        pages.MapGet("/{id:guid}/versions/{versionId:guid}", async (Guid id, Guid versionId, Infrastructure.SiteContentDbContext db, CancellationToken ct) =>
        {
            var version = await db.PageVersions.FirstOrDefaultAsync(v => v.Id == versionId && v.PageId == id, ct);
            if (version is null) return Results.NotFound();

            return Results.Ok(new PageVersionDetailDto(
                version.Id, version.PageId, version.VersionNumber, version.Title, version.MetaDescription,
                version.HeroHeading, version.HeroSubheading, version.HeroMediaId, version.SecondaryMediaId,
                version.IsPublished, version.CreatedAt, version.CreatedByUserId));
        });

        pages.MapPost("/{id:guid}/versions/{versionId:guid}/publish", async (
            Guid id, Guid versionId, Infrastructure.SiteContentDbContext db, ICurrentUser user, IPublisher publisher,
            HttpContext http, CancellationToken ct) =>
        {
            var page = await db.Pages.FindAsync([id], ct);
            if (page is null) return Results.NotFound();

            var version = await db.PageVersions.FirstOrDefaultAsync(v => v.Id == versionId && v.PageId == id, ct);
            if (version is null) return Results.NotFound();

            var previouslyPublished = await db.PageVersions.Where(v => v.PageId == id && v.IsPublished).ToListAsync(ct);
            foreach (var v in previouslyPublished)
                v.IsPublished = false;
            version.IsPublished = true;

            page.Title = version.Title;
            page.MetaDescription = version.MetaDescription;
            page.HeroHeading = version.HeroHeading;
            page.HeroSubheading = version.HeroSubheading;
            page.HeroMediaId = version.HeroMediaId;
            page.SecondaryMediaId = version.SecondaryMediaId;
            page.PublishedVersionId = version.Id;
            page.PublishedAt = DateTimeOffset.UtcNow;

            await db.SaveChangesAsync(ct);

            var publishedDto = new PageDto(page.Id, page.Slug, page.Title, page.MetaDescription, page.HeroHeading, page.HeroSubheading, page.HeroMediaId, page.SecondaryMediaId, page.PublishedVersionId, page.PublishedAt);

            await publisher.Publish(new EntityAuditEvent(
                user.UserId, "Published", "Page", page.Id.ToString(), null,
                JsonSerializer.Serialize(publishedDto), http.Connection.RemoteIpAddress?.ToString()));

            return Results.Ok(publishedDto);
        });

        group.MapAdminCrud<SiteSetting, SettingCreateDto, SettingUpdateDto, SettingDto>(
            "/settings", SiteContentPermissions.ManageSettings,
            s => new SettingDto(s.Id, s.Key, s.Value, s.ValueType),
            dto => new SiteSetting { Key = dto.Key, Value = dto.Value, ValueType = dto.ValueType },
            (s, dto) => { s.Value = dto.Value; s.ValueType = dto.ValueType; });

        group.MapAdminCrud<EnquiryType, EnquiryTypeCreateDto, EnquiryTypeUpdateDto, EnquiryTypeDto>(
            "/enquiry-types", SiteContentPermissions.ManageEnquiryTypes,
            e => new EnquiryTypeDto(e.Id, e.Label, e.DisplayOrder, e.IsActive),
            dto => new EnquiryType { Label = dto.Label, DisplayOrder = dto.DisplayOrder, IsActive = dto.IsActive },
            (e, dto) => { e.Label = dto.Label; e.DisplayOrder = dto.DisplayOrder; e.IsActive = dto.IsActive; });

        group.MapAdminCrud<MetricStat, MetricStatCreateDto, MetricStatUpdateDto, MetricStatDto>(
            "/metric-stats", SiteContentPermissions.ManageStats,
            m => new MetricStatDto(m.Id, m.PageId, m.GroupKey, m.Label, m.Value, m.Prefix, m.Suffix, m.Note, m.DisplayOrder),
            dto => new MetricStat { PageId = dto.PageId, GroupKey = dto.GroupKey, Label = dto.Label, Value = dto.Value, Prefix = dto.Prefix, Suffix = dto.Suffix, Note = dto.Note, DisplayOrder = dto.DisplayOrder },
            (m, dto) => { m.GroupKey = dto.GroupKey; m.Label = dto.Label; m.Value = dto.Value; m.Prefix = dto.Prefix; m.Suffix = dto.Suffix; m.Note = dto.Note; m.DisplayOrder = dto.DisplayOrder; });
    }
}
