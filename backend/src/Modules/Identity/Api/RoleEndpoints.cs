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

        group.MapPost("", async (CreateRoleRequest body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new CreateRoleCommand(body.Name, body.Description), ct);
            return result.IsSuccess ? Results.Created($"/api/admin/roles/{result.Value}", new { id = result.Value }) : Results.BadRequest(new { error = result.Error });
        });

        group.MapPut("/{id:guid}/permissions", async (Guid id, SetPermissionsRequest body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SetRolePermissionsCommand(id, body.PermissionIds), ct);
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(new { error = result.Error });
        });

        app.MapPost("/api/admin/users/{userId:guid}/roles", async (Guid userId, AssignRoleRequest body, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new AssignRoleToUserCommand(userId, body.RoleId), ct);
                return result.IsSuccess ? Results.NoContent() : Results.BadRequest(new { error = result.Error });
            })
            .WithTags("Admin.Users")
            .RequirePermission(IdentityPermissions.ManageUsers);

        app.MapDelete("/api/admin/users/{userId:guid}/roles/{roleId:guid}", async (Guid userId, Guid roleId, ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new RemoveRoleFromUserCommand(userId, roleId), ct);
                return Results.NoContent();
            })
            .WithTags("Admin.Users")
            .RequirePermission(IdentityPermissions.ManageUsers);
    }
}
