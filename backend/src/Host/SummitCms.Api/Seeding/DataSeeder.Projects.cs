using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Projects.Domain;
using SummitCms.Modules.Projects.Infrastructure;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task<Dictionary<string, Guid>> SeedProjectsAsync(IServiceProvider sp, Dictionary<string, Guid> mediaMap)
    {
        var db = sp.GetRequiredService<ProjectsDbContext>();
        Guid Media(string url) => mediaMap[url];

        string[] categoryNames = ["Semiconductor", "Power", "Energy & Terminals", "Renewables", "Manufacturing"];
        var categoryIds = await db.IndustryCategories.ToDictionaryAsync(c => c.Name, c => c.Id);
        for (var i = 0; i < categoryNames.Length; i++)
        {
            if (categoryIds.ContainsKey(categoryNames[i])) continue;
            var cat = new ProjectIndustryCategory { Name = categoryNames[i], DisplayOrder = i };
            db.IndustryCategories.Add(cat);
            categoryIds[categoryNames[i]] = cat.Id;
        }
        await db.SaveChangesAsync();

        var existingSlugs = await db.Projects.Select(p => p.Slug).ToListAsync();

        (string Slug, string Name, string Category, string Stat, bool Featured, string Url, string? Ratio, int? Span)[] rows =
        [
            ("phoenix", "Phoenix Semiconductor UG Infrastructure & AG Piping", "Semiconductor", "480,000 LF pipe", true,
                "https://summit.us/wp-content/uploads/2022/03/21.004-TSMC-3.jpeg", "16/10", 7),
            ("semiconductor-foundation-trestle", "Semiconductor Foundation & Trestle Erection", "Semiconductor", "38,000 T steel", false,
                "https://summit.us/wp-content/uploads/2023/01/Eagle-Mod-Assembly-1.jpeg", "4/5", 5),
            ("osm-yard-modular-assembly", "Semiconductor Modular Assembly — OSM Yard", "Semiconductor", "48-acre yard", false,
                "https://summit.us/wp-content/uploads/2023/01/1.-OSM-Yard-61-Modular-Assembly.jpg", "4/5", 4),
            ("project-star-modular-trestles", "Project Star Modular Trestles", "Semiconductor", "212 modules set", true,
                "https://summit.us/wp-content/uploads/2023/01/22.006-Project-Star-CUB-Trestle-Modules.jpeg", "16/9", 8),
            ("rio-rancho-expansion", "Semiconductor Expansion — Rio Rancho", "Semiconductor", "1.2M work hours", false,
                "https://summit.us/wp-content/uploads/2022/05/22.005-Project-Hedgehog-Rio-Rancho-03.jpg", "3/2", 6),
            ("gray-oak-central-terminal", "Gray Oak Central Terminal", "Energy & Terminals", "900,000 BBL", true,
                "https://summit.us/wp-content/uploads/2022/03/4.-19.005-P66-Central-Three-Rivers.jpeg", "3/2", 6),
            ("dg-hunter-power-plant", "D.G. Hunter Power Plant Units 5–11", "Power", "7 units", true,
                "https://summit.us/wp-content/uploads/2022/03/0_552547_2015-09-04-07-13-49-051.jpg", "4/5", 5),
            ("pinelands-biomass-facility", "Pinelands Biomass Facility", "Renewables", "38 MW", false,
                "https://summit.us/wp-content/uploads/2022/03/MAIN_3-07-18-13-scaled.jpg", "16/10", 7)
        ];

        var slugToId = new Dictionary<string, Guid>();
        for (var i = 0; i < rows.Length; i++)
        {
            var r = rows[i];
            if (existingSlugs.Contains(r.Slug))
            {
                slugToId[r.Slug] = (await db.Projects.FirstAsync(p => p.Slug == r.Slug)).Id;
                continue;
            }

            var project = new Project
            {
                Slug = r.Slug, Name = r.Name, IndustryCategoryId = categoryIds[r.Category], Stat = r.Stat,
                IsFeatured = r.Featured, DisplayOrder = i, HeroMediaId = Media(r.Url), Ratio = r.Ratio, Span = r.Span
            };
            db.Projects.Add(project);
            slugToId[r.Slug] = project.Id;
        }
        await db.SaveChangesAsync();

        await SeedPhoenixDetailAsync(db, slugToId["phoenix"], mediaMap);

        return slugToId;
    }

    private static async Task SeedPhoenixDetailAsync(ProjectsDbContext db, Guid phoenixId, Dictionary<string, Guid> mediaMap)
    {
        if (await db.ScopeFacts.AnyAsync(f => f.ProjectId == phoenixId)) return;

        Guid Media(string url) => mediaMap[url];

        (string Label, string Value)[] scope =
        [
            ("Client type", "Confidential fab owner"),
            ("Location", "Phoenix, AZ"),
            ("Scope volume", "480,000 LF pipe · 31 mi UG"),
            ("Duration", "34 months")
        ];
        for (var i = 0; i < scope.Length; i++)
            db.ScopeFacts.Add(new ProjectScopeFact { ProjectId = phoenixId, Label = scope[i].Label, Value = scope[i].Value, DisplayOrder = i });

        db.GalleryImages.AddRange(
            new ProjectGalleryImage { ProjectId = phoenixId, MediaId = Media("https://summit.us/wp-content/uploads/2022/04/louisiana_helicam_enlinkmidstream_lores081614-53-e1649186720160.jpg"), Role = ProjectImageRole.Break, Caption = "CORRIDOR B — 42 FT BELOW GRADE", DisplayOrder = 0 },
            new ProjectGalleryImage { ProjectId = phoenixId, MediaId = Media("https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg"), Role = ProjectImageRole.Gallery, Caption = "Above-grade rack piping detail", DisplayOrder = 1 },
            new ProjectGalleryImage { ProjectId = phoenixId, MediaId = Media("https://summit.us/wp-content/uploads/2022/05/IMG_5350-1024x768.jpg"), Role = ProjectImageRole.Gallery, Caption = "Hydrotest and QC inspection", DisplayOrder = 2 });

        (string Idx, string Title, string P1, string P2)[] narrative =
        [
            ("01", "Challenge",
                "Install 31 miles of deep underground utilities and 480,000 linear feet of above-grade process piping — beneath and beside an active mega-fab construction site with 12,000 workers, shared laydown, and a tool-move-in date that could not slip.",
                "Every corridor crossed live crane paths. Every excavation sat within feet of freshly poured foundations. Traditional sequencing would have put underground work directly on the critical path for eleven other contractors."),
            ("02", "Approach",
                "We flipped the sequence. Summit engineers modeled the full utility corridor in 4D, pre-fabricated 68% of the AG piping as modules in our OSM yard, and ran underground crews on a counter-flow schedule — always one grid ahead of vertical construction.",
                "Dedicated survey crews issued as-builts within 24 hours of every backfill, so following trades never waited on documentation. Peak staffing hit 640 Summit craft — all direct hire."),
            ("03", "Outcome",
                "Mechanical completion 19 days ahead of the tool-move-in milestone. Zero utility strikes. Zero rework corridors. 14 consecutive months without a recordable incident at peak site congestion.",
                "The owner awarded Summit the follow-on expansion scope without bid — the strongest endorsement our industry has.")
        ];

        for (var i = 0; i < narrative.Length; i++)
        {
            var section = new ProjectNarrativeSection { ProjectId = phoenixId, Idx = narrative[i].Idx, Title = narrative[i].Title, DisplayOrder = i };
            section.Paragraphs.Add(new ProjectNarrativeParagraph { ParagraphOrder = 0, Body = narrative[i].P1 });
            section.Paragraphs.Add(new ProjectNarrativeParagraph { ParagraphOrder = 1, Body = narrative[i].P2 });
            db.NarrativeSections.Add(section);
        }

        db.Quotes.Add(new ProjectQuote
        {
            ProjectId = phoenixId,
            Quote = "\"Summit's crews hit 14 consecutive months without a recordable — on the busiest corridor of the site.\"",
            Attribution = "Owner's construction director"
        });

        await db.SaveChangesAsync();
    }
}
