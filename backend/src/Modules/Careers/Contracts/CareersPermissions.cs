namespace SummitCms.Modules.Careers.Contracts;

public static class CareersPermissions
{
    public const string Manage = "careers.manage";

    public static readonly IReadOnlyList<(string Code, string Description)> All =
    [
        (Manage, "Manage career tracks and job openings")
    ];
}
