namespace SummitCms.Shared.Kernel.Common;

/// <summary>
/// The fixed set of top-level marketing pages every content module hangs data off of
/// (via a soft "PageSlug" reference into SiteContent's `content.pages` table).
/// Kept here, rather than as a foreign key, so content modules don't have to
/// reference the SiteContent module just to know these strings.
/// </summary>
public static class PageSlugs
{
    public const string Home = "home";
    public const string About = "about";
    public const string Capabilities = "capabilities";
    public const string Careers = "careers";
    public const string Contact = "contact";
    public const string Industries = "industries";
    public const string Projects = "projects";

    public static readonly IReadOnlyList<string> All =
        [Home, About, Capabilities, Careers, Contact, Industries, Projects];
}
