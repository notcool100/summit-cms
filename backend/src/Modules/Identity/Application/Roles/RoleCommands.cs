using MediatR;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Identity.Domain;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Modules.Identity.Application.Roles;

public sealed record CreateRoleCommand(string Name, string Description) : IRequest<Result<Guid>>;

public sealed class CreateRoleCommandHandler(IdentityDbContext db) : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        if (await db.Roles.AnyAsync(r => r.Name == request.Name, ct))
            return Result.Failure<Guid>("A role with this name already exists.", "duplicate_role");

        var role = new Role { Name = request.Name.Trim(), Description = request.Description.Trim() };
        db.Roles.Add(role);
        await db.SaveChangesAsync(ct);
        return Result.Success(role.Id);
    }
}

public sealed record SetRolePermissionsCommand(Guid RoleId, List<Guid> PermissionIds) : IRequest<Result>;

public sealed class SetRolePermissionsCommandHandler(IdentityDbContext db)
    : IRequestHandler<SetRolePermissionsCommand, Result>
{
    public async Task<Result> Handle(SetRolePermissionsCommand request, CancellationToken ct)
    {
        var role = await db.Roles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == request.RoleId, ct);
        if (role is null) return Result.Failure("Role not found.", "not_found");

        if (role.IsSystemRole)
            return Result.Failure("System role permissions are fixed and cannot be edited.", "system_role");

        var validPermissionIds = await db.Permissions
            .Where(p => request.PermissionIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(ct);

        role.RolePermissions.Clear();
        role.RolePermissions.AddRange(validPermissionIds.Select(pid => new RolePermission { RoleId = role.Id, PermissionId = pid }));

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public sealed record AssignRoleToUserCommand(Guid UserId, Guid RoleId) : IRequest<Result>;

public sealed class AssignRoleToUserCommandHandler(IdentityDbContext db) : IRequestHandler<AssignRoleToUserCommand, Result>
{
    public async Task<Result> Handle(AssignRoleToUserCommand request, CancellationToken ct)
    {
        var userExists = await db.Users.AnyAsync(u => u.Id == request.UserId, ct);
        var roleExists = await db.Roles.AnyAsync(r => r.Id == request.RoleId, ct);
        if (!userExists || !roleExists) return Result.Failure("User or role not found.", "not_found");

        var already = await db.UserRoles.AnyAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId, ct);
        if (!already)
        {
            db.UserRoles.Add(new UserRole { UserId = request.UserId, RoleId = request.RoleId });
            await db.SaveChangesAsync(ct);
        }

        return Result.Success();
    }
}

public sealed record RemoveRoleFromUserCommand(Guid UserId, Guid RoleId) : IRequest<Result>;

public sealed class RemoveRoleFromUserCommandHandler(IdentityDbContext db) : IRequestHandler<RemoveRoleFromUserCommand, Result>
{
    public async Task<Result> Handle(RemoveRoleFromUserCommand request, CancellationToken ct)
    {
        var link = await db.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId, ct);
        if (link is null) return Result.Success();

        db.UserRoles.Remove(link);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
