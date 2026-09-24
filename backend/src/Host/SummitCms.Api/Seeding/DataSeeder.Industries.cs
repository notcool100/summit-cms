using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Industries.Domain;
using SummitCms.Modules.Industries.Infrastructure;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task SeedIndustriesAsync(IServiceProvider sp, Dictionary<string, Guid> mediaMap, Dictionary<string, Guid> projectSlugToId)
    {
        var db = sp.GetRequiredService<IndustriesDbContext>();
        if (await db.Industries.AnyAsync()) return;

        Guid Media(string url) => mediaMap[url];

        (string Idx, string Name, string Tag, string Body, string Url, string Fig, (string Label, string Stat, string ProjectSlug)[] Links)[] rows =
        [
            ("01", "Mining & Minerals Processing", "Crushers, trestles, conveyors, OSM",
                "The most schedule-compressed construction in the Pilbara. We support mine and processing plant expansions with underground utility corridors, conveyor trestles, crushing and screening plant piping, and modular assembly yards that pull months off stick-built durations.",
                "https://summit.us/wp-content/uploads/2023/01/Eagle-Mod-Assembly-1.jpeg", "PROJECT PHOENIX, GRID D",
                [("Project Phoenix: Confidential Processing Plant Piping", "480,000 LF", "phoenix"),
                 ("Project Star Modular Trestles", "212 modules", "project-star-modular-trestles"),
                 ("Newman Plant Expansion", "1.2M hours", "newman-plant-expansion")]),
            ("02", "Power Generation", "Simple & combined cycle, retrofits",
                "Boiler work, BOP mechanical, and turbine-generator setting for new units and life-extension retrofits. Our millwright and boilermaker crews have recommissioned units other contractors declared unbuildable.",
                "https://summit.us/wp-content/uploads/2022/03/0_552547_2015-09-04-07-13-49-051.jpg", "IRONBARK, UNIT 7",
                [("Ironbark Power Station Units 5-11", "7 units", "ironbark-power-station")]),
            ("03", "Energy & Terminals", "Storage, pipelines, marine loading",
                "Tank farms, manifolds, and pipeline interconnects delivered under live-facility permitting. We plan hot-work around vessel loading schedules so throughput never stops while capacity grows.",
                "https://summit.us/wp-content/uploads/2022/03/4.-19.005-P66-Central-Three-Rivers.jpeg", "PILBARA MARINE TERMINAL",
                [("Pilbara Marine Terminal", "900,000 BBL", "pilbara-marine-terminal")]),
            ("04", "Renewables & Biomass", "Biomass, RNG, waste-to-energy",
                "Fuel-handling systems, boiler islands, and BOP mechanical for biomass and renewable-fuel facilities: scopes that demand power-plant discipline at emerging-tech budgets.",
                "https://summit.us/wp-content/uploads/2022/03/MAIN_3-07-18-13-scaled.jpg", "RIDGELINE, 38 MW",
                [("Ridgeline Biomass Facility", "38 MW", "ridgeline-biomass-facility")]),
            ("05", "Manufacturing", "Heavy industrial & advanced mfg",
                "Process utilities, equipment setting, and structural scopes for heavy and advanced manufacturing plants, executed with the same craft bench that builds our processing plants and power stations.",
                "https://summit.us/wp-content/uploads/2022/03/louisiana_helicam_crosstexenergy_lores011114-28-of-32-1-1-edited.jpg", "PROCESS UTILITIES",
                [])
        ];

        for (var i = 0; i < rows.Length; i++)
        {
            var r = rows[i];
            var industry = new Industry { Idx = r.Idx, Name = r.Name, Tag = r.Tag, Body = r.Body, MediaId = Media(r.Url), FigureLabel = r.Fig, DisplayOrder = i, IsActive = true };

            for (var j = 0; j < r.Links.Length; j++)
            {
                var (label, stat, slug) = r.Links[j];
                if (!projectSlugToId.TryGetValue(slug, out var projectId)) continue;

                industry.ProjectLinks.Add(new IndustryProjectLink
                {
                    ProjectId = projectId,
                    CustomLabel = label,
                    CustomStat = stat,
                    DisplayOrder = j
                });
            }

            db.Industries.Add(industry);
        }

        await db.SaveChangesAsync();
    }
}
