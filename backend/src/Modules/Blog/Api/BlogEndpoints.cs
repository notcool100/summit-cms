using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Blog.Contracts;
using SummitCms.Modules.Blog.Domain;
using SummitCms.Modules.Blog.Infrastructure;
using SummitCms.Shared.Infrastructure.Auditing;
using SummitCms.Shared.Infrastructure.Security;

namespace SummitCms.Modules.Blog.Api;

public static class BlogEndpoints
{
    public sealed record BlogPostListItem(Guid Id, string Slug, string Title, string Category, string Status, DateTimeOffset? PublishedAt, bool IsFeatured, Guid? CoverMediaId);
    public sealed record BlogPostDetail(
        Guid Id, string Slug, string Title, string Excerpt, string Body, string Category,
        string AuthorName, string AuthorRole, Guid? CoverMediaId, string Status, DateTimeOffset? PublishedAt, bool IsFeatured);
    public sealed record BlogPostWriteDto(
        string Slug, string Title, string Excerpt, string Body, string Category,
        string AuthorName, string AuthorRole, Guid? CoverMediaId, BlogPostStatus Status, bool IsFeatured);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/blog/posts").WithTags("Admin.Blog").RequirePermission(BlogPermissions.Manage);

        group.MapGet("", async (BlogDbContext db, CancellationToken ct) =>
            Results.Ok(await db.Posts.OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
                .Select(p => new BlogPostListItem(p.Id, p.Slug, p.Title, p.Category, p.Status.ToString(), p.PublishedAt, p.IsFeatured, p.CoverMediaId))
                .ToListAsync(ct)));

        group.MapGet("/{id:guid}", async (Guid id, BlogDbContext db, CancellationToken ct) =>
        {
            var p = await db.Posts.FindAsync([id], ct);
            return p is null ? Results.NotFound() : Results.Ok(ToDetail(p));
        });

        group.MapPost("", async (BlogPostWriteDto body, BlogDbContext db, ICurrentUser user, IPublisher publisher, HttpContext http, CancellationToken ct) =>
        {
            if (await db.Posts.AnyAsync(x => x.Slug == body.Slug, ct))
                return Results.Conflict(new { error = "A post with this slug already exists." });

            var post = FromDto(new BlogPost(), body);
            db.Posts.Add(post);
            await db.SaveChangesAsync(ct);
            await Audit(publisher, user, http, "Created", post.Id, null, post.Title);
            return Results.Created($"/api/admin/blog/posts/{post.Id}", ToDetail(post));
        });

        group.MapPut("/{id:guid}", async (Guid id, BlogPostWriteDto body, BlogDbContext db, ICurrentUser user, IPublisher publisher, HttpContext http, CancellationToken ct) =>
        {
            var post = await db.Posts.FindAsync([id], ct);
            if (post is null) return Results.NotFound();

            if (await db.Posts.AnyAsync(x => x.Slug == body.Slug && x.Id != id, ct))
                return Results.Conflict(new { error = "A post with this slug already exists." });

            FromDto(post, body);
            await db.SaveChangesAsync(ct);
            await Audit(publisher, user, http, "Updated", id, null, post.Title);
            return Results.Ok(ToDetail(post));
        });

        group.MapDelete("/{id:guid}", async (Guid id, BlogDbContext db, ICurrentUser user, IPublisher publisher, HttpContext http, CancellationToken ct) =>
        {
            var post = await db.Posts.FindAsync([id], ct);
            if (post is null) return Results.NotFound();

            db.Posts.Remove(post);
            await db.SaveChangesAsync(ct);
            await Audit(publisher, user, http, "Deleted", id, post.Title, null);
            return Results.NoContent();
        });
    }

    private static BlogPost FromDto(BlogPost post, BlogPostWriteDto dto)
    {
        var wasPublished = post.Status == BlogPostStatus.Published;
        post.Slug = dto.Slug;
        post.Title = dto.Title;
        post.Excerpt = dto.Excerpt;
        post.Body = dto.Body;
        post.Category = dto.Category;
        post.AuthorName = dto.AuthorName;
        post.AuthorRole = dto.AuthorRole;
        post.CoverMediaId = dto.CoverMediaId;
        post.Status = dto.Status;
        post.IsFeatured = dto.IsFeatured;

        // Stamp PublishedAt the moment a post first transitions into Published, so the public
        // sort order reflects when it actually went live rather than when it was last edited.
        if (dto.Status == BlogPostStatus.Published && !wasPublished)
            post.PublishedAt = DateTimeOffset.UtcNow;
        else if (dto.Status == BlogPostStatus.Draft)
            post.PublishedAt = null;

        return post;
    }

    private static BlogPostDetail ToDetail(BlogPost p) => new(
        p.Id, p.Slug, p.Title, p.Excerpt, p.Body, p.Category, p.AuthorName, p.AuthorRole,
        p.CoverMediaId, p.Status.ToString(), p.PublishedAt, p.IsFeatured);

    private static Task Audit(IPublisher publisher, ICurrentUser user, HttpContext http, string action, Guid id, string? before, string? after) =>
        publisher.Publish(new EntityAuditEvent(user.UserId, action, nameof(BlogPost), id.ToString(), before, after, http.Connection.RemoteIpAddress?.ToString()));
}
