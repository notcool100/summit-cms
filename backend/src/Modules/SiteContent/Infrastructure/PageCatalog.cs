using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.SiteContent.Contracts;

namespace SummitCms.Modules.SiteContent.Infrastructure;

public sealed class PageCatalog(SiteContentDbContext db) : IPageCatalog
{
    public async Task<Guid?> GetIdBySlugAsync(string slug, CancellationToken ct)
    {
        var id = await db.Pages.AsNoTracking().Where(p => p.Slug == slug).Select(p => (Guid?)p.Id).FirstOrDefaultAsync(ct);
        return id;
    }
}
