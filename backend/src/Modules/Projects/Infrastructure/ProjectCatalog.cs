using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Projects.Contracts;

namespace SummitCms.Modules.Projects.Infrastructure;

public sealed class ProjectCatalog(ProjectsDbContext db) : IProjectCatalog
{
    public async Task<ProjectSummary?> GetAsync(Guid projectId, CancellationToken ct)
    {
        var p = await db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId, ct);
        return p is null ? null : new ProjectSummary(p.Id, p.Slug, p.Name, p.Stat);
    }

    public async Task<Dictionary<Guid, ProjectSummary>> GetManyAsync(IEnumerable<Guid> projectIds, CancellationToken ct)
    {
        var ids = projectIds.Distinct().ToList();
        return await db.Projects.AsNoTracking().Where(p => ids.Contains(p.Id))
            .Select(p => new ProjectSummary(p.Id, p.Slug, p.Name, p.Stat))
            .ToDictionaryAsync(p => p.Id, ct);
    }
}
