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

/// <summary>
/// A configurable content block on a marketing page (card grid, fact strip, quote, people list, FAQ).
/// Pages render the active sections for their slug at fixed <see cref="Slot"/> positions, so admins can
/// add, reorder, edit or hide blocks without a deploy. Pages are referenced by slug, not by FK, because
/// the Page table lives in the SiteContent module's schema.
/// </summary>
public class ContentSection : AuditableEntity, IOrderable
{
    public string Key { get; set; } = string.Empty;
    public string PageSlug { get; set; } = string.Empty;
    public string Slot { get; set; } = string.Empty;
    public string Layout { get; set; } = ContentLayouts.Cards;
    public string Eyebrow { get; set; } = string.Empty;
    /// <summary>Heading lines, one per newline.</summary>
    public string Heading { get; set; } = string.Empty;
    public string Intro { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public int Columns { get; set; } = 4;
    public bool Numbered { get; set; }
    public bool Tinted { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public List<ContentItem> Items { get; set; } = [];
}

public class ContentItem : AuditableEntity, IOrderable
{
    public Guid SectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    /// <summary>Short tags, one per newline.</summary>
    public string Tags { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public static class ContentLayouts
{
    public const string Cards = "cards";     // Title + Body cards
    public const string Facts = "facts";     // Title = label, Body = value
    public const string Quote = "quote";     // Intro = statement, Note = sign-off
    public const string People = "people";   // Title name, Subtitle role, Detail credential, Body bio, Tags
    public const string Faq = "faq";         // Title = question, Body = answer
    public static readonly string[] All = [Cards, Facts, Quote, People, Faq];
}
