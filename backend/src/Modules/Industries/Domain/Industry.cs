using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.Industries.Domain;

public class Industry : AuditableEntity, IOrderable
{
    public string Idx { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Guid? MediaId { get; set; }
    public string FigureLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public List<IndustryProjectLink> ProjectLinks { get; set; } = [];
}

/// <summary>
/// Soft-references a Project (by Guid, resolved through Projects' IProjectCatalog) instead of
/// hand-duplicating its name/stat, like the old static RelatedLink[] did - CustomLabel/CustomStat
/// let an admin override the display text for this one link when needed.
/// </summary>
public class IndustryProjectLink : AuditableEntity, IOrderable
{
    public Guid IndustryId { get; set; }
    public Industry Industry { get; set; } = null!;
    public Guid ProjectId { get; set; }
    public string? CustomLabel { get; set; }
    public string? CustomStat { get; set; }
    public int DisplayOrder { get; set; }
}
