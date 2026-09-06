using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.Capabilities.Domain;

public enum CapabilityBackground { Paper = 0, Panel = 1 }

/// <summary>
/// Single source of truth for both the home-page teaser strip and the capabilities-page panels -
/// the old static frontend duplicated this data across home.ts and capabilities.ts.
/// </summary>
public class Capability : AuditableEntity, IOrderable
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    /// <summary>Short tag shown on the home-page teaser card.</summary>
    public string TeaserTag { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Stat { get; set; } = string.Empty;
    public string StatLabel { get; set; } = string.Empty;
    public CapabilityBackground Background { get; set; }
    public bool TextFirst { get; set; }
    public Guid? MediaId { get; set; }
    public string FigureLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
