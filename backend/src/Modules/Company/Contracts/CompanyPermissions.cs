namespace SummitCms.Modules.Company.Contracts;

public static class CompanyPermissions
{
    public const string Manage = "company.manage";

    public static readonly IReadOnlyList<(string Code, string Description)> All =
    [
        (Manage, "Manage About-page content: milestones, values, team, offices, awards, narrative")
    ];
}
