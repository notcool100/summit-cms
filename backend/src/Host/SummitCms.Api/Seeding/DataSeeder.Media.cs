using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Media.Domain;
using SummitCms.Modules.Media.Infrastructure;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    /// <summary>
    /// Seeds one External MediaAsset per distinct image URL used by the pre-CMS static site, so
    /// existing content has something real to point at. Returns url -> MediaAsset.Id.
    /// Idempotent: re-running never creates duplicates for a URL already recorded.
    /// </summary>
    /// <summary>Public path of a bundled illustrative stock image.</summary>
    private static string Stock(string slug) => $"/images/stock/{slug}.jpg";

    /// <summary>
    /// Ids of media rows that still point at the unrelated summit.us website. Filled by
    /// <see cref="SeedMediaAsync"/>; every seeder swaps references to these for stock images and
    /// <see cref="RetireThirdPartyMediaAsync"/> deletes the rows once nothing points at them.
    /// </summary>
    private static readonly HashSet<Guid> ThirdPartyMediaIds = [];

    private static bool IsThirdPartyMedia(Guid? id) => id is Guid g && ThirdPartyMediaIds.Contains(g);

    private static async Task<Dictionary<string, Guid>> SeedMediaAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<MediaDbContext>();

        // Illustrative stock photography (Pexels License, see frontend/static/images/stock/CREDITS.txt),
        // served by the frontend as static files. These replace the images previously hotlinked from
        // summit.us, which belongs to an unrelated US company. Swap for owner-approved Summit photos
        // through the admin Media library when they are supplied.
        (string Url, string Alt)[] images =
        [
            (Stock("scaffold-crew-deck"), "Scaffold crew working on an access platform"),
            (Stock("scaffold-tower-crew"), "Crew working across a tall scaffold structure"),
            (Stock("scaffold-access-worker"), "Worker in high visibility clothing beside a scaffold"),
            (Stock("scaffold-design-cad"), "Access design drawing on a computer screen"),
            (Stock("mobile-crane-outriggers"), "Mobile crane set up on outriggers"),
            (Stock("work-at-height-harness"), "Worker in a harness working at height"),
            (Stock("plant-maintenance-conveyor"), "Maintenance worker walking beside a plant conveyor"),
            (Stock("low-loader-transport"), "Equipment loaded on a low loader trailer"),
            (Stock("energy-process-plant"), "Energy processing plant structure and pipework"),
            (Stock("resources-haul-truck"), "Haul truck in an open cut mine"),
            (Stock("utilities-water-treatment"), "Water treatment facility basins"),
            (Stock("renewables-offshore-wind"), "Wind turbines with an installation vessel offshore"),
            (Stock("rail-freight-wagons"), "Freight wagons in a rail yard"),
            (Stock("marine-port-cranes"), "Port container cranes and storage yard"),
            (Stock("structural-access-frames"), "Steel access frames on a structure"),
            (Stock("pick-and-carry-crane"), "Pick and carry crane in an industrial yard"),
            (Stock("shutdown-process-facility"), "Aerial view of an industrial process facility"),
            (Stock("marine-wharf-cranes"), "Wharf cranes beside the water"),
            (Stock("renewables-platform-crew"), "Technicians working on a turbine platform"),
            (Stock("site-planning-engineers"), "Two people in high visibility clothing planning on site"),
            (Stock("road-train-logistics"), "Road train transporting freight")
        ];

        var externalAssets = await db.MediaAssets
            .Where(a => a.SourceType == MediaSourceType.External)
            .ToListAsync();
        ThirdPartyMediaIds.Clear();
        foreach (var asset in externalAssets.Where(a => a.ExternalUrl!.Contains("summit.us/", StringComparison.OrdinalIgnoreCase)))
            ThirdPartyMediaIds.Add(asset.Id);
        var existing = externalAssets
            .Where(a => !ThirdPartyMediaIds.Contains(a.Id))
            .GroupBy(a => a.ExternalUrl!)
            .ToDictionary(g => g.Key, g => g.First().Id);

        foreach (var (url, alt) in images)
        {
            if (existing.TryGetValue(url, out var existingId))
            {
                // Earlier seeds prefixed alt text with "Illustrative image: "; the client asked for that
                // wording to go, so replace the old seeded text. Alt text edited in the admin is kept.
                var current = externalAssets.First(a => a.Id == existingId);
                if (current.AltText.StartsWith("Illustrative image: ", StringComparison.Ordinal)) current.AltText = alt;
                continue;
            }

            var asset = new MediaAsset
            {
                FileName = url.Split('/').Last(),
                ContentType = "external/url",
                AltText = alt,
                SourceType = MediaSourceType.External,
                ExternalUrl = url
            };
            db.MediaAssets.Add(asset);
            existing[url] = asset.Id;
        }
        await db.SaveChangesAsync();

        return existing;
    }
}
