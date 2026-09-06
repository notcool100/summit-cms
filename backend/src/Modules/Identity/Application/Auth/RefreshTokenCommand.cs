using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Modules.Identity.Application.Auth;

public sealed record RefreshTokenCommand(string RefreshToken, string? IpAddress) : IRequest<Result<LoginResult>>;

public sealed class RefreshTokenCommandHandler(
    IdentityDbContext db,
    ITokenService tokens,
    TimeProvider timeProvider,
    IOptions<JwtOptions> jwtOptions) : IRequestHandler<RefreshTokenCommand, Result<LoginResult>>
{
    public async Task<Result<LoginResult>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var hash = tokens.HashToken(request.RefreshToken);
        var existing = await db.RefreshTokens
            .Include(rt => rt.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(rt => rt.TokenHash == hash, ct);

        if (existing is null || !existing.IsActive || !existing.User.IsActive)
            return Result.Failure<LoginResult>("Invalid or expired refresh token.", "invalid_token");

        var now = timeProvider.GetUtcNow();
        var rawNewToken = tokens.GenerateRefreshToken();
        var newHash = tokens.HashToken(rawNewToken);

        existing.RevokedAt = now;
        existing.ReplacedByTokenHash = newHash;

        db.RefreshTokens.Add(new Domain.RefreshToken
        {
            UserId = existing.UserId,
            TokenHash = newHash,
            CreatedAt = now,
            ExpiresAt = now.AddDays(jwtOptions.Value.RefreshTokenDays),
            CreatedByIp = request.IpAddress
        });

        var roles = existing.User.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = existing.User.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Code))
            .Distinct()
            .ToList();
        var access = tokens.CreateAccessToken(existing.User, roles, permissions);

        await db.SaveChangesAsync(ct);

        return Result.Success(new LoginResult(
            access.Token, access.ExpiresAt, rawNewToken,
            new UserSummaryDto(existing.User.Id, existing.User.Email, existing.User.FullName, roles)));
    }
}

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;

public sealed class LogoutCommandHandler(IdentityDbContext db, ITokenService tokens, TimeProvider timeProvider)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        var hash = tokens.HashToken(request.RefreshToken);
        var existing = await db.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == hash, ct);
        if (existing is null) return Result.Success();

        existing.RevokedAt = timeProvider.GetUtcNow();
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
