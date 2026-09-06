using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Industries.Infrastructure;
using SummitCms.Modules.Media.Contracts;
using SummitCms.Modules.Projects.Contracts;

namespace SummitCms.Modules.Industries.Api;

public static class IndustryPublicEndpoints
{
    public sealed record PublicLink(string Name, string Stat, string Href);
    public sealed record PublicIndustry(string Idx, string Name, string Tag, string Body, string? MediaUrl, string? MediaAlt, string FigureLabel, List<PublicLink> Links);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/public/industries", async (
            IndustriesDbContext db, IMediaCatalog media, IProjectCatalog projects, CancellationToken ct) =>
        {
            var industries = await db.Industries
                .Where(i => i.IsActive)
                .Include(i => i.ProjectLinks)
                .OrderBy(i => i.DisplayOrder)
                .ToListAsync(ct);

            var mediaMap = await media.GetManyAsync(industries.Where(i => i.MediaId.HasValue).Select(i => i.MediaId!.Value), ct);
            var projectIds = industries.SelectMany(i => i.ProjectLinks).Select(l => l.ProjectId);
            var projectMap = await projects.GetManyAsync(projectIds, ct);

            var result = industries.Select(i => new PublicIndustry(
                i.Idx, i.Name, i.Tag, i.Body,
                i.MediaId.HasValue && mediaMap.TryGetValue(i.MediaId.Value, out var m) ? m.Url : null,
                i.MediaId.HasValue && mediaMap.TryGetValue(i.MediaId.Value, out var m2) ? m2.AltText : null,
                i.FigureLabel,
                i.ProjectLinks.OrderBy(l => l.DisplayOrder)
                    .Where(l => projectMap.ContainsKey(l.ProjectId))
                    .Select(l =>
                    {
                        var p = projectMap[l.ProjectId];
                        return new PublicLink(l.CustomLabel ?? p.Name, l.CustomStat ?? p.Stat, $"/projects/{p.Slug}");
                    })
                    .ToList()));

            return Results.Ok(result);
        }).WithTags("Public.Industries");
    }
}
