using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Blog.Domain;
using SummitCms.Modules.Blog.Infrastructure;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task SeedBlogAsync(IServiceProvider sp, Dictionary<string, Guid> mediaMap)
    {
        var db = sp.GetRequiredService<BlogDbContext>();
        if (await db.Posts.AnyAsync()) return;

        Guid? Media(string url) => mediaMap.TryGetValue(url, out var id) ? id : null;

        (string Slug, string Title, string Excerpt, string Category, string AuthorName, string AuthorRole,
            string CoverUrl, bool Featured, DateTimeOffset PublishedAt, string[] Paragraphs)[] rows =
        [
            ("what-18-million-safe-hours-requires",
                "What 18 Million Safe Work Hours Actually Requires",
                "A 0.42 TRIR on projects this size does not happen from a poster in the breakroom. It happens from a stop-work authority every craft worker actually uses.",
                "Safety", "Jordan Dombart", "Chief Operating Officer",
                "https://summit.us/wp-content/uploads/2022/04/IMG_1679-scaled-e1649188273711.jpg", true,
                new DateTimeOffset(2026, 9, 15, 8, 0, 0, TimeSpan.Zero),
                [
                    "Our current trailing rate is a 0.42 TRIR across more than 18 million work hours. That number gets asked about in almost every pre-qualification call we take, so it is worth explaining what actually produces it, because it is not the number itself that matters. It is the behavior underneath it.",
                    "Every Summit craft worker, regardless of tenure, has stop-work authority and is expected to use it. Not report it up a chain first. Stop it. We track how often that authority gets exercised as a leading indicator, not a lagging one, because a crew that never stops work is either perfect or not looking closely enough, and it is never the former.",
                    "The harder discipline is on the busiest days. Fourteen consecutive months without a recordable incident on the Phoenix site happened while that corridor ran three shifts against a tool-move-in date that could not slip. Schedule pressure is exactly when safety habits either hold or quietly erode. Ours held because the stop-work call carries no penalty, ever, even when it turns out to be unnecessary.",
                    "None of this is unique to Summit as a philosophy. What is harder to copy is doing it with 100% direct-hire craft, where every person on that TRIR calculation reports to a Summit superintendent, not a subcontractor's."
                ]),

            ("why-summit-still-direct-hires",
                "Why Summit Still Direct-Hires Every Craft Discipline",
                "Subcontracted craft labor is faster to staff and easier to scale down. We do it anyway because the alternative costs us the thing clients actually hire us for.",
                "Craft Workforce", "Robby Luna", "Executive Vice President",
                "https://summit.us/wp-content/uploads/2022/04/4-19.002-Splitter-Hot-Oil-1024x768.jpg", true,
                new DateTimeOffset(2026, 8, 28, 8, 0, 0, TimeSpan.Zero),
                [
                    "Most specialty mechanical contractors our size have moved toward a broker model: staff a project management layer, subcontract the pipefitters and welders out to whoever is available in that labor market that quarter. It is a rational way to run a business. We do not run ours that way.",
                    "Every welder, pipefitter, and rigger on a Summit site is a Summit employee, trained on Summit procedures, carrying a Summit safety record. When we quote a schedule, we are quoting our own crews' actual productivity, not an estimate of a subcontractor's crew we have not worked with before. That is the difference between a schedule commitment and a schedule guess.",
                    "It also means the craft workforce that finishes a project is frequently the same craft workforce that started it. On Phoenix, peak staffing hit 640 Summit craft, all direct hire, and turnover on that scope stayed low enough that our own superintendents could name most of the crew by the final month. Try getting that continuity from a brokered workforce that turns over between mobilizations.",
                    "The tradeoff is real. Direct hire means we carry the labor market risk ourselves instead of passing it to a sub. We think that is the correct place for that risk to sit, because we are the ones whose name is on the schedule commitment."
                ]),

            ("inside-the-osm-yard",
                "Inside the OSM Yard: How Modular Assembly Cuts Schedule Risk",
                "68% of the AG piping on Phoenix left our yard as finished modules. Here is why that number, not the pipe itself, is what actually protects a schedule.",
                "Modular Construction", "Marcus Reyes", "Director of Modular Fabrication",
                "https://summit.us/wp-content/uploads/2023/01/1.-OSM-Yard-61-Modular-Assembly.jpg", false,
                new DateTimeOffset(2026, 8, 2, 8, 0, 0, TimeSpan.Zero),
                [
                    "Our 48-acre OSM yard exists for one reason: weather, congestion, and crane access at a live jobsite are all variables we cannot control, and a fabrication yard lets us take as much work as possible off a site where we do not control those variables.",
                    "On Phoenix, that meant pre-fabricating 68% of the above-grade piping as complete modules, tested and stamped in the yard, then trucked in for set and tie-in. A module that fails hydrotest in our yard costs us a day. The same failure discovered after it is racked forty feet above an active fab floor costs the whole corridor.",
                    "Modular work also decouples craft headcount from site congestion. At peak, our underground crews were running a counter-flow schedule, always one grid ahead of vertical construction, while yard crews built the next set of modules in parallel with no crane conflicts to manage. Neither crew was waiting on the other.",
                    "The constraint that matters most is not fabrication capacity. It is logistics: module size has to match what the route between yard and site can actually move, on the days it needs to move. We size every module against that constraint before we size it against anything else."
                ]),

            ("gray-oak-central-terminal-mechanical-completion",
                "Gray Oak Central Terminal Reaches Mechanical Completion",
                "900,000 barrels of terminal capacity, handed over ahead of schedule, with the tank farm crew rolling straight onto the next award.",
                "Company News", "Josh Johnson", "President",
                "https://summit.us/wp-content/uploads/2022/03/4.-19.005-P66-Central-Three-Rivers.jpeg", false,
                new DateTimeOffset(2026, 7, 14, 8, 0, 0, TimeSpan.Zero),
                [
                    "Gray Oak Central Terminal reached mechanical completion this month: 900,000 barrels of tank capacity, full process piping tie-ins, and instrumentation loop checks closed out ahead of the owner's turnover date.",
                    "This is the kind of project that does not make headlines outside the industry, which is exactly the point. Energy terminal work rewards a contractor who shows up, executes the scope as engineered, and does not generate change orders the owner did not ask for. That is what our tank farm crew did here, start to finish.",
                    "The same crew is now mobilizing to a follow-on scope for the same client, a pattern we see often enough that we track it as a real metric internally: how frequently an owner brings us back without a competitive rebid. It is the clearest signal we have that a project actually went the way the client needed it to."
                ]),

            ("semiconductor-buildout-mechanical-contracting",
                "The Semiconductor Buildout Is Rewriting Mechanical Contracting",
                "Fab construction does not run on the sequencing logic the rest of our industry was built around. Contractors who have not adapted to that are already behind.",
                "Industry", "Jeff Johnson", "Chief Executive Officer",
                "https://summit.us/wp-content/uploads/2022/05/Intel-Rendering-PNG-edited.png", false,
                new DateTimeOffset(2026, 6, 20, 8, 0, 0, TimeSpan.Zero),
                [
                    "A decade ago, a mechanical contractor could staff up for a semiconductor fab using roughly the same sequencing playbook as a power plant or a chemical facility: underground first, then structural, then piping, then tie-ins, mostly in that order. That playbook does not survive contact with a modern fab schedule.",
                    "Fab owners are now running underground utilities, structural steel, and above-grade piping on parallel, interleaved schedules because the tool-move-in date is fixed years in advance and does not move for construction sequencing. On Phoenix, that meant running underground crews on a counter-flow schedule against active vertical construction, always staying one grid ahead rather than waiting for a clean handoff.",
                    "Contractors who still bid this work with a traditional linear schedule are underpricing the coordination effort and overpromising the float. We have watched capable contractors lose follow-on work not because their craft quality was worse, but because their schedule model assumed a sequencing discipline the project never actually had.",
                    "This is why we built out 4D scheduling and a dedicated modular fabrication yard specifically for fab work, not because it was a nice capability to have, but because the traditional approach was no longer a viable way to bid this category of project."
                ]),

            ("winning-the-abc-excellence-award",
                "What It Takes to Win a Safety Excellence Award Three Years Running",
                "Award submissions ask for numbers. What the judges are actually evaluating is whether your safety program survives a bad week.",
                "Recognition", "Dana Whitfield", "Director of Environmental Health & Safety",
                "https://summit.us/wp-content/uploads/2022/03/louisiana_helicam_crosstexenergy_lores011114-28-of-32-1-1-edited.jpg", false,
                new DateTimeOffset(2026, 5, 30, 8, 0, 0, TimeSpan.Zero),
                [
                    "Every safety excellence submission asks for the same headline numbers: TRIR, DART rate, total work hours. Ours are strong, and they get us shortlisted. They are not what wins the award.",
                    "What the reviewers actually dig into during a site visit is whether the program holds up under pressure: what happens on a shift with a near miss, whether the incident gets reported the same day or quietly absorbed, whether a superintendent under schedule pressure still pulls a crew off a task that looks wrong. That is much harder to fake than a TRIR number, and it is the part of our program we spend the most time building.",
                    "Winning three years running has less to do with any single initiative and more to do with the fact that our safety leadership has not turned over. The same team that built the stop-work culture on our earliest jobs is the team running it on Gray Oak and Phoenix today. Continuity in leadership is, in our experience, the single strongest predictor of whether a safety program is real or just written down."
                ])
        ];

        for (var i = 0; i < rows.Length; i++)
        {
            var r = rows[i];
            var post = new BlogPost
            {
                Slug = r.Slug,
                Title = r.Title,
                Excerpt = r.Excerpt,
                Body = string.Join("\n\n", r.Paragraphs),
                Category = r.Category,
                AuthorName = r.AuthorName,
                AuthorRole = r.AuthorRole,
                CoverMediaId = Media(r.CoverUrl),
                Status = BlogPostStatus.Published,
                PublishedAt = r.PublishedAt,
                IsFeatured = r.Featured
            };
            db.Posts.Add(post);
        }

        await db.SaveChangesAsync();
    }
}
