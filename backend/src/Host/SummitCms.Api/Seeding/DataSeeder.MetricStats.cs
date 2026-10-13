using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.SiteContent.Domain;
using SummitCms.Modules.SiteContent.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task SeedMetricStatsAsync(IServiceProvider sp, Dictionary<string, Guid> pageIds)
    {
        var db = sp.GetRequiredService<SiteContentDbContext>();
        var homeId = pageIds[PageSlugs.Home];
        var staleLabels = new[]
        {
            "Safety (TRIR)", "Craft workforce", "Work hours", "Regions served",
            "TRIR, 3yr avg", "Certified safety pros"
        };
        var staleStats = await db.MetricStats.Where(s => staleLabels.Contains(s.Label)).ToListAsync();
        db.MetricStats.RemoveRange(staleStats);

        (string Label, decimal Value, string Suffix, string Note)[] rows =
        [
            ("Combined industry experience", 50, "+", "Years across the team"),
            ("Core capabilities", 12, "", "Integrated industrial services"),
            ("Industry groupings", 6, "", "Servicing the Greater Pilbara"),
            ("Operating base", 1, "", "Karratha Industrial Estate")
        ];

        for (var i = 0; i < rows.Length; i++)
        {
            var r = rows[i];
            var existing = await db.MetricStats.FirstOrDefaultAsync(s =>
                s.PageId == homeId && s.GroupKey == "home_stats" && s.Label == r.Label);
            if (existing is not null) continue;
            db.MetricStats.Add(new MetricStat
            {
                PageId = homeId, GroupKey = "home_stats", Label = r.Label, Value = r.Value,
                Suffix = r.Suffix, Note = r.Note, DisplayOrder = i
            });
        }

        await db.SaveChangesAsync();
    }
}
