using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SummitCms.Modules.Blog.Domain;
using SummitCms.Modules.Blog.Infrastructure;
using SummitCms.Modules.Media.Contracts;

namespace SummitCms.Modules.Blog.Api;

/// <summary>Anonymous, read-only shape for the public /insights index and /insights/{slug} detail page.
/// Only ever returns Published posts - drafts stay invisible to anonymous callers.</summary>
public static class BlogPublicEndpoints
{
    public sealed record PublicPostListItem(string Slug, string Title, string Excerpt, string Category, string AuthorName, DateTimeOffset PublishedAt, bool IsFeatured, string? CoverUrl, string? CoverAlt, int ReadMinutes);
    public sealed record PublicAdjacentPost(string Slug, string Title);
    public sealed record PublicPostDetail(
        string Slug, string Title, string Excerpt, string Body, string Category,
        string AuthorName, string AuthorRole, DateTimeOffset PublishedAt, string? CoverUrl, string? CoverAlt, int ReadMinutes,
        PublicAdjacentPost? Previous, PublicAdjacentPost? Next);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/public/blog").WithTags("Public.Blog");

        group.MapGet("/posts", async (string? category, BlogDbContext db, IMediaCatalog media, CancellationToken ct) =>
        {
            var query = db.Posts.Where(p => p.Status == BlogPostStatus.Published);
            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.Category == category);

            var posts = await query.OrderByDescending(p => p.PublishedAt).ToListAsync(ct);
            var coverIds = posts.Where(p => p.CoverMediaId.HasValue).Select(p => p.CoverMediaId!.Value);
            var coverMap = await media.GetManyAsync(coverIds, ct);

            return Results.Ok(posts.Select(p => new PublicPostListItem(
                p.Slug, p.Title, p.Excerpt, p.Category, p.AuthorName, p.PublishedAt!.Value, p.IsFeatured,
                p.CoverMediaId.HasValue && coverMap.TryGetValue(p.CoverMediaId.Value, out var m) ? m.Url : null,
                p.CoverMediaId.HasValue && coverMap.TryGetValue(p.CoverMediaId.Value, out var m2) ? m2.AltText : null,
                EstimateReadMinutes(p.Body))));
        });

        group.MapGet("/posts/{slug}", async (string slug, BlogDbContext db, IMediaCatalog media, CancellationToken ct) =>
        {
            var all = await db.Posts.Where(p => p.Status == BlogPostStatus.Published)
                .OrderByDescending(p => p.PublishedAt).ToListAsync(ct);

            var index = all.FindIndex(p => p.Slug == slug);
            if (index < 0) return Results.NotFound();

            var post = all[index];
            var previous = index < all.Count - 1 ? all[index + 1] : null; // older post
            var next = index > 0 ? all[index - 1] : null; // newer post

            var cover = post.CoverMediaId.HasValue ? await media.GetAsync(post.CoverMediaId.Value, ct) : null;

            var detail = new PublicPostDetail(
                post.Slug, post.Title, post.Excerpt, post.Body, post.Category, post.AuthorName, post.AuthorRole,
                post.PublishedAt!.Value, cover?.Url, cover?.AltText, EstimateReadMinutes(post.Body),
                previous is null ? null : new PublicAdjacentPost(previous.Slug, previous.Title),
                next is null ? null : new PublicAdjacentPost(next.Slug, next.Title));

            return Results.Ok(detail);
        });
    }

    private static int EstimateReadMinutes(string body)
    {
        var words = body.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, (int)Math.Ceiling(words / 200.0));
    }
}
