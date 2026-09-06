using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Modules.Identity.Application.Auth;

public sealed record UserSummaryDto(Guid Id, string Email, string FullName, IReadOnlyList<string> Roles);

public sealed record LoginResult(string AccessToken, DateTimeOffset ExpiresAt, string RefreshToken, UserSummaryDto User);

public sealed record LoginCommand(string Email, string Password, string? IpAddress) : IRequest<Result<LoginResult>>;

public sealed class LoginCommandHandler(
    IdentityDbContext db,
    IPasswordHasherService hasher,
    ITokenService tokens,
    TimeProvider timeProvider,
    IOptions<JwtOptions> jwtOptions) : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), ct);

        if (user is null || !user.IsActive || !hasher.Verify(user.PasswordHash, request.Password))
            return Result.Failure<LoginResult>("Invalid email or password.", "invalid_credentials");

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Code))
            .Distinct()
            .ToList();

        var access = tokens.CreateAccessToken(user, roles, permissions);
        var rawRefreshToken = tokens.GenerateRefreshToken();

        db.RefreshTokens.Add(new Domain.RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokens.HashToken(rawRefreshToken),
            CreatedAt = timeProvider.GetUtcNow(),
            ExpiresAt = timeProvider.GetUtcNow().AddDays(jwtOptions.Value.RefreshTokenDays),
            CreatedByIp = request.IpAddress
        });
        user.LastLoginAt = timeProvider.GetUtcNow();
        await db.SaveChangesAsync(ct);

        return Result.Success(new LoginResult(
            access.Token, access.ExpiresAt, rawRefreshToken,
            new UserSummaryDto(user.Id, user.Email, user.FullName, roles)));
    }
}
