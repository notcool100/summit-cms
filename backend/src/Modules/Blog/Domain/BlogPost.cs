using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.Blog.Domain;

public enum BlogPostStatus { Draft = 0, Published = 1 }

public class BlogPost : AuditableEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    /// <summary>Plain-text paragraphs separated by blank lines; the public site renders each as its own &lt;p&gt;.</summary>
    public string Body { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorRole { get; set; } = string.Empty;
    public Guid? CoverMediaId { get; set; }
    public BlogPostStatus Status { get; set; } = BlogPostStatus.Draft;
    public DateTimeOffset? PublishedAt { get; set; }
    public bool IsFeatured { get; set; }
}
