using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Industries.Contracts;
using SummitCms.Modules.Industries.Domain;
using SummitCms.Modules.Industries.Infrastructure;
using SummitCms.Modules.Projects.Contracts;
using SummitCms.Shared.Infrastructure.Endpoints;
using SummitCms.Shared.Infrastructure.Security;

namespace SummitCms.Modules.Industries.Api;

public static class IndustryEndpoints
{
    public sealed record IndustryDto(Guid Id, string Idx, string Name, string Tag, string Body, Guid? MediaId, string FigureLabel, int DisplayOrder, bool IsActive);
    public sealed record IndustryWriteDto(string Idx, string Name, string Tag, string Body, Guid? MediaId, string FigureLabel, int DisplayOrder, bool IsActive);
    public sealed record LinkWriteDto(Guid ProjectId, string? CustomLabel, string? CustomStat, int DisplayOrder);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/industries").WithTags("Admin.Industries").RequirePermission(IndustriesPermissions.Manage);

        group.MapAdminCrud<Industry, IndustryWriteDto, IndustryWriteDto, IndustryDto>(
            "", IndustriesPermissions.Manage,
            i => new IndustryDto(i.Id, i.Idx, i.Name, i.Tag, i.Body, i.MediaId, i.FigureLabel, i.DisplayOrder, i.IsActive),
            dto => new Industry { Idx = dto.Idx, Name = dto.Name, Tag = dto.Tag, Body = dto.Body, MediaId = dto.MediaId, FigureLabel = dto.FigureLabel, DisplayOrder = dto.DisplayOrder, IsActive = dto.IsActive },
            (i, dto) => { i.Idx = dto.Idx; i.Name = dto.Name; i.Tag = dto.Tag; i.Body = dto.Body; i.MediaId = dto.MediaId; i.FigureLabel = dto.FigureLabel; i.DisplayOrder = dto.DisplayOrder; i.IsActive = dto.IsActive; });

        group.MapPost("/{industryId:guid}/links", async (Guid industryId, LinkWriteDto body, IndustriesDbContext db, IProjectCatalog projects, CancellationToken ct) =>
        {
            var project = await projects.GetAsync(body.ProjectId, ct);
            if (project is null) return Results.BadRequest(new { error = "Referenced project does not exist." });

            var link = new IndustryProjectLink { IndustryId = industryId, ProjectId = body.ProjectId, CustomLabel = body.CustomLabel, CustomStat = body.CustomStat, DisplayOrder = body.DisplayOrder };
            db.ProjectLinks.Add(link);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/admin/industries/{industryId}/links/{link.Id}", link.Id);
        });

        group.MapDelete("/links/{id:guid}", async (Guid id, IndustriesDbContext db, CancellationToken ct) =>
        {
            var link = await db.ProjectLinks.FindAsync([id], ct);
            if (link is null) return Results.NotFound();
            db.ProjectLinks.Remove(link);
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });
    }
}
