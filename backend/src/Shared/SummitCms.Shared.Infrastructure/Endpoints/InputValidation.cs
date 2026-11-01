using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace SummitCms.Shared.Infrastructure.Endpoints;

/// <summary>
/// Lightweight request validation for hand-written admin endpoints: required fields, maximum lengths
/// and URL-slug format, returned as a 400 with a readable message (instead of a database error).
/// </summary>
public static partial class InputValidation
{
    public readonly record struct Rule(string Field, string? Value, int MaxLength, bool Required = true);

    [GeneratedRegex("^[a-z0-9]+(?:-[a-z0-9]+)*$")]
    private static partial Regex SlugPattern();

    public static bool IsValidSlug(string? slug) => !string.IsNullOrEmpty(slug) && slug.Length <= 120 && SlugPattern().IsMatch(slug);

    /// <returns>A 400 result describing the first problem, or null when every rule passes.</returns>
    public static IResult? Check(params Rule[] rules)
    {
        foreach (var r in rules)
        {
            if (r.Required && string.IsNullOrWhiteSpace(r.Value))
                return Results.BadRequest(new { error = $"{r.Field} is required.", field = r.Field });
            if (r.Value is not null && r.Value.Length > r.MaxLength)
                return Results.BadRequest(new { error = $"{r.Field} must be {r.MaxLength} characters or fewer.", field = r.Field });
        }
        return null;
    }

    public static IResult? CheckSlug(string? slug) =>
        IsValidSlug(slug)
            ? null
            : Results.BadRequest(new { error = "Slug must be lowercase letters, numbers and single hyphens (e.g. scaffold-safety).", field = "Slug" });
}
