using Microsoft.AspNetCore.Builder;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Identity.Application.Users;
using SummitCms.Modules.Identity.Contracts;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Shared.Infrastructure.Security;

namespace SummitCms.Modules.Identity.Api;

public static class UserEndpoints
{
    public sealed record UserListItem(Guid Id, string Email, string FullName, bool IsActive, DateTimeOffset? LastLoginAt, List<string> Roles);
    public sealed record CreateUserRequest(string Email, string Password, string FirstName, string LastName, List<string> RoleNames);
    public sealed record SetActiveRequest(bool IsActive);
    public sealed record ChangePasswordRequest(string NewPassword);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/users").WithTags("Admin.Users").RequirePermission(IdentityPermissions.ManageUsers);

        group.MapGet("", async (IdentityDbContext db, CancellationToken ct) =>
            Results.Ok(await db.Users
                .Select(u => new UserListItem(
                    u.Id, u.Email, u.FullName, u.IsActive, u.LastLoginAt,
                    u.UserRoles.Select(ur => ur.Role.Name).ToList()))
                .ToListAsync(ct)));

        group.MapPost("", async (CreateUserRequest body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new CreateUserCommand(body.Email, body.Password, body.FirstName, body.LastName, body.RoleNames), ct);
            return result.IsSuccess ? Results.Created($"/api/admin/users/{result.Value}", new { id = result.Value }) : Results.BadRequest(new { error = result.Error });
        });

        group.MapPut("/{id:guid}/active", async (Guid id, SetActiveRequest body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SetUserActiveCommand(id, body.IsActive), ct);
            return result.IsSuccess ? Results.NoContent() : Results.NotFound(new { error = result.Error });
        });

        group.MapPut("/{id:guid}/password", async (Guid id, ChangePasswordRequest body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ChangePasswordCommand(id, body.NewPassword), ct);
            return result.IsSuccess ? Results.NoContent() : Results.NotFound(new { error = result.Error });
        });
    }
}
