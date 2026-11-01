using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace SummitCms.Api.Infrastructure;

/// <summary>
/// Last-resort error handling. Bad input must come back as a 400 with a readable message, never as
/// a 500 with a stack trace:
/// <list type="bullet">
/// <item>Malformed or incomplete request bodies (BadHttpRequestException) -> 400.</item>
/// <item>Database constraint failures (required field missing, duplicate key, value too long) -> 400/409.</item>
/// <item>Anything else -> a generic 500 that leaks no internals; details go to the log only.</item>
/// </list>
/// </summary>
public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception exception, CancellationToken ct)
    {
        var (status, message) = exception switch
        {
            BadHttpRequestException bad => (bad.StatusCode, "The request was malformed or missing required fields."),
            DbUpdateException db when IsUniqueViolation(db) => (StatusCodes.Status409Conflict, "A record with the same unique value already exists."),
            DbUpdateException => (StatusCodes.Status400BadRequest, "The request is missing required information or contains a value that is too long."),
            OperationCanceledException when ct.IsCancellationRequested => (499, "Request cancelled."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again.")
        };

        if (status >= 500)
            logger.LogError(exception, "Unhandled exception for {Method} {Path}", http.Request.Method, http.Request.Path);
        else
            logger.LogWarning("Rejected {Method} {Path}: {Message}", http.Request.Method, http.Request.Path, exception.GetBaseException().Message);

        if (http.Response.HasStarted) return false;
        http.Response.StatusCode = status;
        await http.Response.WriteAsJsonAsync(new { error = message }, ct);
        return true;
    }

    // Npgsql reports unique-constraint violations with SQLSTATE 23505.
    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.GetBaseException() is Npgsql.PostgresException { SqlState: "23505" };
}
