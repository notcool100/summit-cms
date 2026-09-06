using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.Identity.Domain;

/// <summary>
/// Immutable record of every admin mutation across every module (written by
/// <c>AuditLoggingSaveChangesInterceptor</c> registered in each module's DbContext), so
/// "who changed what, when" is always answerable regardless of which module owns the data.
/// </summary>
public class AuditLog : Entity
{
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty; // Created | Updated | Deleted
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? DataBefore { get; set; } // jsonb
    public string? DataAfter { get; set; }  // jsonb
    public string? IpAddress { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
