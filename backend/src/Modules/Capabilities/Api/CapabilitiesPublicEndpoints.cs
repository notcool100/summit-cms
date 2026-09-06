using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Capabilities.Infrastructure;
using SummitCms.Modules.Media.Contracts;

namespace SummitCms.Modules.Capabilities.Api;

/// <summary>Anonymous read shape for the home-page teaser strip and the capabilities-page panels - same rows, both callers.</summary>
public static class CapabilitiesPublicEndpoints
{
    public sealed record PublicCapability(
        string Key, string Name, string TeaserTag, string Body, string Stat, string StatLabel,
        string Background, bool TextFirst, string? MediaUrl, string? MediaAlt, string FigureLabel, int DisplayOrder);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/public/capabilities", async (CapabilitiesDbContext db, IMediaCatalog media, CancellationToken ct) =>
        {
            var capabilities = await db.Capabilities.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ToListAsync(ct);
            var mediaMap = await media.GetManyAsync(capabilities.Where(c => c.MediaId.HasValue).Select(c => c.MediaId!.Value), ct);

            var result = capabilities.Select(c => new PublicCapability(
                c.Key, c.Name, c.TeaserTag, c.Body, c.Stat, c.StatLabel, c.Background.ToString(), c.TextFirst,
                c.MediaId.HasValue && mediaMap.TryGetValue(c.MediaId.Value, out var m) ? m.Url : null,
                c.MediaId.HasValue && mediaMap.TryGetValue(c.MediaId.Value, out var m2) ? m2.AltText : null,
                c.FigureLabel, c.DisplayOrder));

            return Results.Ok(result);
        }).WithTags("Public.Capabilities");
    }
}
