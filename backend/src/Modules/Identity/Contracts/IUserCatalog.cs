namespace SummitCms.Modules.Identity.Contracts;

public sealed record UserSummary(Guid Id, string Email, string FullName);

/// <summary>
/// The only surface other modules (e.g. Contact, for "assigned to") are allowed to depend on —
/// never reference Identity.Domain/Infrastructure directly from another module.
/// </summary>
public interface IUserCatalog
{
    Task<UserSummary?> GetAsync(Guid userId, CancellationToken ct);
    Task<bool> ExistsAsync(Guid userId, CancellationToken ct);
}
