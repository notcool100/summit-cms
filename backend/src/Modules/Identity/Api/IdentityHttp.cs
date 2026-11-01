using MediatR;
using Microsoft.AspNetCore.Http;
using SummitCms.Modules.Identity.Application.Security;
using SummitCms.Shared.Infrastructure.Auditing;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Modules.Identity.Api;

internal static class IdentityHttp
{
    /// <summary>Maps an application Result to the right HTTP status (403/404/409/400).</summary>
    public static IResult ToHttp(this Result result, Func<IResult> onSuccess)
    {
        if (result.IsSuccess) return onSuccess();
        var body = new { error = result.Error };
        return result.ErrorCode switch
        {
            IdentityAccessGuard.ForbiddenCode => Results.Json(body, statusCode: StatusCodes.Status403Forbidden),
            "not_found" => Results.NotFound(body),
            IdentityAccessGuard.ConflictCode or "duplicate_email" or "duplicate_role" => Results.Conflict(body),
            "system_role" => Results.Conflict(body),
            _ => Results.BadRequest(body)
        };
    }

    /// <summary>Writes an identity change to the audit log with the (proxy-resolved) client IP.</summary>
    public static Task AuditAsync(this IPublisher publisher, HttpContext http, Guid? actorId, string action,
        string entity, string entityId, string? before = null, string? after = null, CancellationToken ct = default) =>
        publisher.Publish(new EntityAuditEvent(actorId, action, entity, entityId, before, after,
            http.Connection.RemoteIpAddress?.ToString()), ct);
}
