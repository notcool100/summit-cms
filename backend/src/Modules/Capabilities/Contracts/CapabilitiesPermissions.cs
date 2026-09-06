namespace SummitCms.Modules.Capabilities.Contracts;

public static class CapabilitiesPermissions
{
    public const string Manage = "capabilities.manage";

    public static readonly IReadOnlyList<(string Code, string Description)> All =
    [
        (Manage, "Manage capability panels shown on the home and capabilities pages")
    ];
}
