namespace SummitCms.Modules.Media.Contracts;

public sealed record MediaSummary(Guid Id, string Url, string AltText, int? Width, int? Height);

/// <summary>The only surface other modules may depend on to resolve a MediaId into a displayable URL.</summary>
public interface IMediaCatalog
{
    Task<MediaSummary?> GetAsync(Guid mediaId, CancellationToken ct);
    Task<Dictionary<Guid, MediaSummary>> GetManyAsync(IEnumerable<Guid> mediaIds, CancellationToken ct);
    Task<bool> ExistsAsync(Guid mediaId, CancellationToken ct);
}
