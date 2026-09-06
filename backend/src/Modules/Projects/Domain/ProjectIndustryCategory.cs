using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.Projects.Domain;

/// <summary>Admin-extensible lookup (Semiconductor, Power, Energy & Terminals, Renewables, Manufacturing, ...)
/// instead of a hardcoded enum, so a new industry category doesn't need a code change.</summary>
public class ProjectIndustryCategory : AuditableEntity, IOrderable
{
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
