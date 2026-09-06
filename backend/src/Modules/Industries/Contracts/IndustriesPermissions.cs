namespace SummitCms.Modules.Industries.Contracts;

public static class IndustriesPermissions
{
    public const string Manage = "industries.manage";

    public static readonly IReadOnlyList<(string Code, string Description)> All =
    [
        (Manage, "Manage industries served and their linked projects")
    ];
}
