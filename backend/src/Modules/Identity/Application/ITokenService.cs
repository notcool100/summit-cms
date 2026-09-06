using SummitCms.Modules.Identity.Domain;

namespace SummitCms.Modules.Identity.Application;

public sealed record AccessTokenResult(string Token, DateTimeOffset ExpiresAt);

public interface ITokenService
{
    AccessTokenResult CreateAccessToken(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissionCodes);

    /// <summary>Returns the raw refresh token (given to the client) - only its SHA-256 hash is persisted.</summary>
    string GenerateRefreshToken();

    string HashToken(string rawToken);
}
