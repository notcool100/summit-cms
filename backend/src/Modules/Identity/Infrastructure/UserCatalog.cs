using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Identity.Contracts;

namespace SummitCms.Modules.Identity.Infrastructure;

public sealed class UserCatalog(IdentityDbContext db) : IUserCatalog
{
    public async Task<UserSummary?> GetAsync(Guid userId, CancellationToken ct)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);
        return user is null ? null : new UserSummary(user.Id, user.Email, user.FullName);
    }

    public Task<bool> ExistsAsync(Guid userId, CancellationToken ct) =>
        db.Users.AsNoTracking().AnyAsync(u => u.Id == userId, ct);
}
