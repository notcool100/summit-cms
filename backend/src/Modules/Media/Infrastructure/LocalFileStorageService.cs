using Microsoft.Extensions.Options;
using SummitCms.Modules.Media.Application;

namespace SummitCms.Modules.Media.Infrastructure;

public sealed class LocalFileStorageService(IOptions<LocalFileStorageOptions> options) : IFileStorageService
{
    private readonly LocalFileStorageOptions _options = options.Value;

    public async Task<StoredFile> SaveAsync(Stream content, string fileName, string contentType, CancellationToken ct)
    {
        Directory.CreateDirectory(_options.RootPath);

        var safeExtension = Path.GetExtension(fileName);
        var storageKey = $"{Guid.CreateVersion7():N}{safeExtension}";
        var fullPath = Path.Combine(_options.RootPath, storageKey);

        await using var fileStream = File.Create(fullPath);
        await content.CopyToAsync(fileStream, ct);

        return new StoredFile(storageKey, GetPublicUrl(storageKey));
    }

    public Task DeleteAsync(string storageKey, CancellationToken ct)
    {
        var fullPath = Path.Combine(_options.RootPath, storageKey);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public string GetPublicUrl(string storageKey) => $"{_options.PublicBaseUrl.TrimEnd('/')}/{storageKey}";
}
