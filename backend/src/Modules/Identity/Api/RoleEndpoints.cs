using Microsoft.AspNetCore.Builder;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Identity.Application.Roles;
using SummitCms.Modules.Identity.Contracts;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Shared.Infrastructure.Security;

namespace SummitCms.Modules.Identity.Api;

public static class RoleEndpoints
{
    public sealed record RoleListItem(Guid Id, string Name, string Description, bool IsSystemRole, List<string> PermissionCodes);
    public sealed record PermissionListItem(Guid Id, string Code, string Description);
    public sealed record CreateRoleRequest(string Name, string Description);
    public sealed record SetPermissionsRequest(List<Guid> PermissionIds);
    public sealed record AssignRoleRequest(Guid RoleId);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/roles").WithTags("Admin.Roles").RequirePermission(IdentityPermissions.ManageRoles);

        group.MapGet("", async (IdentityDbContext db, CancellationToken ct) =>
            Results.Ok(await db.Roles
                .Select(r => new RoleListItem(
                    r.Id, r.Name, r.Description, r.IsSystemRole,
                    r.RolePermissions.Select(rp => rp.Permission.Code).ToList()))
                .ToListAsync(ct)));

        group.MapGet("/permissions", async (IdentityDbContext db, CancellationToken ct) =>
            Results.Ok(await db.Permissions
                .Select(p => new PermissionListItem(p.Id, p.Code, p.Description))
                .ToListAsync(ct)));

        group.MapPost("", async (CreateRoleRequest body, ISender sender, IPublisher publisher, ICurrentUser actor, HttpContext http, CancellationToken ct) =>
        {
            var result = await sender.Send(new CreateRoleCommand(body.Name, body.Description), ct);
            if (result.IsSuccess)
                await publisher.AuditAsync(http, actor.UserId, "Created", "Role", result.Value.ToString(), after: body.Name?.Trim(), ct: ct);
            return result.ToHttp(() => Results.Created($"/api/admin/roles/{result.Value}", new { id = result.Value }));
        });

        group.MapPut("/{id:guid}/permissions", async (Guid id, SetPermissionsRequest body, ISender sender, IdentityDbContext db, IPublisher publisher, ICurrentUser actor, HttpContext http, CancellationToken ct) =>
        {
            var before = string.Join(", ", await db.RolePermissions.Where(rp => rp.RoleId == id).Select(rp => rp.Permission.Code).ToListAsync(ct));
            var result = await sender.Send(new SetRolePermissionsCommand(id, body.PermissionIds ?? []), ct);
            if (result.IsSuccess)
            {
                var after = string.Join(", ", await db.RolePermissions.Where(rp => rp.RoleId == id).Select(rp => rp.Permission.Code).ToListAsync(ct));
                await publisher.AuditAsync(http, actor.UserId, "PermissionsChanged", "Role", id.ToString(), before, after, ct);
            }
            return result.ToHttp(Results.NoContent);
        });

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, IPublisher publisher, ICurrentUser actor, HttpContext http, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteRoleCommand(id), ct);
            if (result.IsSuccess)
                await publisher.AuditAsync(http, actor.UserId, "Deleted", "Role", id.ToString(), before: result.Value, ct: ct);
            return result.ToHttp(Results.NoContent);
        });

        app.MapPost("/api/admin/users/{userId:guid}/roles", async (Guid userId, AssignRoleRequest body, ISender sender, IPublisher publisher, ICurrentUser actor, HttpContext http, CancellationToken ct) =>
            {
                var result = await sender.Send(new AssignRoleToUserCommand(userId, body.RoleId), ct);
                if (result.IsSuccess)
                    await publisher.AuditAsync(http, actor.UserId, "RoleAssigned", "User", userId.ToString(), after: body.RoleId.ToString(), ct: ct);
                return result.ToHttp(Results.NoContent);
            })
            .WithTags("Admin.Users")
            .RequirePermission(IdentityPermissions.ManageUsers);

        app.MapDelete("/api/admin/users/{userId:guid}/roles/{roleId:guid}", async (Guid userId, Guid roleId, ISender sender, IPublisher publisher, ICurrentUser actor, HttpContext http, CancellationToken ct) =>
            {
                var result = await sender.Send(new RemoveRoleFromUserCommand(userId, roleId), ct);
                if (result.IsSuccess)
                    await publisher.AuditAsync(http, actor.UserId, "RoleRemoved", "User", userId.ToString(), before: roleId.ToString(), ct: ct);
                return result.ToHttp(Results.NoContent);
            })
            .WithTags("Admin.Users")
            .RequirePermission(IdentityPermissions.ManageUsers);
    }
}
