using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Shared.Infrastructure.Security;

namespace SummitCms.Shared.Infrastructure.Auditing;

/// <summary>
/// Opt-in audit logging for hand-written admin endpoints that don't go through MapAdminCrud.
/// After the handler succeeds (2xx), publishes an <see cref="EntityAuditEvent"/> with the action
/// (POST = Created, PUT = Updated, DELETE = Deleted), the record id from the route, the request body
/// as the "after" data, and the client IP. Usage: <c>group.MapPut(...).WithAudit("ProjectQuote");</c>
/// </summary>
public static class AuditEndpointFilterExtensions
{
    private static readonly string[] IdRouteKeys = ["id", "projectId", "sectionId", "industryId", "userId"];

    public static RouteHandlerBuilder WithAudit(this RouteHandlerBuilder builder, string entityName) =>
        builder.AddEndpointFilter(async (context, next) =>
        {
            var result = await next(context);

            var status = result switch
            {
                IStatusCodeHttpResult s => s.StatusCode ?? 200,
                null => 200,
                _ => 200
            };
            if (status is < 200 or >= 300) return result;

            var http = context.HttpContext;
            var action = http.Request.Method switch
            {
                "POST" => "Created",
                "PUT" or "PATCH" => "Updated",
                "DELETE" => "Deleted",
                _ => null
            };
            if (action is null) return result;

            var entityId = IdRouteKeys
                .Select(k => http.Request.RouteValues.TryGetValue(k, out var v) ? v?.ToString() : null)
                .FirstOrDefault(v => !string.IsNullOrEmpty(v)) ?? string.Empty;

            // The request body is the first argument that's a plain DTO (not an id, service or token).
            var body = context.Arguments.FirstOrDefault(a => a is not null && IsRequestDto(a.GetType()));

            var services = http.RequestServices;
            var user = services.GetRequiredService<ICurrentUser>();
            var publisher = services.GetRequiredService<IPublisher>();
            await publisher.Publish(new EntityAuditEvent(
                user.UserId, action, entityName, entityId,
                null,
                body is null ? null : JsonSerializer.Serialize(body, body.GetType()),
                http.Connection.RemoteIpAddress?.ToString()));

            return result;
        });

    private static bool IsRequestDto(Type t) =>
        t.Name.EndsWith("Dto", StringComparison.Ordinal) ||
        t.Name.EndsWith("Request", StringComparison.Ordinal) ||
        t.Name.EndsWith("Write", StringComparison.Ordinal);
}
