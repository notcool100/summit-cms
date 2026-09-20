using System.Globalization;
using CsvHelper;
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
using SummitCms.Modules.Identity.Contracts;
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
    public sealed record SubmissionStats(int TotalCount, int NewCount, int InReviewCount, int ResolvedCount, int ArchivedCount, int Last7DaysCount);

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

        admin.MapGet("/submissions/stats", async (ContactDbContext db, CancellationToken ct) =>
        {
            var statusCounts = await db.Submissions
                .GroupBy(s => s.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var totalCount = statusCounts.Sum(x => x.Count);
            var newCount = statusCounts.FirstOrDefault(x => x.Status == SubmissionStatus.New)?.Count ?? 0;
            var inReviewCount = statusCounts.FirstOrDefault(x => x.Status == SubmissionStatus.InReview)?.Count ?? 0;
            var resolvedCount = statusCounts.FirstOrDefault(x => x.Status == SubmissionStatus.Resolved)?.Count ?? 0;
            var archivedCount = statusCounts.FirstOrDefault(x => x.Status == SubmissionStatus.Archived)?.Count ?? 0;

            var since = DateTimeOffset.UtcNow.AddDays(-7);
            var last7DaysCount = await db.Submissions.CountAsync(s => s.CreatedAt >= since, ct);

            return Results.Ok(new SubmissionStats(totalCount, newCount, inReviewCount, resolvedCount, archivedCount, last7DaysCount));
        });

        admin.MapGet("/submissions/export", async (ContactDbContext db, IEnquiryTypeCatalog enquiryTypes, IUserCatalog users, CancellationToken ct) =>
        {
            var submissions = await db.Submissions.AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(ct);

            var enquiryTypeLabels = new Dictionary<Guid, string>();
            foreach (var enquiryTypeId in submissions.Select(s => s.EnquiryTypeId).Distinct())
            {
                var summary = await enquiryTypes.GetAsync(enquiryTypeId, ct);
                enquiryTypeLabels[enquiryTypeId] = summary?.Label ?? enquiryTypeId.ToString();
            }

            var assignedUserNames = new Dictionary<Guid, string>();
            foreach (var userId in submissions.Where(s => s.AssignedUserId.HasValue).Select(s => s.AssignedUserId!.Value).Distinct())
            {
                var summary = await users.GetAsync(userId, ct);
                assignedUserNames[userId] = summary is null
                    ? string.Empty
                    : (string.IsNullOrWhiteSpace(summary.FullName) ? summary.Email : summary.FullName);
            }

            var fileName = $"contact-submissions-{DateTime.UtcNow:yyyyMMdd}.csv";

            return Results.Stream(async stream =>
            {
                await using var writer = new StreamWriter(stream, leaveOpen: true);
                await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture, leaveOpen: true);

                csv.WriteField("Name");
                csv.WriteField("Email");
                csv.WriteField("Phone");
                csv.WriteField("Company");
                csv.WriteField("Enquiry Type");
                csv.WriteField("Message");
                csv.WriteField("Status");
                csv.WriteField("Assigned To");
                csv.WriteField("Created At");
                csv.WriteField("Updated At");
                await csv.NextRecordAsync();

                foreach (var s in submissions)
                {
                    csv.WriteField(s.Name);
                    csv.WriteField(s.Email);
                    csv.WriteField(s.Phone ?? string.Empty);
                    csv.WriteField(s.Company ?? string.Empty);
                    csv.WriteField(enquiryTypeLabels.GetValueOrDefault(s.EnquiryTypeId, s.EnquiryTypeId.ToString()));
                    csv.WriteField(s.Message);
                    csv.WriteField(s.Status.ToString());
                    csv.WriteField(s.AssignedUserId.HasValue ? assignedUserNames.GetValueOrDefault(s.AssignedUserId.Value, string.Empty) : string.Empty);
                    csv.WriteField(s.CreatedAt.ToString("O"));
                    csv.WriteField(s.UpdatedAt.ToString("O"));
                    await csv.NextRecordAsync();
                }

                await writer.FlushAsync();
            }, "text/csv", fileName);
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
