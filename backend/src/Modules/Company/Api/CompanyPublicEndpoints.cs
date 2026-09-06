using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Company.Infrastructure;
using SummitCms.Modules.Media.Contracts;

namespace SummitCms.Modules.Company.Api;

/// <summary>Anonymous, read-only aggregate for the About page - this module only ever backs that one page.</summary>
public static class CompanyPublicEndpoints
{
    public sealed record PublicMilestone(string Year, string Title, string Body);
    public sealed record PublicValue(string Code, string Name, string Body);
    public sealed record PublicTeamMember(string Name, string Title, string? MediaUrl, string? MediaAlt);
    public sealed record PublicLocation(string City, string RoleDescription, bool IsHeadquarters);
    public sealed record PublicAward(string Year, string Name);
    public sealed record PublicNarrative(string Eyebrow, string TitleLine1, string TitleLine2, string Body, string? MediaUrl, string? MediaAlt, string ImageCaption, bool ImageFirst);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/public/about", async (CompanyDbContext db, IMediaCatalog media, CancellationToken ct) =>
        {
            var narrative = await db.NarrativeBlocks.OrderBy(n => n.DisplayOrder).ToListAsync(ct);
            var milestones = await db.Milestones.OrderBy(m => m.DisplayOrder).ToListAsync(ct);
            var values = await db.CompanyValues.OrderBy(v => v.DisplayOrder).ToListAsync(ct);
            var team = await db.TeamMembers.Where(t => t.IsActive).OrderBy(t => t.DisplayOrder).ToListAsync(ct);
            var locations = await db.OfficeLocations.OrderBy(l => l.DisplayOrder).ToListAsync(ct);
            var awards = await db.Awards.OrderBy(a => a.DisplayOrder).ToListAsync(ct);

            var mediaIds = narrative.Where(n => n.MediaId.HasValue).Select(n => n.MediaId!.Value)
                .Concat(team.Where(t => t.MediaId.HasValue).Select(t => t.MediaId!.Value));
            var mediaMap = await media.GetManyAsync(mediaIds, ct);

            string? Url(Guid? id) => id.HasValue && mediaMap.TryGetValue(id.Value, out var m) ? m.Url : null;
            string? Alt(Guid? id) => id.HasValue && mediaMap.TryGetValue(id.Value, out var m) ? m.AltText : null;

            return Results.Ok(new
            {
                narrative = narrative.Select(n => new PublicNarrative(n.Eyebrow, n.TitleLine1, n.TitleLine2, n.Body, Url(n.MediaId), Alt(n.MediaId), n.ImageCaption, n.ImageFirst)),
                milestones = milestones.Select(m => new PublicMilestone(m.Year, m.Title, m.Body)),
                values = values.Select(v => new PublicValue(v.Code, v.Name, v.Body)),
                team = team.Select(t => new PublicTeamMember(t.Name, t.Title, Url(t.MediaId), Alt(t.MediaId))),
                locations = locations.Select(l => new PublicLocation(l.City, l.RoleDescription, l.IsHeadquarters)),
                awards = awards.Select(a => new PublicAward(a.Year, a.Name))
            });
        }).WithTags("Public.Company");
    }
}
