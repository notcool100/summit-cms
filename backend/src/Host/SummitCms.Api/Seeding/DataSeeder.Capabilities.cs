using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Capabilities.Domain;
using SummitCms.Modules.Capabilities.Infrastructure;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task SeedCapabilitiesAsync(IServiceProvider sp, Dictionary<string, Guid> mediaMap)
    {
        var db = sp.GetRequiredService<CapabilitiesDbContext>();
        if (await db.Capabilities.AnyAsync()) return;

        Guid Media(string url) => mediaMap[url];

        (string Key, string Name, string Tag, string Body, string Stat, string StatLabel, CapabilityBackground Bg, bool TextFirst, string Url, string Fig)[] rows =
        [
            ("mechanical", "Mechanical & Piping", "Process, utility & high-purity systems",
                "Process, utility, and high-purity piping systems from 1/2\" tubing to 96\" headers — carbon, chrome-moly, stainless, and exotic alloys. In-house ASME code stamps, weld engineering, and NDE keep the critical path under one roof.",
                "480K LF", "Pipe installed, single site", CapabilityBackground.Paper, true,
                "https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg", "FIG. 01 — CHROME & HIGH-PURITY"),
            ("steel", "Structural Steel", "Erection at fab and terminal scale",
                "Heavy structural erection for fab support buildings, pipe trestles, and terminal structures. Our ironworker crews sequence with mechanical installation so steel never becomes the bottleneck.",
                "38K T", "Steel erected to date", CapabilityBackground.Panel, false,
                "https://summit.us/wp-content/uploads/2022/04/4-19.002-Splitter-Hot-Oil-1024x768.jpg", "FIG. 02 — TRESTLE ERECTION"),
            ("modular", "Modular Assembly", "Off-site manufacturing (OSM)",
                "Off-site manufacturing yards where trestle modules, pipe racks, and skids are assembled on the ground, under cover, at shop-grade quality — then shipped and set in a fraction of stick-built duration.",
                "212", "Modules set, Project Star", CapabilityBackground.Paper, true,
                "https://summit.us/wp-content/uploads/2023/01/1.-OSM-Yard-61-Modular-Assembly.jpg", "FIG. 03 — OFF-SITE MANUFACTURING"),
            ("underground", "Underground Infrastructure", "Deep utilities & duct banks",
                "Deep utility corridors, duct banks, and gravity systems beneath active construction sites — engineered shoring, dewatering, and survey-grade as-builts that protect everything built above.",
                "31 MI", "Underground utilities placed", CapabilityBackground.Panel, false,
                "https://summit.us/wp-content/uploads/2022/04/IMG_1373-scaled-e1649187636615-875x1024.jpg", "FIG. 04 — DEEP UTILITIES"),
            ("equipment", "Equipment Setting", "Heavy rigging & precision placement",
                "Heavy rigging and millwright-grade placement — turbines, vessels, process tools — set to thousandths with engineered lift plans reviewed by our own PEs.",
                "1,400+", "Engineered lifts executed", CapabilityBackground.Paper, true,
                "https://summit.us/wp-content/uploads/2022/04/IMG_1679-scaled-e1649188273711.jpg", "FIG. 05 — PRECISION RIGGING"),
            ("engineering", "Design-Assist Engineering", "Constructability from day one",
                "Licensed engineers embedded from concept through commissioning. We model constructability, sequence modularization, and price the design as it evolves — so the drawings that reach the field can actually be built.",
                "60+", "Engineers & designers in-house", CapabilityBackground.Panel, false,
                "https://summit.us/wp-content/uploads/2022/05/Intel-Rendering-PNG-edited.png", "FIG. 06 — CONSTRUCTABILITY")
        ];

        for (var i = 0; i < rows.Length; i++)
        {
            var r = rows[i];
            db.Capabilities.Add(new Capability
            {
                Key = r.Key, Name = r.Name, TeaserTag = r.Tag, Body = r.Body, Stat = r.Stat, StatLabel = r.StatLabel,
                Background = r.Bg, TextFirst = r.TextFirst, MediaId = Media(r.Url), FigureLabel = r.Fig, DisplayOrder = i, IsActive = true
            });
        }

        await db.SaveChangesAsync();
    }
}
