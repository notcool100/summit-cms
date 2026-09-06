using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Projects.Contracts;
using SummitCms.Modules.Projects.Domain;
using SummitCms.Modules.Projects.Infrastructure;
using SummitCms.Shared.Infrastructure.Auditing;
using SummitCms.Shared.Infrastructure.Security;

namespace SummitCms.Modules.Projects.Api;

public static class ProjectEndpoints
{
    public sealed record ProjectListItem(Guid Id, string Slug, string Name, Guid IndustryCategoryId, string IndustryCategoryName, string Stat, bool IsFeatured, int DisplayOrder, Guid? HeroMediaId, string? Ratio, int? Span);
    public sealed record ProjectWriteDto(string Slug, string Name, Guid IndustryCategoryId, string Stat, bool IsFeatured, int DisplayOrder, Guid? HeroMediaId, string? Ratio, int? Span);

    public sealed record ScopeFactDto(Guid Id, string Label, string Value, int DisplayOrder);
    public sealed record ParagraphDto(Guid Id, int ParagraphOrder, string Body);
    public sealed record NarrativeSectionDto(Guid Id, string Idx, string Title, int DisplayOrder, List<ParagraphDto> Paragraphs);
    public sealed record GalleryImageDto(Guid Id, Guid MediaId, string Role, string Caption, int DisplayOrder);
    public sealed record QuoteDto(string Quote, string Attribution);

    public sealed record ProjectDetail(
        Guid Id, string Slug, string Name, Guid IndustryCategoryId, string IndustryCategoryName, string Stat,
        bool IsFeatured, int DisplayOrder, Guid? HeroMediaId, string? Ratio, int? Span,
        List<GalleryImageDto> GalleryImages, List<ScopeFactDto> ScopeFacts, List<NarrativeSectionDto> NarrativeSections, QuoteDto? Quote);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/projects").WithTags("Admin.Projects").RequirePermission(ProjectsPermissions.Manage);

        group.MapGet("", async (ProjectsDbContext db, CancellationToken ct) =>
            Results.Ok(await db.Projects.Include(p => p.IndustryCategory).OrderBy(p => p.DisplayOrder)
                .Select(p => new ProjectListItem(p.Id, p.Slug, p.Name, p.IndustryCategoryId, p.IndustryCategory.Name, p.Stat, p.IsFeatured, p.DisplayOrder, p.HeroMediaId, p.Ratio, p.Span))
                .ToListAsync(ct)));

        group.MapGet("/{id:guid}", async (Guid id, ProjectsDbContext db, CancellationToken ct) =>
        {
            var p = await db.Projects
                .Include(x => x.IndustryCategory)
                .Include(x => x.GalleryImages)
                .Include(x => x.ScopeFacts)
                .Include(x => x.NarrativeSections).ThenInclude(s => s.Paragraphs)
                .Include(x => x.Quote)
                .FirstOrDefaultAsync(x => x.Id == id, ct);
            return p is null ? Results.NotFound() : Results.Ok(ToDetail(p));
        });

        group.MapPost("", async (ProjectWriteDto body, ProjectsDbContext db, ICurrentUser user, IPublisher publisher, HttpContext http, CancellationToken ct) =>
        {
            var project = FromDto(body);
            db.Projects.Add(project);
            await db.SaveChangesAsync(ct);
            await Audit(publisher, user, http, "Created", project.Id, null, project.Name);
            return Results.Created($"/api/admin/projects/{project.Id}", new { id = project.Id });
        });

        group.MapPut("/{id:guid}", async (Guid id, ProjectWriteDto body, ProjectsDbContext db, ICurrentUser user, IPublisher publisher, HttpContext http, CancellationToken ct) =>
        {
            var project = await db.Projects.FindAsync([id], ct);
            if (project is null) return Results.NotFound();

            project.Slug = body.Slug; project.Name = body.Name; project.IndustryCategoryId = body.IndustryCategoryId;
            project.Stat = body.Stat; project.IsFeatured = body.IsFeatured; project.DisplayOrder = body.DisplayOrder;
            project.HeroMediaId = body.HeroMediaId; project.Ratio = body.Ratio; project.Span = body.Span;
            await db.SaveChangesAsync(ct);
            await Audit(publisher, user, http, "Updated", id, null, project.Name);
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (Guid id, ProjectsDbContext db, ICurrentUser user, IPublisher publisher, HttpContext http, CancellationToken ct) =>
        {
            var project = await db.Projects.FindAsync([id], ct);
            if (project is null) return Results.NotFound();

            db.Projects.Remove(project);
            await db.SaveChangesAsync(ct);
            await Audit(publisher, user, http, "Deleted", id, project.Name, null);
            return Results.NoContent();
        });

        MapChildCollections(group);
    }

