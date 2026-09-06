using MediatR;
using SummitCms.Modules.Identity.Domain;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Shared.Infrastructure.Auditing;

namespace SummitCms.Modules.Identity.Application.Auditing;

/// <summary>The single subscriber for audit events published by every module in the system.</summary>
public sealed class EntityAuditEventHandler(IdentityDbContext db, TimeProvider timeProvider) : INotificationHandler<EntityAuditEvent>
{
    public async Task Handle(EntityAuditEvent notification, CancellationToken ct)
    {
        db.AuditLogs.Add(new AuditLog
        {
            UserId = notification.UserId,
            Action = notification.Action,
            EntityName = notification.EntityName,
            EntityId = notification.EntityId,
            DataBefore = notification.DataBefore,
            DataAfter = notification.DataAfter,
            IpAddress = notification.IpAddress,
            CreatedAt = timeProvider.GetUtcNow()
        });
        await db.SaveChangesAsync(ct);
    }
}
