namespace SummitCms.Modules.Projects.Contracts;

public static class ProjectsPermissions
{
    public const string Manage = "projects.manage";

    public static readonly IReadOnlyList<(string Code, string Description)> All =
    [
        (Manage, "Manage projects, their gallery, scope facts, narrative, and quotes")
    ];
}
