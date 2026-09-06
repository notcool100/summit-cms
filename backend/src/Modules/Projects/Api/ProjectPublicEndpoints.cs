using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Media.Contracts;
using SummitCms.Modules.Projects.Domain;
using SummitCms.Modules.Projects.Infrastructure;

namespace SummitCms.Modules.Projects.Api;

/// <summary>Anonymous, read-only shape for the public /projects grid and /projects/{slug} detail page.</summary>
public static class ProjectPublicEndpoints
{
    public sealed record PublicProjectListItem(string Slug, string Name, string IndustryCategory, string Stat, string? HeroUrl, string? HeroAlt, string? Ratio, int? Span, bool IsFeatured);
    public sealed record PublicAdjacent(string Slug, string Name, string? HeroUrl, string? HeroAlt);
    public sealed record PublicGalleryImage(string Url, string AltText, string Role, string Caption);
    public sealed record PublicScopeFact(string Label, string Value);
    public sealed record PublicNarrativeSection(string Idx, string Title, List<string> Paragraphs);
    public sealed record PublicQuote(string Quote, string Attribution);
    public sealed record PublicProjectDetail(
        string Slug, string Name, string IndustryCategory, string Stat, string? HeroUrl, string? HeroAlt,
        List<PublicGalleryImage> GalleryImages, List<PublicScopeFact> ScopeFacts,
        List<PublicNarrativeSection> NarrativeSections, PublicQuote? Quote,
        PublicAdjacent? Previous, PublicAdjacent? Next);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/public/projects").WithTags("Public.Projects");

        group.MapGet("", async (ProjectsDbContext db, IMediaCatalog media, CancellationToken ct) =>
        {
            var projects = await db.Projects.Include(p => p.IndustryCategory).OrderBy(p => p.DisplayOrder).ToListAsync(ct);
            var heroIds = projects.Where(p => p.HeroMediaId.HasValue).Select(p => p.HeroMediaId!.Value);
            var heroMap = await media.GetManyAsync(heroIds, ct);

            return Results.Ok(projects.Select(p => new PublicProjectListItem(
                p.Slug, p.Name, p.IndustryCategory.Name, p.Stat,
                p.HeroMediaId.HasValue && heroMap.TryGetValue(p.HeroMediaId.Value, out var m) ? m.Url : null,
                p.HeroMediaId.HasValue && heroMap.TryGetValue(p.HeroMediaId.Value, out var m2) ? m2.AltText : null,
                p.Ratio, p.Span, p.IsFeatured)));
        });

        group.MapGet("/{slug}", async (string slug, ProjectsDbContext db, IMediaCatalog media, CancellationToken ct) =>
        {
            var all = await db.Projects
                .Include(p => p.IndustryCategory)
                .Include(p => p.GalleryImages)
                .Include(p => p.ScopeFacts)
                .Include(p => p.NarrativeSections).ThenInclude(s => s.Paragraphs)
                .Include(p => p.Quote)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync(ct);

            var index = all.FindIndex(p => p.Slug == slug);
            if (index < 0) return Results.NotFound();

            var project = all[index];
            var previous = index > 0 ? all[index - 1] : null;
            var next = index < all.Count - 1 ? all[index + 1] : null;

            var mediaIds = project.GalleryImages.Select(g => g.MediaId).ToList();
            if (project.HeroMediaId.HasValue) mediaIds.Add(project.HeroMediaId.Value);
            if (previous?.HeroMediaId is { } prevId) mediaIds.Add(prevId);
            if (next?.HeroMediaId is { } nextId) mediaIds.Add(nextId);
            var mediaMap = await media.GetManyAsync(mediaIds, ct);

            string? HeroUrl(Project? p) => p?.HeroMediaId is { } id && mediaMap.TryGetValue(id, out var m) ? m.Url : null;
            string? HeroAlt(Project? p) => p?.HeroMediaId is { } id && mediaMap.TryGetValue(id, out var m) ? m.AltText : null;

            var detail = new PublicProjectDetail(
                project.Slug, project.Name, project.IndustryCategory.Name, project.Stat, HeroUrl(project), HeroAlt(project),
                project.GalleryImages.OrderBy(g => g.DisplayOrder)
                    .Select(g => mediaMap.TryGetValue(g.MediaId, out var m)
                        ? new PublicGalleryImage(m.Url, m.AltText, g.Role.ToString(), g.Caption)
                        : new PublicGalleryImage("", "", g.Role.ToString(), g.Caption))
                    .ToList(),
                project.ScopeFacts.OrderBy(f => f.DisplayOrder).Select(f => new PublicScopeFact(f.Label, f.Value)).ToList(),
                project.NarrativeSections.OrderBy(s => s.DisplayOrder)
                    .Select(s => new PublicNarrativeSection(s.Idx, s.Title, s.Paragraphs.OrderBy(x => x.ParagraphOrder).Select(x => x.Body).ToList()))
                    .ToList(),
                project.Quote is null ? null : new PublicQuote(project.Quote.Quote, project.Quote.Attribution),
                previous is null ? null : new PublicAdjacent(previous.Slug, previous.Name, HeroUrl(previous), HeroAlt(previous)),
                next is null ? null : new PublicAdjacent(next.Slug, next.Name, HeroUrl(next), HeroAlt(next)));

            return Results.Ok(detail);
        });
    }
}
