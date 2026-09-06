using MediatR;

namespace SummitCms.Shared.Infrastructure.Auditing;

/// <summary>
/// Published by the generic CRUD endpoints (and any bespoke command handler that wants to)
/// whenever an admin mutates a record. Identity module is the only subscriber - it writes
/// these into identity.audit_logs - so every module gets audit trail without a DB-level
/// dependency on the Identity schema.
/// </summary>
public sealed record EntityAuditEvent(
    Guid? UserId,
    string Action,
    string EntityName,
    string EntityId,
    string? DataBefore,
    string? DataAfter,
    string? IpAddress) : INotification;
