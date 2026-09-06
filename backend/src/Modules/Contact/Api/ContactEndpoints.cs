using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Contact.Application;
using SummitCms.Modules.Contact.Contracts;
using SummitCms.Modules.Contact.Domain;
using SummitCms.Modules.Contact.Infrastructure;
using SummitCms.Modules.SiteContent.Contracts;
using SummitCms.Shared.Infrastructure.Security;
using SummitCms.Shared.Kernel.Pagination;

namespace SummitCms.Modules.Contact.Api;

public static class ContactEndpoints
{
    public sealed record SubmitRequest(string Name, string Email, string? Phone, string? Company, Guid EnquiryTypeId, string Message);
    public sealed record SubmissionItem(Guid Id, string Name, string Email, string? Phone, string? Company, Guid EnquiryTypeId, string Message, string Status, Guid? AssignedUserId, DateTimeOffset CreatedAt);
    public sealed record SetStatusRequest(SubmissionStatus Status);
    public sealed record AssignRequest(Guid? UserId);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/public/enquiry-types", async (IEnquiryTypeCatalog catalog, CancellationToken ct) =>
            Results.Ok(await catalog.ListActiveAsync(ct)))
            .WithTags("Public.Contact");

        app.MapPost("/api/public/contact", async (SubmitRequest body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new CreateSubmissionCommand(body.Name, body.Email, body.Phone, body.Company, body.EnquiryTypeId, body.Message), ct);
            return result.IsSuccess ? Results.Created($"/api/admin/contact/submissions/{result.Value}", new { id = result.Value }) : Results.BadRequest(new { error = result.Error });
        })
        .RequireRateLimiting("contact")
        .WithTags("Public.Contact");

        var admin = app.MapGroup("/api/admin/contact").WithTags("Admin.Contact").RequirePermission(ContactPermissions.Manage);

        admin.MapGet("/submissions", async (int page, int pageSize, ContactDbContext db, CancellationToken ct) =>
        {
            var request = new PageRequest(page <= 0 ? 1 : page, pageSize <= 0 ? 25 : pageSize);
            var query = db.Submissions.OrderByDescending(s => s.CreatedAt);
            var total = await query.CountAsync(ct);
            var items = await query.Skip(request.Skip).Take(request.NormalizedPageSize)
                .Select(s => new SubmissionItem(s.Id, s.Name, s.Email, s.Phone, s.Company, s.EnquiryTypeId, s.Message, s.Status.ToString(), s.AssignedUserId, s.CreatedAt))
                .ToListAsync(ct);
            return Results.Ok(PagedResult<SubmissionItem>.Create(items, total, request));
        });

        admin.MapPut("/submissions/{id:guid}/status", async (Guid id, SetStatusRequest body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SetSubmissionStatusCommand(id, body.Status), ct);
            return result.IsSuccess ? Results.NoContent() : Results.NotFound(new { error = result.Error });
        });

        admin.MapPut("/submissions/{id:guid}/assign", async (Guid id, AssignRequest body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new AssignSubmissionCommand(id, body.UserId), ct);
            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(new { error = result.Error });
        });
    }
}
