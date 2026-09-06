using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SummitCms.Shared.Infrastructure.Auditing;
using SummitCms.Shared.Infrastructure.Persistence;
using SummitCms.Shared.Infrastructure.Security;
using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Shared.Infrastructure.Endpoints;

/// <summary>
/// Wires up GET/GET-by-id/POST/PUT/DELETE for a simple reference-data entity in one call, so the
/// ~20 straightforward content entities (capabilities, milestones, awards, ...) don't each need
/// hand-written MediatR commands/handlers just to move data in and out of a table. Every mutation
/// publishes an <see cref="EntityAuditEvent"/> for the Identity module's audit log.
/// </summary>
public static class CrudEndpointExtensions
{
    public static RouteGroupBuilder MapAdminCrud<TEntity, TCreateDto, TUpdateDto, TReadDto>(
        this RouteGroupBuilder group,
        string routePrefix,
        string permissionCode,
        Func<TEntity, TReadDto> toReadDto,
        Func<TCreateDto, TEntity> fromCreateDto,
        Action<TEntity, TUpdateDto> applyUpdate)
        where TEntity : AuditableEntity
    {
        var entityName = typeof(TEntity).Name;
        var sub = group.MapGroup(routePrefix).RequirePermission(permissionCode);

        sub.MapGet("", async (ICrudRepository<TEntity> repo, CancellationToken ct) =>
            Results.Ok((await repo.ListAsync(ct)).Select(toReadDto)));

        sub.MapGet("/{id:guid}", async (Guid id, ICrudRepository<TEntity> repo, CancellationToken ct) =>
        {
            var entity = await repo.GetAsync(id, ct);
            return entity is null ? Results.NotFound() : Results.Ok(toReadDto(entity));
        });

        sub.MapPost("", async (
            TCreateDto dto, ICrudRepository<TEntity> repo, ICurrentUser user, IPublisher publisher,
            HttpContext http, CancellationToken ct) =>
        {
            var entity = fromCreateDto(dto);
            await repo.AddAsync(entity, ct);
            await PublishAsync(publisher, user, http, "Created", entityName, entity.Id, null, entity);
            return Results.Created($"{routePrefix}/{entity.Id}", toReadDto(entity));
        });

        sub.MapPut("/{id:guid}", async (
            Guid id, TUpdateDto dto, ICrudRepository<TEntity> repo, ICurrentUser user, IPublisher publisher,
            HttpContext http, CancellationToken ct) =>
        {
            var entity = await repo.GetAsync(id, ct);
            if (entity is null) return Results.NotFound();

            var before = JsonSerializer.Serialize(entity);
            applyUpdate(entity, dto);
            await repo.UpdateAsync(ct);
            await PublishAsync(publisher, user, http, "Updated", entityName, entity.Id, before, entity);
            return Results.Ok(toReadDto(entity));
        });

        sub.MapDelete("/{id:guid}", async (
            Guid id, ICrudRepository<TEntity> repo, ICurrentUser user, IPublisher publisher,
            HttpContext http, CancellationToken ct) =>
        {
            var entity = await repo.GetAsync(id, ct);
            if (entity is null) return Results.NotFound();

            var ok = await repo.DeleteAsync(id, ct);
            if (ok)
                await PublishAsync(publisher, user, http, "Deleted", entityName, id, entity, null);
            return ok ? Results.NoContent() : Results.NotFound();
        });

        return sub;
    }

    private static Task PublishAsync(
        IPublisher publisher, ICurrentUser user, HttpContext http,
        string action, string entityName, Guid entityId, object? before, object? after) =>
        publisher.Publish(new EntityAuditEvent(
            user.UserId,
            action,
            entityName,
            entityId.ToString(),
            before is null ? null : JsonSerializer.Serialize(before),
            after is null ? null : JsonSerializer.Serialize(after),
            http.Connection.RemoteIpAddress?.ToString()));
}
