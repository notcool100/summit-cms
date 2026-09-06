namespace SummitCms.Modules.Projects.Contracts;

public sealed record ProjectSummary(Guid Id, string Slug, string Name, string Stat);

/// <summary>The only surface the Industries module may depend on to resolve its project links.</summary>
public interface IProjectCatalog
{
    Task<ProjectSummary?> GetAsync(Guid projectId, CancellationToken ct);
    Task<Dictionary<Guid, ProjectSummary>> GetManyAsync(IEnumerable<Guid> projectIds, CancellationToken ct);
}
