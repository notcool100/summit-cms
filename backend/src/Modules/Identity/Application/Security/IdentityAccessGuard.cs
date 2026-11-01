using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SummitCms.Modules.Identity.Domain;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Shared.Infrastructure.Security;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Modules.Identity.Application.Security;

/// <summary>
/// Privilege rules for user and role administration. Holding "identity.users.manage" lets someone
/// manage accounts, but never beyond their own level:
/// <list type="bullet">
/// <item>Only a SuperAdmin can grant the SuperAdmin role or change a SuperAdmin's account.</item>
/// <item>Anyone else can only grant roles whose permissions they already hold, and can only manage
/// users whose permissions are a subset of their own.</item>
/// <item>Nobody can deactivate, delete or strip roles from themselves, and the last active
/// SuperAdmin can never be deactivated, deleted or demoted - so the system can't be locked out.</item>
/// </list>
/// </summary>
public sealed class IdentityAccessGuard(IdentityDbContext db, ICurrentUser currentUser, IMemoryCache cache)
{
    public const string SuperAdminRole = "SuperAdmin";
    public const string ForbiddenCode = "forbidden";
    public const string ConflictCode = "conflict";

    /// <summary>Cache key for the per-request "is this account still active?" check (see Program.cs).</summary>
    public static string ActiveCacheKey(Guid userId) => $"user-active:{userId}";

    private sealed record Principal(bool IsSuperAdmin, HashSet<string> Permissions);

    private async Task<Principal> LoadAsync(Guid userId, CancellationToken ct)
    {
        var roles = await db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => new { ur.Role.Name, Codes = ur.Role.RolePermissions.Select(rp => rp.Permission.Code).ToList() })
            .ToListAsync(ct);
        return new Principal(
            roles.Any(r => r.Name == SuperAdminRole),
            roles.SelectMany(r => r.Codes).ToHashSet(StringComparer.Ordinal));
    }

    private async Task<Principal> CallerAsync(CancellationToken ct) =>
        currentUser.UserId is Guid id ? await LoadAsync(id, ct) : new Principal(false, []);

    public Guid? CallerId => currentUser.UserId;

    /// <summary>May the caller give these roles to someone?</summary>
    public async Task<Result> CanGrantAsync(IReadOnlyCollection<Role> roles, CancellationToken ct)
    {
        var caller = await CallerAsync(ct);
        if (caller.IsSuperAdmin) return Result.Success();

        if (roles.Any(r => r.Name == SuperAdminRole))
            return Result.Failure("Only a SuperAdmin can grant the SuperAdmin role.", ForbiddenCode);

        var roleIds = roles.Select(r => r.Id).ToList();
        var requested = await db.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync(ct);
        var missing = requested.Where(code => !caller.Permissions.Contains(code)).ToList();
        return missing.Count == 0
            ? Result.Success()
            : Result.Failure("You can't grant a role with permissions you don't have yourself.", ForbiddenCode);
    }

    /// <summary>May the caller put these permission codes on a role?</summary>
    public async Task<Result> CanGrantPermissionsAsync(IReadOnlyCollection<string> codes, CancellationToken ct)
    {
        var caller = await CallerAsync(ct);
        if (caller.IsSuperAdmin || codes.All(caller.Permissions.Contains)) return Result.Success();
        return Result.Failure("You can't give a role permissions you don't have yourself.", ForbiddenCode);
    }

    /// <summary>May the caller change this user's account (password, status, roles, deletion)?</summary>
    public async Task<Result> CanManageUserAsync(Guid targetUserId, CancellationToken ct)
    {
        var caller = await CallerAsync(ct);
        if (caller.IsSuperAdmin) return Result.Success();

        var target = await LoadAsync(targetUserId, ct);
        if (target.IsSuperAdmin)
            return Result.Failure("Only a SuperAdmin can change a SuperAdmin account.", ForbiddenCode);
        if (!target.Permissions.IsSubsetOf(caller.Permissions))
            return Result.Failure("You can't change an account that has more access than you.", ForbiddenCode);
        return Result.Success();
    }

    public Result NotSelf(Guid targetUserId, string action) =>
        currentUser.UserId == targetUserId
            ? Result.Failure($"You can't {action} your own account.", ConflictCode)
            : Result.Success();

    /// <summary>Refuses an operation that would leave no active SuperAdmin.</summary>
    public async Task<Result> KeepsASuperAdminAsync(Guid targetUserId, CancellationToken ct)
    {
        var targetIsActiveSuper = await db.UserRoles.AnyAsync(
            ur => ur.UserId == targetUserId && ur.Role.Name == SuperAdminRole && ur.User.IsActive, ct);
        if (!targetIsActiveSuper) return Result.Success();

        var activeSupers = await db.UserRoles
            .Where(ur => ur.Role.Name == SuperAdminRole && ur.User.IsActive)
            .Select(ur => ur.UserId)
            .Distinct()
            .CountAsync(ct);
        return activeSupers > 1
            ? Result.Success()
            : Result.Failure("This is the last active SuperAdmin. Add another SuperAdmin first.", ConflictCode);
    }

    /// <summary>Revokes every live refresh token so a deactivated/deleted/reset account is signed out.</summary>
    public async Task RevokeSessionsAsync(Guid userId, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var tokens = await db.RefreshTokens.Where(t => t.UserId == userId && t.RevokedAt == null).ToListAsync(ct);
        foreach (var t in tokens) t.RevokedAt = now;
        ForgetActiveStatus(userId);
    }

    /// <summary>Drops the cached active-status so the very next request re-checks the database.</summary>
    public void ForgetActiveStatus(Guid userId) => cache.Remove(ActiveCacheKey(userId));
}
