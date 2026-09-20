using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.SiteContent.Domain;

/// <summary>
/// An immutable snapshot of a <see cref="Page"/>'s editable content, taken on every draft save.
/// Supports draft/publish/rollback: <see cref="Page.PublishedVersionId"/> points at whichever row
/// here is currently live. Versions are never mutated after creation, so this inherits the plain
/// <see cref="Entity"/> base (own <see cref="CreatedAt"/>, no <c>UpdatedAt</c>).
/// </summary>
public class PageVersion : Entity
{
    public Guid PageId { get; set; }
    public Page Page { get; set; } = null!;
    public int VersionNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string HeroHeading { get; set; } = string.Empty;
    public string HeroSubheading { get; set; } = string.Empty;
    public Guid? HeroMediaId { get; set; }
    public Guid? SecondaryMediaId { get; set; }
    public bool IsPublished { get; set; }

    /// <summary>Soft reference to identity.users (no cross-schema FK per this repo's convention) - who authored this version.</summary>
    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
