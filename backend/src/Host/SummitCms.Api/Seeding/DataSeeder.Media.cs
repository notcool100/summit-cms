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
    private static async Task<Dictionary<string, Guid>> SeedMediaAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<MediaDbContext>();

        (string Url, string Alt)[] images =
        [
            ("https://summit.us/wp-content/uploads/2022/05/IMG_5350-scaled.jpg", "Summit crew on an active industrial construction site"),
            ("https://summit.us/wp-content/uploads/2022/05/IMG_5350-1024x768.jpg", "Hydrotest and QC inspection"),
            ("https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg", "Summit welder at work on a pipe rack"),
            ("https://summit.us/wp-content/uploads/2022/04/4-19.002-Splitter-Hot-Oil-1024x768.jpg", "Structural steel erection with crane"),
            ("https://summit.us/wp-content/uploads/2023/01/1.-OSM-Yard-61-Modular-Assembly.jpg", "Modules assembled in the OSM yard"),
            ("https://summit.us/wp-content/uploads/2022/04/IMG_1373-scaled-e1649187636615-875x1024.jpg", "Deep excavation for underground utilities"),
            ("https://summit.us/wp-content/uploads/2022/04/IMG_1679-scaled-e1649188273711.jpg", "Heavy equipment being set by rigging crew"),
            ("https://summit.us/wp-content/uploads/2022/05/Intel-Rendering-PNG-edited.png", "Engineers reviewing a constructability model"),
            ("https://summit.us/wp-content/uploads/2022/03/21.004-TSMC-3.jpeg", "Underground piping installation at a semiconductor fab yard"),
            ("https://summit.us/wp-content/uploads/2023/01/22.006-Project-Star-CUB-Trestle-Modules.jpeg", "Modular trestle module being lifted into place"),
            ("https://summit.us/wp-content/uploads/2022/03/4.-19.005-P66-Central-Three-Rivers.jpeg", "Gray Oak terminal tank farm"),
            ("https://summit.us/wp-content/uploads/2022/03/0_552547_2015-09-04-07-13-49-051.jpg", "D.G. Hunter power plant turbine hall"),
            ("https://summit.us/wp-content/uploads/2022/05/20140514_161421-edited-scaled.jpg", "Early Summit fabrication yard in 1996"),
            ("https://summit.us/wp-content/uploads/2022/04/IMG_0239-scaled-e1649186987916-873x1024.jpg", "Aerial view of the OSM modular assembly yard"),
            ("https://summit.us/wp-content/uploads/2022/04/220116-Summit-Headshots208003-edited.jpg", "Portrait of W. Jeff Johnson, Chief Executive Officer"),
            ("https://summit.us/wp-content/uploads/2022/04/220116-Summit-Headshots209831-edited.jpg", "Portrait of Josh Johnson, President"),
            ("https://summit.us/wp-content/uploads/2023/04/Taylor-Madden-website-2-1024x941.jpg", "Portrait of Taylor Madden, Chief Financial Officer"),
            ("https://summit.us/wp-content/uploads/2022/04/220116-Summit-Headshots208058-edited.jpg", "Portrait of Jordan Dombart, Chief Operating Officer"),
            ("https://summit.us/wp-content/uploads/2022/04/220116-Summit-Headshots208147-edited.jpg", "Portrait of Robby Luna, Executive Vice President"),
            ("https://summit.us/wp-content/uploads/2022/05/22.005-Project-Hedgehog-Rio-Rancho-03.jpg", "Summit crew working at height during golden hour"),
            ("https://summit.us/wp-content/uploads/2023/01/Eagle-Mod-Assembly-1.jpeg", "Aerial view of semiconductor fab construction"),
            ("https://summit.us/wp-content/uploads/2022/03/MAIN_3-07-18-13-scaled.jpg", "Structural steel at a biomass facility"),
            ("https://summit.us/wp-content/uploads/2022/03/louisiana_helicam_crosstexenergy_lores011114-28-of-32-1-1-edited.jpg", "Interior of a manufacturing plant"),
            ("https://summit.us/wp-content/uploads/2022/04/louisiana_helicam_enlinkmidstream_lores081614-53-e1649186720160.jpg", "Wide shot of the pipe corridor trench with crews at scale")
        ];

        var existing = await db.MediaAssets
            .Where(a => a.SourceType == MediaSourceType.External)
            .ToDictionaryAsync(a => a.ExternalUrl!, a => a.Id);

        foreach (var (url, alt) in images)
        {
            if (existing.ContainsKey(url)) continue;

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
