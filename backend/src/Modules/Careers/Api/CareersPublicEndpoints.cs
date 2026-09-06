using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Careers.Infrastructure;
using SummitCms.Modules.Media.Contracts;

namespace SummitCms.Modules.Careers.Api;

public static class CareersPublicEndpoints
{
    public sealed record PublicTrack(string Title, string PathLabel, string Body, string? MediaUrl, string? MediaAlt, string CtaLabel, List<string> Tags);
    public sealed record PublicOpening(string Title, string Department, string Location, string EmploymentType, string TrackType, string Description, string ApplyContact, DateTimeOffset PostedAt);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/public/careers", async (CareersDbContext db, IMediaCatalog media, CancellationToken ct) =>
        {
            var tracks = await db.JobTracks.Include(t => t.Tags).OrderBy(t => t.DisplayOrder).ToListAsync(ct);
            var mediaMap = await media.GetManyAsync(tracks.Where(t => t.MediaId.HasValue).Select(t => t.MediaId!.Value), ct);

            var openings = await db.JobOpenings
                .Where(o => o.IsActive && (o.ClosesAt == null || o.ClosesAt > DateTimeOffset.UtcNow))
                .OrderByDescending(o => o.PostedAt)
                .Select(o => new PublicOpening(o.Title, o.Department, o.Location, o.EmploymentType.ToString(), o.TrackType.ToString(), o.Description, o.ApplyContact, o.PostedAt))
                .ToListAsync(ct);

            return Results.Ok(new
            {
                tracks = tracks.Select(t => new PublicTrack(
                    t.Title, t.PathLabel, t.Body,
                    t.MediaId.HasValue && mediaMap.TryGetValue(t.MediaId.Value, out var m) ? m.Url : null,
                    t.MediaId.HasValue && mediaMap.TryGetValue(t.MediaId.Value, out var m2) ? m2.AltText : null,
                    t.CtaLabel,
                    t.Tags.OrderBy(x => x.DisplayOrder).Select(x => x.Tag).ToList())),
                openings
            });
        }).WithTags("Public.Careers");
    }
}
