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
        if (await db.MetricStats.AnyAsync()) return;

        var homeId = pageIds[PageSlugs.Home];
        var aboutId = pageIds[PageSlugs.About];
        var careersId = pageIds[PageSlugs.Careers];

        (Guid PageId, string Group, string Label, decimal Value, string? Prefix, string? Suffix, string? Note)[] rows =
        [
            (homeId, "home_stats", "Safety — TRIR", 0.42m, null, null, "Rolling 3-year average"),
            (homeId, "home_stats", "Craft workforce", 2400, null, "+", "Direct-hire, self-performed"),
            (homeId, "home_stats", "Work hours", 18, null, "M+", "Executed since 1996"),
            (homeId, "home_stats", "States served", 35, null, "+", "Licensed & registered"),

            (careersId, "why_summit", "TRIR", 0.42m, null, null, "The safest big sites in the industry. Stop-work authority is yours from day one."),
            (careersId, "why_summit", "Retention, year over year", 92, null, "%", "We keep crews between projects instead of laying off at every demob."),
            (careersId, "why_summit", "Per diem on travel scopes", 120, "$", "/day", "Paid straight, paid weekly — no games with hours thresholds."),
            (careersId, "why_summit", "Craft promoted to leadership", 400, null, "+", "Foremen, GFs, and supers grown from our own bench — not hired over you."),

            (aboutId, "hse", "TRIR — 3yr avg", 0.42m, null, "", null),
            (aboutId, "hse", "Certified safety pros", 126, null, "", null)
        ];

        for (var i = 0; i < rows.Length; i++)
        {
            var r = rows[i];
            db.MetricStats.Add(new MetricStat
            {
                PageId = r.PageId, GroupKey = r.Group, Label = r.Label, Value = r.Value,
                Prefix = r.Prefix, Suffix = r.Suffix, Note = r.Note, DisplayOrder = i
            });
        }

        await db.SaveChangesAsync();
    }
}
