using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Media.Application;
using SummitCms.Modules.Media.Contracts;
using SummitCms.Modules.Media.Domain;
using SummitCms.Modules.Media.Infrastructure;
using SummitCms.Shared.Infrastructure.Auditing;
using SummitCms.Shared.Infrastructure.Security;

namespace SummitCms.Modules.Media.Api;

public static class MediaEndpoints
{
    public sealed record MediaItem(Guid Id, string Url, string FileName, string AltText, string SourceType, int? Width, int? Height, long SizeBytes, DateTimeOffset CreatedAt);
    public sealed record CreateExternalRequest(string ExternalUrl, string FileName, string AltText);
    public sealed record UpdateAltTextRequest(string AltText);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/media").WithTags("Admin.Media").RequirePermission(MediaPermissions.Manage);

        group.MapGet("", async (MediaDbContext db, IFileStorageService storage, CancellationToken ct) =>
        {
            var assets = await db.MediaAssets.AsNoTracking().OrderByDescending(a => a.CreatedAt).ToListAsync(ct);
            return Results.Ok(assets.Select(a => ToItem(a, storage)));
        });

        group.MapPost("/upload", async (
            HttpRequest request, MediaDbContext db, IFileStorageService storage, ICurrentUser user,
            IPublisher publisher, CancellationToken ct) =>
        {
            if (!request.HasFormContentType) return Results.BadRequest(new { error = "Expected multipart/form-data." });

            var form = await request.ReadFormAsync(ct);
            var file = form.Files.GetFile("file");
            if (file is null || file.Length == 0) return Results.BadRequest(new { error = "No file provided." });
            if (file.Length > MediaFileInspector.MaxBytes)
                return Results.BadRequest(new { error = "File is too large. The maximum size is 15 MB." });

            await using var stream = file.OpenReadStream();
            // Identify the file by its contents, not its name/declared type, and store it under the
            // matching extension - so e.g. an HTML file renamed .png can never be served as a page.
            var detected = await MediaFileInspector.DetectAsync(stream, ct);
            if (detected is null)
                return Results.BadRequest(new { error = "Unsupported file type. Upload a JPEG, PNG, GIF, WebP, AVIF image or a PDF." });

            var baseName = Path.GetFileNameWithoutExtension(file.FileName);
            var safeName = (string.IsNullOrWhiteSpace(baseName) ? "upload" : baseName) + detected.Extension;
            var stored = await storage.SaveAsync(stream, safeName, detected.ContentType, ct);

            var asset = new MediaAsset
            {
                FileName = safeName,
                ContentType = detected.ContentType,
                SizeBytes = file.Length,
                AltText = form["altText"].ToString(),
                SourceType = MediaSourceType.Uploaded,
                StorageKey = stored.StorageKey,
                UploadedByUserId = user.UserId
            };
            db.MediaAssets.Add(asset);
            await db.SaveChangesAsync(ct);

            await publisher.Publish(new EntityAuditEvent(user.UserId, "Created", nameof(MediaAsset), asset.Id.ToString(), null, asset.FileName, request.HttpContext.Connection.RemoteIpAddress?.ToString()), ct);

            return Results.Created($"/api/admin/media/{asset.Id}", ToItem(asset, storage));
        }).DisableAntiforgery();

        group.MapPost("/external", async (
            CreateExternalRequest body, MediaDbContext db, IFileStorageService storage, ICurrentUser user,
            IPublisher publisher, HttpContext http, CancellationToken ct) =>
        {
            if (!MediaFileInspector.IsAllowedExternalUrl(body.ExternalUrl))
                return Results.BadRequest(new { error = "External images must use a full https:// URL." });
            if (string.IsNullOrWhiteSpace(body.FileName))
                return Results.BadRequest(new { error = "A name is required." });

            var asset = new MediaAsset
            {
                FileName = body.FileName,
                ContentType = "external/url",
                AltText = body.AltText,
                SourceType = MediaSourceType.External,
                ExternalUrl = body.ExternalUrl,
                UploadedByUserId = user.UserId
            };
            db.MediaAssets.Add(asset);
            await db.SaveChangesAsync(ct);

            await publisher.Publish(new EntityAuditEvent(user.UserId, "Created", nameof(MediaAsset), asset.Id.ToString(), null, asset.FileName, http.Connection.RemoteIpAddress?.ToString()), ct);

            return Results.Created($"/api/admin/media/{asset.Id}", ToItem(asset, storage));
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateAltTextRequest body, MediaDbContext db, CancellationToken ct) =>
        {
            var asset = await db.MediaAssets.FindAsync([id], ct);
            if (asset is null) return Results.NotFound();

            asset.AltText = body.AltText;
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        }).WithAudit("MediaAssetAltText");

        group.MapDelete("/{id:guid}", async (
            Guid id, MediaDbContext db, IFileStorageService storage, ICurrentUser user, IPublisher publisher,
            HttpContext http, CancellationToken ct) =>
        {
            var asset = await db.MediaAssets.FindAsync([id], ct);
            if (asset is null) return Results.NotFound();

            if (asset.SourceType == MediaSourceType.Uploaded && asset.StorageKey is not null)
                await storage.DeleteAsync(asset.StorageKey, ct);

            db.MediaAssets.Remove(asset);
            await db.SaveChangesAsync(ct);

            await publisher.Publish(new EntityAuditEvent(user.UserId, "Deleted", nameof(MediaAsset), id.ToString(), asset.FileName, null, http.Connection.RemoteIpAddress?.ToString()), ct);
            return Results.NoContent();
        });
    }

    private static MediaItem ToItem(MediaAsset a, IFileStorageService storage) => new(
        a.Id,
        a.SourceType == MediaSourceType.External ? a.ExternalUrl! : storage.GetPublicUrl(a.StorageKey!),
        a.FileName, a.AltText, a.SourceType.ToString(), a.Width, a.Height, a.SizeBytes, a.CreatedAt);
}
