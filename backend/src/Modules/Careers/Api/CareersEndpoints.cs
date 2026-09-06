using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SummitCms.Modules.Careers.Contracts;
using SummitCms.Modules.Careers.Domain;
using SummitCms.Modules.Careers.Infrastructure;
using SummitCms.Shared.Infrastructure.Endpoints;
using SummitCms.Shared.Infrastructure.Security;

namespace SummitCms.Modules.Careers.Api;

public static class CareersEndpoints
{
    public sealed record TrackDto(Guid Id, Guid PageId, string Title, string PathLabel, string Body, Guid? MediaId, string CtaLabel, int DisplayOrder);
    public sealed record TrackWriteDto(Guid PageId, string Title, string PathLabel, string Body, Guid? MediaId, string CtaLabel, int DisplayOrder);
    public sealed record TagWriteDto(Guid JobTrackId, string Tag, int DisplayOrder);

    public sealed record OpeningDto(Guid Id, string Title, string Department, string Location, string EmploymentType, string TrackType, string Description, string ApplyContact, bool IsActive, DateTimeOffset PostedAt, DateTimeOffset? ClosesAt, int DisplayOrder);
    public sealed record OpeningWriteDto(string Title, string Department, string Location, EmploymentType EmploymentType, CareerTrackType TrackType, string Description, string ApplyContact, bool IsActive, DateTimeOffset PostedAt, DateTimeOffset? ClosesAt, int DisplayOrder);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/careers").WithTags("Admin.Careers").RequirePermission(CareersPermissions.Manage);

        group.MapAdminCrud<JobTrack, TrackWriteDto, TrackWriteDto, TrackDto>(
            "/tracks", CareersPermissions.Manage,
            t => new TrackDto(t.Id, t.PageId, t.Title, t.PathLabel, t.Body, t.MediaId, t.CtaLabel, t.DisplayOrder),
            dto => new JobTrack { PageId = dto.PageId, Title = dto.Title, PathLabel = dto.PathLabel, Body = dto.Body, MediaId = dto.MediaId, CtaLabel = dto.CtaLabel, DisplayOrder = dto.DisplayOrder },
            (t, dto) => { t.Title = dto.Title; t.PathLabel = dto.PathLabel; t.Body = dto.Body; t.MediaId = dto.MediaId; t.CtaLabel = dto.CtaLabel; t.DisplayOrder = dto.DisplayOrder; });

        group.MapPost("/tracks/{trackId:guid}/tags", async (Guid trackId, TagWriteDto body, CareersDbContext db, CancellationToken ct) =>
        {
            var tag = new JobTrackTag { JobTrackId = trackId, Tag = body.Tag, DisplayOrder = body.DisplayOrder };
            db.JobTrackTags.Add(tag);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/admin/careers/tracks/{trackId}/tags/{tag.Id}", tag.Id);
        });
        group.MapDelete("/tracks/tags/{id:guid}", async (Guid id, CareersDbContext db, CancellationToken ct) =>
        {
            var tag = await db.JobTrackTags.FindAsync([id], ct);
            if (tag is null) return Results.NotFound();
            db.JobTrackTags.Remove(tag);
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        group.MapAdminCrud<JobOpening, OpeningWriteDto, OpeningWriteDto, OpeningDto>(
            "/openings", CareersPermissions.Manage,
            o => new OpeningDto(o.Id, o.Title, o.Department, o.Location, o.EmploymentType.ToString(), o.TrackType.ToString(), o.Description, o.ApplyContact, o.IsActive, o.PostedAt, o.ClosesAt, o.DisplayOrder),
            dto => new JobOpening { Title = dto.Title, Department = dto.Department, Location = dto.Location, EmploymentType = dto.EmploymentType, TrackType = dto.TrackType, Description = dto.Description, ApplyContact = dto.ApplyContact, IsActive = dto.IsActive, PostedAt = dto.PostedAt, ClosesAt = dto.ClosesAt, DisplayOrder = dto.DisplayOrder },
            (o, dto) => { o.Title = dto.Title; o.Department = dto.Department; o.Location = dto.Location; o.EmploymentType = dto.EmploymentType; o.TrackType = dto.TrackType; o.Description = dto.Description; o.ApplyContact = dto.ApplyContact; o.IsActive = dto.IsActive; o.PostedAt = dto.PostedAt; o.ClosesAt = dto.ClosesAt; o.DisplayOrder = dto.DisplayOrder; });
    }
}
