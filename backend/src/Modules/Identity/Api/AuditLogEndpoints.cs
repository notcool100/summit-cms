using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Identity.Contracts;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Shared.Infrastructure.Security;
using SummitCms.Shared.Kernel.Pagination;

namespace SummitCms.Modules.Identity.Api;

public static class AuditLogEndpoints
{
    public sealed record AuditLogItem(
        Guid Id, Guid? UserId, string Action, string EntityName, string EntityId,
        string? DataBefore, string? DataAfter, string? IpAddress, DateTimeOffset CreatedAt);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/audit-logs", async (
                int page, int pageSize, IdentityDbContext db, CancellationToken ct) =>
            {
                var request = new PageRequest(page <= 0 ? 1 : page, pageSize <= 0 ? 25 : pageSize);
                var query = db.AuditLogs.OrderByDescending(a => a.CreatedAt);

                var total = await query.CountAsync(ct);
                var items = await query.Skip(request.Skip).Take(request.NormalizedPageSize)
                    .Select(a => new AuditLogItem(a.Id, a.UserId, a.Action, a.EntityName, a.EntityId, a.DataBefore, a.DataAfter, a.IpAddress, a.CreatedAt))
                    .ToListAsync(ct);

                return Results.Ok(PagedResult<AuditLogItem>.Create(items, total, request));
            })
            .WithTags("Admin.AuditLog")
            .RequirePermission(IdentityPermissions.ReadAuditLog);
    }
}
