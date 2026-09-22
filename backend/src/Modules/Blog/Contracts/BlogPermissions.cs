namespace SummitCms.Modules.Blog.Contracts;

public static class BlogPermissions
{
    public const string Manage = "blog.manage";

    public static readonly IReadOnlyList<(string Code, string Description)> All =
    [
        (Manage, "Manage blog posts, drafts, and publishing")
    ];
}
