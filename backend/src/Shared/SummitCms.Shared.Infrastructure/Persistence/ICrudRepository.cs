using Microsoft.EntityFrameworkCore;
using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Shared.Infrastructure.Persistence;

/// <summary>
/// Minimal persistence contract used by the generic admin CRUD endpoint mapper.
/// Registered per (DbContext, Entity) pair by each module for its simple reference-data entities.
/// </summary>
public interface ICrudRepository<TEntity> where TEntity : AuditableEntity
{
    Task<List<TEntity>> ListAsync(CancellationToken ct);
    Task<TEntity?> GetAsync(Guid id, CancellationToken ct);
    Task AddAsync(TEntity entity, CancellationToken ct);
    Task UpdateAsync(CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}

public sealed class EfCrudRepository<TContext, TEntity>(TContext db) : ICrudRepository<TEntity>
    where TContext : DbContext
    where TEntity : AuditableEntity
{
    public async Task<List<TEntity>> ListAsync(CancellationToken ct)
    {
        var items = await db.Set<TEntity>().AsNoTracking().ToListAsync(ct);
        return items is List<IOrderable> ? items : OrderIfOrderable(items);
    }

    private static List<TEntity> OrderIfOrderable(List<TEntity> items)
    {
        if (items.Count == 0 || items[0] is not IOrderable)
            return items;

        return items.Cast<IOrderable>().OrderBy(x => x.DisplayOrder).Cast<TEntity>().ToList();
    }

    public Task<TEntity?> GetAsync(Guid id, CancellationToken ct) =>
        db.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task AddAsync(TEntity entity, CancellationToken ct)
    {
        db.Set<TEntity>().Add(entity);
        await db.SaveChangesAsync(ct);
    }

    public Task UpdateAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await db.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is null) return false;

        db.Set<TEntity>().Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
