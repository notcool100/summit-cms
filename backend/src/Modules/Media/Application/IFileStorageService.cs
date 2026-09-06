namespace SummitCms.Modules.Media.Application;

public sealed record StoredFile(string StorageKey, string PublicUrl);

/// <summary>Swappable behind local disk today, an S3-compatible bucket later - callers never see the difference.</summary>
public interface IFileStorageService
{
    Task<StoredFile> SaveAsync(Stream content, string fileName, string contentType, CancellationToken ct);
    Task DeleteAsync(string storageKey, CancellationToken ct);
    string GetPublicUrl(string storageKey);
}
