using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Careers.Domain;
using SummitCms.Modules.Careers.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task SeedCareersAsync(IServiceProvider sp, Dictionary<string, Guid> pageIds, Dictionary<string, Guid> mediaMap)
    {
        var db = sp.GetRequiredService<CareersDbContext>();
        if (await db.JobTracks.AnyAsync()) return;

        var careersPageId = pageIds[PageSlugs.Careers];
        Guid Media(string url) => mediaMap[url];

        var craft = new JobTrack
        {
            PageId = careersPageId, DisplayOrder = 0, Title = "Craft", PathLabel = "Path 01",
            Body = "Pipefitters, welders, boilermakers, ironworkers, millwrights, operators, riggers. Per-diem that's real, overtime that's steady, and NCCER-certified training on our dime.",
            MediaId = Media("https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg"),
            CtaLabel = "Join the crew →"
        };
        string[] craftTags = ["Combo Welder", "Pipefitter", "Ironworker", "Millwright", "Crane Operator"];
        for (var i = 0; i < craftTags.Length; i++)
            craft.Tags.Add(new JobTrackTag { Tag = craftTags[i], DisplayOrder = i });

        var professional = new JobTrack
        {
            PageId = careersPageId, DisplayOrder = 1, Title = "Professional", PathLabel = "Path 02",
            Body = "Engineers, superintendents, schedulers, safety professionals, QC. Run scopes measured in the hundreds of millions — with authority to match the responsibility.",
            MediaId = Media("https://summit.us/wp-content/uploads/2022/05/Intel-Rendering-PNG-edited.png"),
            CtaLabel = "Lead the work →"
        };
        string[] proTags = ["Project Engineer", "Superintendent", "HSE Manager", "Scheduler", "QC Inspector"];
        for (var i = 0; i < proTags.Length; i++)
            professional.Tags.Add(new JobTrackTag { Tag = proTags[i], DisplayOrder = i });

        db.JobTracks.AddRange(craft, professional);
        await db.SaveChangesAsync();
    }
}