    private static void MapChildCollections(RouteGroupBuilder group)
    {
        group.MapPost("/{projectId:guid}/gallery-images", async (Guid projectId, GalleryImageWrite body, ProjectsDbContext db, CancellationToken ct) =>
        {
            var img = new ProjectGalleryImage { ProjectId = projectId, MediaId = body.MediaId, Role = body.Role, Caption = body.Caption, DisplayOrder = body.DisplayOrder };
            db.GalleryImages.Add(img);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/admin/projects/{projectId}/gallery-images/{img.Id}", img.Id);
        });
        group.MapDelete("/gallery-images/{id:guid}", async (Guid id, ProjectsDbContext db, CancellationToken ct) =>
        {
            var e = await db.GalleryImages.FindAsync([id], ct);
            if (e is null) return Results.NotFound();
            db.GalleryImages.Remove(e); await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        group.MapPost("/{projectId:guid}/scope-facts", async (Guid projectId, ScopeFactWrite body, ProjectsDbContext db, CancellationToken ct) =>
        {
            var f = new ProjectScopeFact { ProjectId = projectId, Label = body.Label, Value = body.Value, DisplayOrder = body.DisplayOrder };
            db.ScopeFacts.Add(f);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/admin/projects/{projectId}/scope-facts/{f.Id}", f.Id);
        });
        group.MapDelete("/scope-facts/{id:guid}", async (Guid id, ProjectsDbContext db, CancellationToken ct) =>
        {
            var e = await db.ScopeFacts.FindAsync([id], ct);
            if (e is null) return Results.NotFound();
            db.ScopeFacts.Remove(e); await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        group.MapPost("/{projectId:guid}/narrative-sections", async (Guid projectId, NarrativeSectionWrite body, ProjectsDbContext db, CancellationToken ct) =>
        {
            var section = new ProjectNarrativeSection { ProjectId = projectId, Idx = body.Idx, Title = body.Title, DisplayOrder = body.DisplayOrder };
            db.NarrativeSections.Add(section);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/admin/projects/narrative-sections/{section.Id}", section.Id);
        });
        group.MapDelete("/narrative-sections/{id:guid}", async (Guid id, ProjectsDbContext db, CancellationToken ct) =>
        {
            var e = await db.NarrativeSections.FindAsync([id], ct);
            if (e is null) return Results.NotFound();
            db.NarrativeSections.Remove(e); await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        group.MapPost("/narrative-sections/{sectionId:guid}/paragraphs", async (Guid sectionId, ParagraphWrite body, ProjectsDbContext db, CancellationToken ct) =>
        {
            var p = new ProjectNarrativeParagraph { NarrativeSectionId = sectionId, ParagraphOrder = body.ParagraphOrder, Body = body.Body };
            db.NarrativeParagraphs.Add(p);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/admin/projects/narrative-paragraphs/{p.Id}", p.Id);
        });
        group.MapDelete("/narrative-paragraphs/{id:guid}", async (Guid id, ProjectsDbContext db, CancellationToken ct) =>
        {
            var e = await db.NarrativeParagraphs.FindAsync([id], ct);
            if (e is null) return Results.NotFound();
            db.NarrativeParagraphs.Remove(e); await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        group.MapPut("/{projectId:guid}/quote", async (Guid projectId, QuoteDto body, ProjectsDbContext db, CancellationToken ct) =>
        {
            var quote = await db.Quotes.FirstOrDefaultAsync(q => q.ProjectId == projectId, ct);
            if (quote is null)
            {
                quote = new ProjectQuote { ProjectId = projectId };
                db.Quotes.Add(quote);
            }
            quote.Quote = body.Quote; quote.Attribution = body.Attribution;
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });
    }

    public sealed record GalleryImageWrite(Guid MediaId, ProjectImageRole Role, string Caption, int DisplayOrder);
    public sealed record ScopeFactWrite(string Label, string Value, int DisplayOrder);
    public sealed record NarrativeSectionWrite(string Idx, string Title, int DisplayOrder);
    public sealed record ParagraphWrite(int ParagraphOrder, string Body);

    private static Project FromDto(ProjectWriteDto dto) => new()
    {
        Slug = dto.Slug, Name = dto.Name, IndustryCategoryId = dto.IndustryCategoryId, Stat = dto.Stat,
        IsFeatured = dto.IsFeatured, DisplayOrder = dto.DisplayOrder, HeroMediaId = dto.HeroMediaId, Ratio = dto.Ratio, Span = dto.Span
    };

    private static ProjectDetail ToDetail(Project p) => new(
        p.Id, p.Slug, p.Name, p.IndustryCategoryId, p.IndustryCategory.Name, p.Stat, p.IsFeatured, p.DisplayOrder, p.HeroMediaId, p.Ratio, p.Span,
        p.GalleryImages.OrderBy(g => g.DisplayOrder).Select(g => new GalleryImageDto(g.Id, g.MediaId, g.Role.ToString(), g.Caption, g.DisplayOrder)).ToList(),
        p.ScopeFacts.OrderBy(f => f.DisplayOrder).Select(f => new ScopeFactDto(f.Id, f.Label, f.Value, f.DisplayOrder)).ToList(),
        p.NarrativeSections.OrderBy(s => s.DisplayOrder).Select(s => new NarrativeSectionDto(s.Id, s.Idx, s.Title, s.DisplayOrder,
            s.Paragraphs.OrderBy(x => x.ParagraphOrder).Select(x => new ParagraphDto(x.Id, x.ParagraphOrder, x.Body)).ToList())).ToList(),
        p.Quote is null ? null : new QuoteDto(p.Quote.Quote, p.Quote.Attribution));

    private static Task Audit(IPublisher publisher, ICurrentUser user, HttpContext http, string action, Guid id, string? before, string? after) =>
        publisher.Publish(new EntityAuditEvent(user.UserId, action, nameof(Project), id.ToString(), before, after, http.Connection.RemoteIpAddress?.ToString()));
}
