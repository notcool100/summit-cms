using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.SiteContent.Contracts;
using SummitCms.Modules.SiteContent.Domain;
using SummitCms.Shared.Infrastructure.Endpoints;
using SummitCms.Shared.Infrastructure.Security;

namespace SummitCms.Modules.SiteContent.Api;

public static class SiteContentEndpoints
{
    public sealed record PageDto(Guid Id, string Slug, string Title, string MetaDescription, string HeroHeading, string HeroSubheading, Guid? HeroMediaId, Guid? SecondaryMediaId);
    public sealed record PageUpdateDto(string Title, string MetaDescription, string HeroHeading, string HeroSubheading, Guid? HeroMediaId, Guid? SecondaryMediaId);

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
                .Select(p => new PageDto(p.Id, p.Slug, p.Title, p.MetaDescription, p.HeroHeading, p.HeroSubheading, p.HeroMediaId, p.SecondaryMediaId))
                .ToListAsync(ct)));
        pages.MapPut("/{id:guid}", async (Guid id, PageUpdateDto dto, Infrastructure.SiteContentDbContext db, CancellationToken ct) =>
        {
            var page = await db.Pages.FindAsync([id], ct);
            if (page is null) return Results.NotFound();
            page.Title = dto.Title; page.MetaDescription = dto.MetaDescription;
            page.HeroHeading = dto.HeroHeading; page.HeroSubheading = dto.HeroSubheading;
            page.HeroMediaId = dto.HeroMediaId; page.SecondaryMediaId = dto.SecondaryMediaId;
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
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
