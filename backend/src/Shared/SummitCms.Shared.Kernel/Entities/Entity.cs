namespace SummitCms.Shared.Kernel.Entities;

/// <summary>
/// Base type for every entity in every module. Ids are time-ordered (UUIDv7) so they
/// sort naturally and stay index-friendly as primary keys.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.CreateVersion7();
}

/// <summary>
/// An entity whose creation/update timestamps are stamped automatically by
/// <c>AuditableEntitySaveChangesInterceptor</c> on every <c>SaveChanges</c>.
/// </summary>
public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// Convenience contract for reference-data entities rendered as ordered lists
/// (capabilities, milestones, gallery images, ...).
/// </summary>
public interface IOrderable
{
    int DisplayOrder { get; set; }
}
