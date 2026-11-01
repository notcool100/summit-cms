using MediatR;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Identity.Application.Security;
using SummitCms.Modules.Identity.Domain;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Modules.Identity.Application.Users;

public sealed record CreateUserCommand(
    string Email, string Password, string FirstName, string LastName, List<string> RoleNames) : IRequest<Result<Guid>>;

public sealed class CreateUserCommandHandler(IdentityDbContext db, IPasswordHasherService hasher, IdentityAccessGuard guard)
    : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
        if (email.Length is 0 or > 254 || !email.Contains('@') || email.StartsWith('@') || email.EndsWith('@'))
            return Result.Failure<Guid>("Enter a valid email address.", "validation");
        if (string.IsNullOrWhiteSpace(request.FirstName) || request.FirstName.Length > 100 || (request.LastName?.Length ?? 0) > 100)
            return Result.Failure<Guid>("First name is required (max 100 characters).", "validation");
        if (PasswordPolicy.Validate(request.Password, email) is { } passwordError)
            return Result.Failure<Guid>(passwordError, "validation");

        if (await db.Users.AnyAsync(u => u.Email == email, ct))
            return Result.Failure<Guid>("A user with this email already exists.", "duplicate_email");

        var roleNames = (request.RoleNames ?? []).Distinct().ToList();
        var roles = await db.Roles.Where(r => roleNames.Contains(r.Name)).ToListAsync(ct);
        if (roles.Count != roleNames.Count)
            return Result.Failure<Guid>("One or more roles do not exist.", "invalid_role");

        var canGrant = await guard.CanGrantAsync(roles, ct);
        if (!canGrant.IsSuccess) return Result.Failure<Guid>(canGrant.Error!, canGrant.ErrorCode!);

        var user = new User
        {
            Email = email,
            PasswordHash = hasher.Hash(request.Password),
            FirstName = request.FirstName.Trim(),
            LastName = (request.LastName ?? string.Empty).Trim(),
            IsActive = true
        };
        user.UserRoles = roles.Select(r => new UserRole { UserId = user.Id, RoleId = r.Id }).ToList();

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return Result.Success(user.Id);
    }
}

public sealed record SetUserActiveCommand(Guid UserId, bool IsActive) : IRequest<Result>;

public sealed class SetUserActiveCommandHandler(IdentityDbContext db, IdentityAccessGuard guard) : IRequestHandler<SetUserActiveCommand, Result>
{
    public async Task<Result> Handle(SetUserActiveCommand request, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([request.UserId], ct);
        if (user is null) return Result.Failure("User not found.", "not_found");

        var allowed = await guard.CanManageUserAsync(user.Id, ct);
        if (!allowed.IsSuccess) return allowed;

        if (!request.IsActive)
        {
            var notSelf = guard.NotSelf(user.Id, "deactivate");
            if (!notSelf.IsSuccess) return notSelf;
            var keepsSuper = await guard.KeepsASuperAdminAsync(user.Id, ct);
            if (!keepsSuper.IsSuccess) return keepsSuper;
            await guard.RevokeSessionsAsync(user.Id, ct);
        }

        user.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        guard.ForgetActiveStatus(user.Id);
        return Result.Success();
    }
}

public sealed record ChangePasswordCommand(Guid UserId, string NewPassword) : IRequest<Result>;

public sealed class ChangePasswordCommandHandler(IdentityDbContext db, IPasswordHasherService hasher, IdentityAccessGuard guard)
    : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([request.UserId], ct);
        if (user is null) return Result.Failure("User not found.", "not_found");

        var allowed = await guard.CanManageUserAsync(user.Id, ct);
        if (!allowed.IsSuccess) return allowed;
        if (PasswordPolicy.Validate(request.NewPassword, user.Email) is { } passwordError)
            return Result.Failure(passwordError, "validation");

        user.PasswordHash = hasher.Hash(request.NewPassword);
        // A password reset signs the account out everywhere.
        await guard.RevokeSessionsAsync(user.Id, ct);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result<string>>;

public sealed class DeleteUserCommandHandler(IdentityDbContext db, IdentityAccessGuard guard) : IRequestHandler<DeleteUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([request.UserId], ct);
        if (user is null) return Result.Failure<string>("User not found.", "not_found");

        foreach (var check in new[] { guard.NotSelf(user.Id, "delete"), await guard.CanManageUserAsync(user.Id, ct), await guard.KeepsASuperAdminAsync(user.Id, ct) })
            if (!check.IsSuccess) return Result.Failure<string>(check.Error!, check.ErrorCode!);

        // Role links and refresh tokens are removed by cascade.
        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
        guard.ForgetActiveStatus(user.Id);
        return Result.Success(user.Email);
    }
}
