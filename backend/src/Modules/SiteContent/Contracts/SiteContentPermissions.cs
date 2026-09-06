namespace SummitCms.Modules.SiteContent.Contracts;

public static class SiteContentPermissions
{
    public const string ManagePages = "content.pages.manage";
    public const string ManageSettings = "content.settings.manage";
    public const string ManageStats = "content.stats.manage";
    public const string ManageEnquiryTypes = "content.enquiry-types.manage";

    public static readonly IReadOnlyList<(string Code, string Description)> All =
    [
        (ManagePages, "Edit page hero content and SEO metadata"),
        (ManageSettings, "Edit global site settings (contact info, social links, footer)"),
        (ManageStats, "Edit homepage/about/careers metric stats"),
        (ManageEnquiryTypes, "Edit the contact form's enquiry-type options")
    ];
}
