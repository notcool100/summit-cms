using MediatR;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Identity.Application.Security;
using SummitCms.Modules.Identity.Domain;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Modules.Identity.Application.Roles;

public sealed record CreateRoleCommand(string Name, string Description) : IRequest<Result<Guid>>;

public sealed class CreateRoleCommandHandler(IdentityDbContext db) : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        var name = (request.Name ?? string.Empty).Trim();
        if (name.Length is < 2 or > 60)
            return Result.Failure<Guid>("Role name must be 2 to 60 characters.", "validation");
        if ((request.Description?.Length ?? 0) > 300)
            return Result.Failure<Guid>("Description must be 300 characters or fewer.", "validation");
        if (await db.Roles.AnyAsync(r => r.Name == name, ct))
            return Result.Failure<Guid>("A role with this name already exists.", "duplicate_role");

        var role = new Role { Name = name, Description = (request.Description ?? string.Empty).Trim() };
        db.Roles.Add(role);
        await db.SaveChangesAsync(ct);
        return Result.Success(role.Id);
    }
}

public sealed record SetRolePermissionsCommand(Guid RoleId, List<Guid> PermissionIds) : IRequest<Result>;

public sealed class SetRolePermissionsCommandHandler(IdentityDbContext db, IdentityAccessGuard guard)
    : IRequestHandler<SetRolePermissionsCommand, Result>
{
    public async Task<Result> Handle(SetRolePermissionsCommand request, CancellationToken ct)
    {
        var role = await db.Roles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == request.RoleId, ct);
        if (role is null) return Result.Failure("Role not found.", "not_found");

        if (role.IsSystemRole)
            return Result.Failure("System role permissions are fixed and cannot be edited.", "system_role");

        var valid = await db.Permissions
            .Where(p => request.PermissionIds.Contains(p.Id))
            .Select(p => new { p.Id, p.Code })
            .ToListAsync(ct);
        var canGrant = await guard.CanGrantPermissionsAsync(valid.Select(p => p.Code).ToList(), ct);
        if (!canGrant.IsSuccess) return canGrant;
        var validPermissionIds = valid.Select(p => p.Id).ToList();

        role.RolePermissions.Clear();
        role.RolePermissions.AddRange(validPermissionIds.Select(pid => new RolePermission { RoleId = role.Id, PermissionId = pid }));

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public sealed record AssignRoleToUserCommand(Guid UserId, Guid RoleId) : IRequest<Result>;

public sealed class AssignRoleToUserCommandHandler(IdentityDbContext db, IdentityAccessGuard guard) : IRequestHandler<AssignRoleToUserCommand, Result>
{
    public async Task<Result> Handle(AssignRoleToUserCommand request, CancellationToken ct)
    {
        var userExists = await db.Users.AnyAsync(u => u.Id == request.UserId, ct);
        var role = await db.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId, ct);
        if (!userExists || role is null) return Result.Failure("User or role not found.", "not_found");

        var notSelf = guard.NotSelf(request.UserId, "change the roles on");
        if (!notSelf.IsSuccess) return notSelf;
        var canManage = await guard.CanManageUserAsync(request.UserId, ct);
        if (!canManage.IsSuccess) return canManage;
        var canGrant = await guard.CanGrantAsync([role], ct);
        if (!canGrant.IsSuccess) return canGrant;

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

public sealed class RemoveRoleFromUserCommandHandler(IdentityDbContext db, IdentityAccessGuard guard) : IRequestHandler<RemoveRoleFromUserCommand, Result>
{
    public async Task<Result> Handle(RemoveRoleFromUserCommand request, CancellationToken ct)
    {
        var link = await db.UserRoles.Include(ur => ur.Role).FirstOrDefaultAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId, ct);
        if (link is null) return Result.Success();

        var notSelf = guard.NotSelf(request.UserId, "change the roles on");
        if (!notSelf.IsSuccess) return notSelf;
        var canManage = await guard.CanManageUserAsync(request.UserId, ct);
        if (!canManage.IsSuccess) return canManage;
        if (link.Role.Name == IdentityAccessGuard.SuperAdminRole)
        {
            var keepsSuper = await guard.KeepsASuperAdminAsync(request.UserId, ct);
            if (!keepsSuper.IsSuccess) return keepsSuper;
        }

        db.UserRoles.Remove(link);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public sealed record DeleteRoleCommand(Guid RoleId) : IRequest<Result<string>>;

public sealed class DeleteRoleCommandHandler(IdentityDbContext db) : IRequestHandler<DeleteRoleCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DeleteRoleCommand request, CancellationToken ct)
    {
        var role = await db.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId, ct);
        if (role is null) return Result.Failure<string>("Role not found.", "not_found");
        if (role.IsSystemRole)
            return Result.Failure<string>("Built-in roles can't be deleted.", IdentityAccessGuard.ConflictCode);

        var holders = await db.UserRoles.CountAsync(ur => ur.RoleId == role.Id, ct);
        if (holders > 0)
            return Result.Failure<string>($"This role is still assigned to {holders} user{(holders == 1 ? "" : "s")}. Remove it from them first.", IdentityAccessGuard.ConflictCode);

        db.Roles.Remove(role); // role permissions are removed by cascade
        await db.SaveChangesAsync(ct);
        return Result.Success(role.Name);
    }
}
