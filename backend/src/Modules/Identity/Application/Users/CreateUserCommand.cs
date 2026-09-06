using MediatR;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Identity.Domain;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Modules.Identity.Application.Users;

public sealed record CreateUserCommand(
    string Email, string Password, string FirstName, string LastName, List<string> RoleNames)
    : IRequest<Result<Guid>>;

public sealed class CreateUserCommandHandler(IdentityDbContext db, IPasswordHasherService hasher)
    : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email, ct))
            return Result.Failure<Guid>("A user with this email already exists.", "duplicate_email");

        var roles = await db.Roles.Where(r => request.RoleNames.Contains(r.Name)).ToListAsync(ct);
        if (roles.Count != request.RoleNames.Distinct().Count())
            return Result.Failure<Guid>("One or more roles do not exist.", "invalid_role");

        var user = new User
        {
            Email = email,
            PasswordHash = hasher.Hash(request.Password),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            IsActive = true
        };
        user.UserRoles = roles.Select(r => new UserRole { UserId = user.Id, RoleId = r.Id }).ToList();

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return Result.Success(user.Id);
    }
}

public sealed record SetUserActiveCommand(Guid UserId, bool IsActive) : IRequest<Result>;

public sealed class SetUserActiveCommandHandler(IdentityDbContext db) : IRequestHandler<SetUserActiveCommand, Result>
{
    public async Task<Result> Handle(SetUserActiveCommand request, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([request.UserId], ct);
        if (user is null) return Result.Failure("User not found.", "not_found");

        user.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public sealed record ChangePasswordCommand(Guid UserId, string NewPassword) : IRequest<Result>;

public sealed class ChangePasswordCommandHandler(IdentityDbContext db, IPasswordHasherService hasher)
    : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([request.UserId], ct);
        if (user is null) return Result.Failure("User not found.", "not_found");

        user.PasswordHash = hasher.Hash(request.NewPassword);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
