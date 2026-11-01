using Microsoft.AspNetCore.Builder;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using SummitCms.Modules.Identity.Application.Auth;
using SummitCms.Shared.Infrastructure.Security;

namespace SummitCms.Modules.Identity.Api;

public static class AuthEndpoints
{
    public sealed record LoginRequest(string Email, string Password);
    public sealed record RefreshRequest(string RefreshToken);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (
                [FromBody] LoginRequest body, ISender sender, IPublisher publisher, HttpContext http, CancellationToken ct) =>
            {
                var ip = http.Connection.RemoteIpAddress?.ToString();
                var email = (body.Email ?? string.Empty).Trim().ToLowerInvariant();
                var result = await sender.Send(new LoginCommand(email, body.Password ?? string.Empty, ip), ct);
                if (result.IsSuccess)
                {
                    await publisher.AuditAsync(http, result.Value.User.Id, "SignedIn", "User", result.Value.User.Id.ToString(), ct: ct);
                    return Results.Ok(result.Value);
                }
                await publisher.AuditAsync(http, null, result.ErrorCode == LoginCommandHandler.LockedOutCode ? "SignInLockedOut" : "SignInFailed",
                    "User", email.Length > 254 ? email[..254] : email, ct: ct);
                return Results.Json(new { error = result.Error },
                    statusCode: result.ErrorCode == LoginCommandHandler.LockedOutCode ? StatusCodes.Status429TooManyRequests : StatusCodes.Status401Unauthorized);
            })
            .RequireRateLimiting("auth")
            .WithName("Login");

        group.MapPost("/refresh", async ([FromBody] RefreshRequest body, ISender sender, HttpContext http, CancellationToken ct) =>
            {
                var ip = http.Connection.RemoteIpAddress?.ToString();
                var result = await sender.Send(new RefreshTokenCommand(body.RefreshToken, ip), ct);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.Json(new { error = result.Error }, statusCode: 401);
            })
            .RequireRateLimiting("auth")
            .WithName("RefreshToken");

        group.MapPost("/logout", async ([FromBody] RefreshRequest body, ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new LogoutCommand(body.RefreshToken), ct);
                return Results.NoContent();
            })
            .WithName("Logout");

        group.MapGet("/me", (ICurrentUser user) => Results.Ok(new
            {
                userId = user.UserId,
                email = user.Email,
                roles = user.Roles
            }))
            .RequireAuthorization()
            .WithName("Me");
    }
}
