namespace SummitCms.Modules.Contact.Contracts;

public static class ContactPermissions
{
    public const string Manage = "contact.manage";

    public static readonly IReadOnlyList<(string Code, string Description)> All =
    [
        (Manage, "View and triage contact-form submissions")
    ];
}
