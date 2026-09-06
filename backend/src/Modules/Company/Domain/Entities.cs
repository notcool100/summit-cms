using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.Company.Domain;

/// <summary>All About-page reference data hangs off the "about" Page (via PageId, resolved through SiteContent's IPageCatalog at seed/admin time).</summary>
public abstract class AboutPageEntity : AuditableEntity, IOrderable
{
    public Guid PageId { get; set; }
    public int DisplayOrder { get; set; }
}

public class Milestone : AboutPageEntity
{
    public string Year { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public class CompanyValue : AboutPageEntity
{
    public string Code { get; set; } = string.Empty; // e.g. "01"
    public string Name { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public class TeamMember : AboutPageEntity
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Guid? MediaId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class OfficeLocation : AboutPageEntity
{
    public string City { get; set; } = string.Empty;
    public string RoleDescription { get; set; } = string.Empty;
    public bool IsHeadquarters { get; set; }
}

public class Award : AboutPageEntity
{
    public string Year { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class NarrativeBlock : AboutPageEntity
{
    public string Eyebrow { get; set; } = string.Empty;
    public string TitleLine1 { get; set; } = string.Empty;
    public string TitleLine2 { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Guid? MediaId { get; set; }
    public string ImageCaption { get; set; } = string.Empty;
    public bool ImageFirst { get; set; }
}
