using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Company.Domain;
using SummitCms.Modules.Company.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task SeedCompanyAsync(IServiceProvider sp, Dictionary<string, Guid> pageIds, Dictionary<string, Guid> mediaMap)
    {
        var db = sp.GetRequiredService<CompanyDbContext>();
        var aboutPageId = pageIds[PageSlugs.About];

        Guid Media(string url) => mediaMap[url];

        if (!await db.NarrativeBlocks.AnyAsync())
        {
            db.NarrativeBlocks.AddRange(
                new NarrativeBlock
                {
                    PageId = aboutPageId, DisplayOrder = 0, Eyebrow = "The founding bet",
                    TitleLine1 = "Self-perform", TitleLine2 = "or don't bid.",
                    Body = "Our first contract was 4,100 feet of chrome piping nobody else wanted — a schedule everyone said was impossible. We staffed it with welders we knew by name, finished eleven days early, and never looked back. Today more than 90% of our field hours are worked by Summit employees, not subcontractors.",
                    MediaId = Media("https://summit.us/wp-content/uploads/2022/05/20140514_161421-edited-scaled.jpg"),
                    ImageCaption = "HOUSTON YARD — 1996", ImageFirst = true
                },
                new NarrativeBlock
                {
                    PageId = aboutPageId, DisplayOrder = 1, Eyebrow = "Scale, without losing the plot",
                    TitleLine1 = "From one crew", TitleLine2 = "to 2,400 craft.",
                    Body = "Semiconductor changed everything. Fab schedules demand modular thinking — so we built our own off-site manufacturing yards, where trestles and pipe racks are assembled under cover, on the ground, at quality levels field work can't touch. What ships to site arrives ready to set.",
                    MediaId = Media("https://summit.us/wp-content/uploads/2022/04/IMG_0239-scaled-e1649186987916-873x1024.jpg"),
                    ImageCaption = "OSM YARD — PRESENT DAY", ImageFirst = false
                });
        }

        if (!await db.Milestones.AnyAsync())
        {
            (string Year, string Title, string Body)[] milestones =
            [
                ("1996", "Founded in Houston", "Three superintendents, one rented yard, and a chrome-piping contract nobody else would touch."),
                ("2012", "First full power scope", "D.G. Hunter Units 5–11 — our first multi-unit plant, delivered with 100% self-performed mechanical."),
                ("2016", "OSM yard opens", "Off-site manufacturing changes our economics: assembly on the ground, under cover, at shop quality."),
                ("2019", "Semiconductor entry", "First fab support scope. Cleanroom-adjacent discipline meets heavy mechanical."),
                ("2022", "18M safe work hours", "TRIR at 0.42 across the rolling period — while headcount tripled."),
                ("2025", "Phoenix mega-site", "480,000 LF of pipe underground and above grade on the largest fab build in the country.")
            ];
            for (var i = 0; i < milestones.Length; i++)
                db.Milestones.Add(new Milestone { PageId = aboutPageId, DisplayOrder = i, Year = milestones[i].Year, Title = milestones[i].Title, Body = milestones[i].Body });
        }

        if (!await db.CompanyValues.AnyAsync())
        {
            (string Code, string Name, string Body)[] values =
            [
                ("01", "Safety", "Not a program — a condition of employment. Stop-work authority belongs to everyone with boots on our site."),
                ("02", "Quality", "Every weld traceable. Every turnover package complete before we call it done."),
                ("03", "People", "Direct-hire craft, trained in our own programs, kept between projects. Loyalty runs both directions."),
                ("04", "Execution", "Schedules are promises. We plan the work, work the plan, and report the truth.")
            ];
            for (var i = 0; i < values.Length; i++)
                db.CompanyValues.Add(new CompanyValue { PageId = aboutPageId, DisplayOrder = i, Code = values[i].Code, Name = values[i].Name, Body = values[i].Body });
        }

        if (!await db.TeamMembers.AnyAsync())
        {
            (string Name, string Title, string Url)[] leaders =
            [
                ("W. Jeff Johnson", "Chief Executive Officer", "https://summit.us/wp-content/uploads/2022/04/220116-Summit-Headshots208003-edited.jpg"),
                ("Josh Johnson", "President", "https://summit.us/wp-content/uploads/2022/04/220116-Summit-Headshots209831-edited.jpg"),
                ("Taylor Madden", "Chief Financial Officer", "https://summit.us/wp-content/uploads/2023/04/Taylor-Madden-website-2-1024x941.jpg"),
                ("Jordan Dombart", "Chief Operating Officer", "https://summit.us/wp-content/uploads/2022/04/220116-Summit-Headshots208058-edited.jpg"),
                ("Robby Luna", "Executive Vice President", "https://summit.us/wp-content/uploads/2022/04/220116-Summit-Headshots208147-edited.jpg")
            ];
            for (var i = 0; i < leaders.Length; i++)
                db.TeamMembers.Add(new TeamMember { PageId = aboutPageId, DisplayOrder = i, Name = leaders[i].Name, Title = leaders[i].Title, MediaId = Media(leaders[i].Url), IsActive = true });
        }

        if (!await db.OfficeLocations.AnyAsync())
        {
            (string City, string Role, bool Hq)[] locations =
            [
                ("Houston, TX", "HQ · Engineering · OSM Yard", true),
                ("La Porte, TX", "Gulf Coast operations", false),
                ("Crosby, TX", "Modular fab & assembly yards", false),
                ("Scottsdale, AZ", "Southwest operations", false),
                ("Clarendon, TX", "Field office", false)
            ];
            for (var i = 0; i < locations.Length; i++)
                db.OfficeLocations.Add(new OfficeLocation { PageId = aboutPageId, DisplayOrder = i, City = locations[i].City, RoleDescription = locations[i].Role, IsHeadquarters = locations[i].Hq });
        }

        if (!await db.Awards.AnyAsync())
        {
            (string Year, string Name)[] awards =
            [
                ("2025", "ABC National Excellence in Safety"),
                ("2024", "AGC Construction Safety Excellence — Finalist"),
                ("2024", "OSHA VPP Star Worksite — Phoenix"),
                ("2023", "ABC STEP Diamond"),
                ("2022", "NCCER Training Excellence"),
                ("2021", "Client Zero-Harm Award — Gray Oak")
            ];
            for (var i = 0; i < awards.Length; i++)
                db.Awards.Add(new Award { PageId = aboutPageId, DisplayOrder = i, Year = awards[i].Year, Name = awards[i].Name });
        }

        await db.SaveChangesAsync();
    }
}
