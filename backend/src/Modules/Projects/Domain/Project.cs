using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.Projects.Domain;

public class Project : AuditableEntity, IOrderable
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid IndustryCategoryId { get; set; }
    public ProjectIndustryCategory IndustryCategory { get; set; } = null!;
    public string Stat { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }
    public Guid? HeroMediaId { get; set; }
    /// <summary>Masonry-grid layout hints for the /projects grid (e.g. "16/10", grid column span).</summary>
    public string? Ratio { get; set; }
    public int? Span { get; set; }

    public List<ProjectGalleryImage> GalleryImages { get; set; } = [];
    public List<ProjectScopeFact> ScopeFacts { get; set; } = [];
    public List<ProjectNarrativeSection> NarrativeSections { get; set; } = [];
    public ProjectQuote? Quote { get; set; }
}

public enum ProjectImageRole { Hero = 0, Break = 1, Gallery = 2 }

public class ProjectGalleryImage : AuditableEntity, IOrderable
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public Guid MediaId { get; set; }
    public ProjectImageRole Role { get; set; }
    public string Caption { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public class ProjectScopeFact : AuditableEntity, IOrderable
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public class ProjectNarrativeSection : AuditableEntity, IOrderable
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Idx { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public List<ProjectNarrativeParagraph> Paragraphs { get; set; } = [];
}

public class ProjectNarrativeParagraph : AuditableEntity
{
    public Guid NarrativeSectionId { get; set; }
    public ProjectNarrativeSection NarrativeSection { get; set; } = null!;
    public int ParagraphOrder { get; set; }
    public string Body { get; set; } = string.Empty;
}

public class ProjectQuote : AuditableEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Quote { get; set; } = string.Empty;
    public string Attribution { get; set; } = string.Empty;
}
