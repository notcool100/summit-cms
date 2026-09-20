using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.SiteContent.Domain;

/// <summary>One row per top-level marketing page (see PageSlugs) holding its hero/SEO content.</summary>
public class Page : AuditableEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string HeroHeading { get; set; } = string.Empty;
    public string HeroSubheading { get; set; } = string.Empty;
    public Guid? HeroMediaId { get; set; }
    public Guid? SecondaryMediaId { get; set; }

    /// <summary>The <see cref="PageVersion"/> currently live on the public site, if any version has ever been published.</summary>
    public Guid? PublishedVersionId { get; set; }

    /// <summary>When <see cref="PublishedVersionId"/> was last published.</summary>
    public DateTimeOffset? PublishedAt { get; set; }
}
