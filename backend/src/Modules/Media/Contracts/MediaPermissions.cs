namespace SummitCms.Modules.Media.Contracts;

public static class MediaPermissions
{
    public const string Manage = "media.manage";

    public static readonly IReadOnlyList<(string Code, string Description)> All =
    [
        (Manage, "Upload, edit, and delete media library assets")
    ];
}
