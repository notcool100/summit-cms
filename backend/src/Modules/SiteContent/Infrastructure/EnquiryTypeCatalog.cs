using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.SiteContent.Contracts;

namespace SummitCms.Modules.SiteContent.Infrastructure;

public sealed class EnquiryTypeCatalog(SiteContentDbContext db) : IEnquiryTypeCatalog
{
    public async Task<EnquiryTypeSummary?> GetAsync(Guid id, CancellationToken ct)
    {
        var e = await db.EnquiryTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : new EnquiryTypeSummary(e.Id, e.Label);
    }

    public Task<List<EnquiryTypeSummary>> ListActiveAsync(CancellationToken ct) =>
        db.EnquiryTypes.AsNoTracking().Where(e => e.IsActive).OrderBy(e => e.DisplayOrder)
            .Select(e => new EnquiryTypeSummary(e.Id, e.Label)).ToListAsync(ct);
}
