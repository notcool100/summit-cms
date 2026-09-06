namespace SummitCms.Modules.Media.Infrastructure;

public sealed class LocalFileStorageOptions
{
    public const string SectionName = "FileStorage";

    /// <summary>Absolute path on disk where uploads are written.</summary>
    public string RootPath { get; set; } = "App_Data/uploads";
    /// <summary>Public base URL the host serves that root path from (see Program.cs static file mapping).</summary>
    public string PublicBaseUrl { get; set; } = "/uploads";
}
