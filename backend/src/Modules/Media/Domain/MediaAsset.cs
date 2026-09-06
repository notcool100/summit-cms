using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.Media.Domain;

public enum MediaSourceType
{
    /// <summary>Uploaded through the admin and stored via IFileStorageService.</summary>
    Uploaded = 0,
    /// <summary>A pre-existing external URL (e.g. content migrated from the old CDN).</summary>
    External = 1
}

public class MediaAsset : AuditableEntity
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string AltText { get; set; } = string.Empty;

    public MediaSourceType SourceType { get; set; }
    /// <summary>Set when SourceType == Uploaded; the key IFileStorageService uses to resolve/delete the file.</summary>
    public string? StorageKey { get; set; }
    /// <summary>Set when SourceType == External.</summary>
    public string? ExternalUrl { get; set; }

    public Guid? FolderId { get; set; }
    public MediaFolder? Folder { get; set; }
    public Guid? UploadedByUserId { get; set; }
}
