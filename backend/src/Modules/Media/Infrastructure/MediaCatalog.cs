using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Media.Application;
using SummitCms.Modules.Media.Contracts;

namespace SummitCms.Modules.Media.Infrastructure;

public sealed class MediaCatalog(MediaDbContext db, IFileStorageService storage) : IMediaCatalog
{
    public async Task<MediaSummary?> GetAsync(Guid mediaId, CancellationToken ct)
    {
        var asset = await db.MediaAssets.AsNoTracking().FirstOrDefaultAsync(a => a.Id == mediaId, ct);
        return asset is null ? null : ToSummary(asset);
    }

    public async Task<Dictionary<Guid, MediaSummary>> GetManyAsync(IEnumerable<Guid> mediaIds, CancellationToken ct)
    {
        var ids = mediaIds.Distinct().ToList();
        var assets = await db.MediaAssets.AsNoTracking().Where(a => ids.Contains(a.Id)).ToListAsync(ct);
        return assets.ToDictionary(a => a.Id, ToSummary);
    }

    public Task<bool> ExistsAsync(Guid mediaId, CancellationToken ct) =>
        db.MediaAssets.AsNoTracking().AnyAsync(a => a.Id == mediaId, ct);

    private MediaSummary ToSummary(Domain.MediaAsset asset) => new(
        asset.Id,
        asset.SourceType == Domain.MediaSourceType.External ? asset.ExternalUrl! : storage.GetPublicUrl(asset.StorageKey!),
        asset.AltText,
        asset.Width,
        asset.Height);
}
