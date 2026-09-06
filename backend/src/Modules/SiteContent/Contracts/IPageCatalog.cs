namespace SummitCms.Modules.SiteContent.Contracts;

/// <summary>Lets other modules (Company, Capabilities, Careers, Projects) resolve a page slug to its Id
/// so their own reference-data rows can hang a real PageId FK off it.</summary>
public interface IPageCatalog
{
    Task<Guid?> GetIdBySlugAsync(string slug, CancellationToken ct);
}
