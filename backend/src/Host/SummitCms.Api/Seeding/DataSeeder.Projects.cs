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

        string[] categoryNames = ["Mining & Minerals Processing", "Power", "Energy & Terminals", "Renewables", "Manufacturing"];
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
            ("phoenix", "Project Phoenix — Confidential Processing Plant Piping", "Mining & Minerals Processing", "480,000 LF pipe", true,
                "https://summit.us/wp-content/uploads/2022/03/21.004-TSMC-3.jpeg", "16/10", 7),
            ("processing-plant-foundation-trestle", "Ore Processing Plant Foundations & Trestle Erection", "Mining & Minerals Processing", "38,000 T steel", false,
                "https://summit.us/wp-content/uploads/2023/01/Eagle-Mod-Assembly-1.jpeg", "4/5", 5),
            ("osm-yard-modular-assembly", "Modular Conveyor & Trestle Assembly — OSM Yard", "Mining & Minerals Processing", "48-acre yard", false,
                "https://summit.us/wp-content/uploads/2023/01/1.-OSM-Yard-61-Modular-Assembly.jpg", "4/5", 4),
            ("project-star-modular-trestles", "Project Star — Modular Conveyor Trestles", "Mining & Minerals Processing", "212 modules set", true,
                "https://summit.us/wp-content/uploads/2023/01/22.006-Project-Star-CUB-Trestle-Modules.jpeg", "16/9", 8),
            ("newman-plant-expansion", "Minerals Processing Plant Expansion — Newman", "Mining & Minerals Processing", "1.2M work hours", false,
                "https://summit.us/wp-content/uploads/2022/05/22.005-Project-Hedgehog-Rio-Rancho-03.jpg", "3/2", 6),
            ("pilbara-marine-terminal", "Pilbara Marine Terminal — Tank Farm & Piping", "Energy & Terminals", "900,000 BBL", true,
                "https://summit.us/wp-content/uploads/2022/03/4.-19.005-P66-Central-Three-Rivers.jpeg", "3/2", 6),
            ("ironbark-power-station", "Ironbark Power Station Units 5–11", "Power", "7 units", true,
                "https://summit.us/wp-content/uploads/2022/03/0_552547_2015-09-04-07-13-49-051.jpg", "4/5", 5),
            ("ridgeline-biomass-facility", "Ridgeline Biomass Facility", "Renewables", "38 MW", false,
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

        (string Slug,
            (string Label, string Value)[] Scope,
            (string Url, ProjectImageRole Role, string Caption)[] Gallery,
            (string Idx, string Title, string P1, string P2)[] Narrative,
            string Quote, string Attribution)[] details =
        [
            ("phoenix",
                [
                    ("Client type", "Confidential minerals processing client"),
                    ("Location", "Pilbara region, WA (confidential)"),
                    ("Scope volume", "480,000 LF pipe · 31 mi UG"),
                    ("Duration", "34 months")
                ],
                [
                    ("https://summit.us/wp-content/uploads/2022/04/louisiana_helicam_enlinkmidstream_lores081614-53-e1649186720160.jpg", ProjectImageRole.Break, "CORRIDOR B — 42 FT BELOW GRADE"),
                    ("https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg", ProjectImageRole.Gallery, "Above-grade rack piping detail"),
                    ("https://summit.us/wp-content/uploads/2022/05/IMG_5350-1024x768.jpg", ProjectImageRole.Gallery, "Hydrotest and QC inspection")
                ],
                [
                    ("01", "Challenge",
                        "Install 31 miles of deep underground utilities and 480,000 linear feet of above-grade process piping — beneath and beside an active ore processing plant expansion, with round-the-clock production continuing on adjacent trains and a shared laydown yard that never stopped moving.",
                        "Every corridor crossed live haul-truck and conveyor routes. Every excavation sat within feet of operating plant foundations. Traditional sequencing would have put underground work directly on the critical path for eleven other contractors."),
                    ("02", "Approach",
                        "We flipped the sequence. Summit engineers modelled the full utility corridor in 4D, pre-fabricated 68% of the AG piping as modules in our OSM yard, and ran underground crews on a counter-flow schedule — always one grid ahead of the plant's own construction front.",
                        "Dedicated survey crews issued as-builts within 24 hours of every backfill, so following trades never waited on documentation. Peak staffing hit 640 Summit craft — all direct hire."),
                    ("03", "Outcome",
                        "Mechanical completion 19 days ahead of the production start-up milestone. Zero utility strikes. Zero rework corridors. 14 consecutive months without a recordable incident at peak site congestion.",
                        "The owner awarded Summit the follow-on expansion scope without bid — the strongest endorsement our industry has.")
                ],
                "\"Summit's crews hit 14 consecutive months without a recordable, on the busiest corridor of the site.\"",
                "Owner's construction director"),

            ("processing-plant-foundation-trestle",
                [
                    ("Client type", "Confidential iron ore producer"),
                    ("Location", "Tom Price, WA"),
                    ("Scope volume", "38,000 T structural steel · 6.4 km conveyor trestle"),
                    ("Duration", "22 months")
                ],
                [
                    ("https://summit.us/wp-content/uploads/2022/04/4-19.002-Splitter-Hot-Oil-1024x768.jpg", ProjectImageRole.Break, "TRESTLE BAY 14 — TOPPING OUT"),
                    ("https://summit.us/wp-content/uploads/2022/04/IMG_1373-scaled-e1649187636615-875x1024.jpg", ProjectImageRole.Gallery, "Foundation excavation ahead of steel"),
                    ("https://summit.us/wp-content/uploads/2022/04/IMG_0239-scaled-e1649186987916-873x1024.jpg", ProjectImageRole.Gallery, "Pre-assembled trestle spans staged in the yard")
                ],
                [
                    ("01", "Challenge",
                        "Erect 6.4 kilometres of conveyor trestle and pour foundations for a new crushing and screening train, on a live minesite where ore haulage could not be interrupted for more than a scheduled shift change.",
                        "Foundation work ran through a wet-season window that shrank the available pour days by nearly a third, while the trestle route crossed two active haul roads that could only close on a rotating basis."),
                    ("02", "Approach",
                        "We staged foundation crews ahead of steel by exactly one bay at all times, so no crane ever waited on concrete cure and no excavation sat open longer than the shift that dug it. Trestle sections were pre-assembled on the ground and lifted as complete spans.",
                        "A dedicated haul-road liaison coordinated every closure window directly with the client's mine control room, turning what could have been a scheduling conflict into a routine daily handoff."),
                    ("03", "Outcome",
                        "Steel topped out six weeks ahead of the revised wet-season schedule, with zero haul-road closures that ran past their approved window.",
                        "The crushing train commissioned on the client's original production date, despite the compressed pour schedule — the metric the client cared about most.")
                ],
                "\"Summit adjusted around our production schedule, not the other way around. That's rare on a brownfield site.\"",
                "Client's project director"),

            ("osm-yard-modular-assembly",
                [
                    ("Client type", "Internal capability — multi-client yard"),
                    ("Location", "Karratha, WA"),
                    ("Scope volume", "48-acre off-site manufacturing yard"),
                    ("Duration", "Ongoing")
                ],
                [
                    ("https://summit.us/wp-content/uploads/2022/04/IMG_0239-scaled-e1649186987916-873x1024.jpg", ProjectImageRole.Break, "OSM YARD — AERIAL, KARRATHA"),
                    ("https://summit.us/wp-content/uploads/2022/04/IMG_1679-scaled-e1649188273711.jpg", ProjectImageRole.Gallery, "Module set by rigging crew"),
                    ("https://summit.us/wp-content/uploads/2022/04/4-19.002-Splitter-Hot-Oil-1024x768.jpg", ProjectImageRole.Gallery, "Structural module under fabrication")
                ],
                [
                    ("01", "Challenge",
                        "Cyclone season shuts down Pilbara fieldwork for weeks at a time most years, and daytime heat alone caps productive field hours for months on either side of it. A stick-built schedule inherits every one of those lost days.",
                        "Clients kept asking for the same thing: shorter time-on-site without lower quality. Field conditions in this region make that a genuine engineering problem, not a sales pitch."),
                    ("02", "Approach",
                        "We built a 48-acre off-site manufacturing yard in Karratha where trestle spans, pipe racks, and conveyor modules are assembled under cover, on the ground, at shop-grade quality — outside the reach of cyclone shutdowns and daytime heat stops.",
                        "Modules are sized against what the route between yard and site can actually move, then trucked and set in a fraction of the time a stick-built equivalent would take in the field."),
                    ("03", "Outcome",
                        "Yard-built trestle and rack modules now account for the majority of structural steel on our multi-client Pilbara scopes, with defect rates well below field-erected comparables.",
                        "Clients get a firmer schedule commitment, because the highest-variance work — weather-exposed field assembly — has been moved somewhere the weather can't touch it.")
                ],
                "\"The yard is the reason Summit can commit to a date through cyclone season and actually hold it.\"",
                "Summit Director of Modular Fabrication"),

            ("project-star-modular-trestles",
                [
                    ("Client type", "Confidential mining client"),
                    ("Location", "Newman, WA"),
                    ("Scope volume", "212 trestle modules · 9.1 km route"),
                    ("Duration", "28 months")
                ],
                [
                    ("https://summit.us/wp-content/uploads/2022/05/Intel-Rendering-PNG-edited.png", ProjectImageRole.Break, "LIFT SEQUENCE MODEL — BAY 40"),
                    ("https://summit.us/wp-content/uploads/2022/04/IMG_1679-scaled-e1649188273711.jpg", ProjectImageRole.Gallery, "Module lift during a booked production window"),
                    ("https://summit.us/wp-content/uploads/2023/01/1.-OSM-Yard-61-Modular-Assembly.jpg", ProjectImageRole.Gallery, "Completed modules staged for transport")
                ],
                [
                    ("01", "Challenge",
                        "Replace 9.1 kilometres of ageing conveyor trestle on a mine expansion without stopping ore movement on the line running directly beneath it.",
                        "Every module set required a scheduled production window measured in hours, not shifts, coordinated against a haul schedule the client could not move."),
                    ("02", "Approach",
                        "212 trestle modules were pre-built complete with walkways, cable trays, and conveyor idlers in our OSM yard, then trucked to site and set during short, pre-booked production windows using a dedicated heavy-lift crew.",
                        "Our engineers modelled every lift sequence in advance and rehearsed the crane pick on a mock-up bay in the yard before the first module ever reached site, so field time was spent setting steel, not solving problems."),
                    ("03", "Outcome",
                        "All 212 modules were set within their booked production windows, with zero unplanned line stoppages across the full replacement program.",
                        "The client has since awarded two further trestle replacement scopes on the strength of this program's schedule discipline.")
                ],
                "\"Every module went up in the window we gave them. Not most windows. Every one.\"",
                "Client's maintenance planning manager"),

            ("newman-plant-expansion",
                [
                    ("Client type", "Confidential iron ore producer"),
                    ("Location", "Newman, WA"),
                    ("Scope volume", "1.2M work hours · full mechanical scope"),
                    ("Duration", "30 months")
                ],
                [
                    ("https://summit.us/wp-content/uploads/2022/03/louisiana_helicam_crosstexenergy_lores011114-28-of-32-1-1-edited.jpg", ProjectImageRole.Break, "EXPANSION TRAIN — INTERIOR"),
                    ("https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg", ProjectImageRole.Gallery, "Pipefitting on the expansion train"),
                    ("https://summit.us/wp-content/uploads/2022/05/IMG_5350-1024x768.jpg", ProjectImageRole.Gallery, "Hydrotest and QC inspection")
                ],
                [
                    ("01", "Challenge",
                        "Deliver the full mechanical scope for a processing plant expansion running in parallel with an operating plant next door, sharing a single site access road and a single laydown yard between construction and operations traffic.",
                        "Peak workforce demand for the expansion coincided with the client's own peak maintenance shutdown period, putting two large workforces on the same site at the same time."),
                    ("02", "Approach",
                        "We negotiated a dedicated construction access corridor and laydown allocation separate from operations traffic, and phased craft mobilization to avoid competing with the client's shutdown workforce for camp beds and site inductions.",
                        "1.2 million work hours were delivered with a stable core crew retained across the full 30-month program, rather than the high-turnover mobilization-and-demobilization cycle typical of remote expansion work."),
                    ("03", "Outcome",
                        "The expansion reached mechanical completion on the client's revised schedule despite the overlapping shutdown, with no lost-time injuries across the full work-hour total.",
                        "Crew retention on this scope ran well above the industry norm for remote Pilbara work, which the client cited directly in awarding Summit the site's ongoing maintenance contract.")
                ],
                "\"We didn't lose a single day to workforce conflict with our own shutdown crew. That took real coordination on Summit's end.\"",
                "Client's expansion project manager"),

            ("pilbara-marine-terminal",
                [
                    ("Client type", "Confidential terminal operator"),
                    ("Location", "Dampier, WA"),
                    ("Scope volume", "900,000 BBL tank capacity · full process piping tie-ins"),
                    ("Duration", "20 months")
                ],
                [
                    ("https://summit.us/wp-content/uploads/2022/04/louisiana_helicam_enlinkmidstream_lores081614-53-e1649186720160.jpg", ProjectImageRole.Break, "TIE-IN CORRIDOR — BERTH 2"),
                    ("https://summit.us/wp-content/uploads/2022/05/IMG_5350-1024x768.jpg", ProjectImageRole.Gallery, "Hydrotest and QC inspection"),
                    ("https://summit.us/wp-content/uploads/2022/04/4-19.002-Splitter-Hot-Oil-1024x768.jpg", ProjectImageRole.Gallery, "Process piping tie-in at the tank farm")
                ],
                [
                    ("01", "Challenge",
                        "Build a new 900,000-barrel tank farm and tie it into a live marine terminal's existing process piping, without interrupting vessel loading schedules the terminal had committed to shipping clients.",
                        "Every hot-work permit had to be sequenced around loading windows, and the tie-in points sat inside the terminal's existing safety exclusion zones."),
                    ("02", "Approach",
                        "Our piping and instrumentation crews worked to a tie-in schedule built jointly with the terminal's operations team, with hot work windows locked in weeks ahead of each vessel booking rather than negotiated day-to-day.",
                        "Tank erection and process piping ran on parallel crews so neither discipline waited on the other, with instrumentation loop checks closed out progressively instead of batched at the end."),
                    ("03", "Outcome",
                        "Mechanical completion was reached ahead of the owner's turnover date, with zero missed vessel loading windows across the full construction period.",
                        "The same tank farm crew mobilised directly onto a follow-on scope for the same client without a competitive rebid.")
                ],
                "\"Not one shipment was delayed for construction. That's the number our clients actually track.\"",
                "Terminal operations manager"),

            ("ironbark-power-station",
                [
                    ("Client type", "Confidential power utility"),
                    ("Location", "Pilbara region, WA"),
                    ("Scope volume", "7 gas turbine units recommissioned"),
                    ("Duration", "26 months")
                ],
                [
                    ("https://summit.us/wp-content/uploads/2022/04/IMG_1679-scaled-e1649188273711.jpg", ProjectImageRole.Break, "UNIT 9 — TURBINE SET"),
                    ("https://summit.us/wp-content/uploads/2022/05/Intel-Rendering-PNG-edited.png", ProjectImageRole.Gallery, "Engineered lift plan under review"),
                    ("https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg", ProjectImageRole.Gallery, "Boilermaker crew on the turbine hall floor")
                ],
                [
                    ("01", "Challenge",
                        "Recommission seven ageing gas turbine units supplying power to multiple mine sites, without dropping generation capacity the client's operations depended on around the clock.",
                        "Other contractors had assessed several of the units as uneconomical to recommission. The client needed a second opinion backed by a workable execution plan, not just a cost estimate."),
                    ("02", "Approach",
                        "Our millwright and boilermaker crews recommissioned units in a staggered sequence, one at a time, so generation capacity never dropped below the client's minimum committed supply to its mine-site customers.",
                        "Engineered lift plans for turbine-generator setting were reviewed by our own in-house PEs before any unit came offline, cutting the outage window on each unit compared to the client's original estimate."),
                    ("03", "Outcome",
                        "All seven units were returned to service, with the average outage window per unit beating the client's original schedule by several weeks.",
                        "The client now maintains the station on a rolling Summit maintenance contract rather than a per-outage bid.")
                ],
                "\"Two other contractors told us units 9 and 10 weren't worth saving. Summit brought them back online.\"",
                "Client's plant manager"),

            ("ridgeline-biomass-facility",
                [
                    ("Client type", "Confidential renewable energy developer"),
                    ("Location", "Pilbara region, WA"),
                    ("Scope volume", "38 MW fuel-handling & boiler island"),
                    ("Duration", "18 months")
                ],
                [
                    ("https://summit.us/wp-content/uploads/2022/03/louisiana_helicam_crosstexenergy_lores011114-28-of-32-1-1-edited.jpg", ProjectImageRole.Break, "BOILER ISLAND — MECHANICAL COMPLETION"),
                    ("https://summit.us/wp-content/uploads/2022/04/IMG_1373-scaled-e1649187636615-875x1024.jpg", ProjectImageRole.Gallery, "Fuel-handling structural foundations"),
                    ("https://summit.us/wp-content/uploads/2022/04/4-19.002-Splitter-Hot-Oil-1024x768.jpg", ProjectImageRole.Gallery, "Boiler island structural steel")
                ],
                [
                    ("01", "Challenge",
                        "Deliver the fuel-handling system and boiler island for a 38 MW biomass facility — a first-of-scale build for this developer, with no existing site precedent to plan against.",
                        "Fuel-handling equipment for biomass runs on tighter tolerances than the developer's team had budgeted for, and the boiler island schedule had almost no float against the plant's committed grid-connection date."),
                    ("02", "Approach",
                        "We applied the same power-plant discipline we use on gas turbine outages to a project scaled and budgeted like an emerging-technology build, without cutting the engineering rigor to match the smaller price tag.",
                        "Boiler island mechanical and fuel-handling structural work ran on parallel crews, with weekly schedule reviews against the fixed grid-connection date rather than the more relaxed cadence typical of a project this size."),
                    ("03", "Outcome",
                        "The facility reached mechanical completion in time to hit its committed grid-connection date, with the fuel-handling system commissioning without a single design rework.",
                        "The developer has cited this project as the reference case in two subsequent facility proposals to its own investors.")
                ],
                "\"Summit treated a 38-megawatt plant with the same discipline most contractors save for jobs ten times the size.\"",
                "Developer's project sponsor")
        ];

        foreach (var d in details)
            await SeedProjectDetailAsync(db, slugToId[d.Slug], d.Scope, d.Gallery, d.Narrative, d.Quote, d.Attribution, mediaMap);

        return slugToId;
    }

    private static async Task SeedProjectDetailAsync(
        ProjectsDbContext db,
        Guid projectId,
        (string Label, string Value)[] scope,
        (string Url, ProjectImageRole Role, string Caption)[] gallery,
        (string Idx, string Title, string P1, string P2)[] narrative,
        string quote,
        string attribution,
        Dictionary<string, Guid> mediaMap)
    {
        if (await db.ScopeFacts.AnyAsync(f => f.ProjectId == projectId)) return;

        Guid Media(string url) => mediaMap[url];

        for (var i = 0; i < scope.Length; i++)
            db.ScopeFacts.Add(new ProjectScopeFact { ProjectId = projectId, Label = scope[i].Label, Value = scope[i].Value, DisplayOrder = i });

        for (var i = 0; i < gallery.Length; i++)
            db.GalleryImages.Add(new ProjectGalleryImage { ProjectId = projectId, MediaId = Media(gallery[i].Url), Role = gallery[i].Role, Caption = gallery[i].Caption, DisplayOrder = i });

        for (var i = 0; i < narrative.Length; i++)
        {
            var section = new ProjectNarrativeSection { ProjectId = projectId, Idx = narrative[i].Idx, Title = narrative[i].Title, DisplayOrder = i };
            section.Paragraphs.Add(new ProjectNarrativeParagraph { ParagraphOrder = 0, Body = narrative[i].P1 });
            section.Paragraphs.Add(new ProjectNarrativeParagraph { ParagraphOrder = 1, Body = narrative[i].P2 });
            db.NarrativeSections.Add(section);
        }

        db.Quotes.Add(new ProjectQuote { ProjectId = projectId, Quote = quote, Attribution = attribution });

        await db.SaveChangesAsync();
    }
}
