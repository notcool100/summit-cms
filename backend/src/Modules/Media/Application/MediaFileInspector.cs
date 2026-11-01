namespace SummitCms.Modules.Media.Application;

/// <summary>
/// Decides whether an uploaded file is an allowed media type by reading its leading bytes
/// ("magic numbers") - never trusting the client's file name or declared Content-Type, which an
/// attacker controls. Anything that isn't a raster image or PDF (HTML, SVG, scripts, executables)
/// is rejected, so uploads can never be served back as active content on the site's domain.
/// </summary>
public static class MediaFileInspector
{
    public const long MaxBytes = 15 * 1024 * 1024;

    public sealed record DetectedType(string Extension, string ContentType);

    /// <summary>Extensions the /uploads static file handler is allowed to serve, with their MIME types.</summary>
    public static readonly IReadOnlyDictionary<string, string> ServableTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".png"] = "image/png",
        [".gif"] = "image/gif",
        [".webp"] = "image/webp",
        [".avif"] = "image/avif",
        [".pdf"] = "application/pdf"
    };

    public static async Task<DetectedType?> DetectAsync(Stream stream, CancellationToken ct)
    {
        var header = new byte[16];
        var read = 0;
        while (read < header.Length)
        {
            var n = await stream.ReadAsync(header.AsMemory(read, header.Length - read), ct);
            if (n == 0) break;
            read += n;
        }
        if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
        return Detect(header.AsSpan(0, read));
    }

    public static DetectedType? Detect(ReadOnlySpan<byte> h)
    {
        if (h.Length >= 3 && h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF)
            return new(".jpg", "image/jpeg");
        if (h.Length >= 8 && h[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
            return new(".png", "image/png");
        if (h.Length >= 6 && (h[..6].SequenceEqual("GIF87a"u8) || h[..6].SequenceEqual("GIF89a"u8)))
            return new(".gif", "image/gif");
        if (h.Length >= 12 && h[..4].SequenceEqual("RIFF"u8) && h[8..12].SequenceEqual("WEBP"u8))
            return new(".webp", "image/webp");
        if (h.Length >= 12 && h[4..8].SequenceEqual("ftyp"u8) && (h[8..12].SequenceEqual("avif"u8) || h[8..12].SequenceEqual("avis"u8)))
            return new(".avif", "image/avif");
        if (h.Length >= 5 && h[..5].SequenceEqual("%PDF-"u8))
            return new(".pdf", "application/pdf");
        return null;
    }

    /// <summary>Only absolute https URLs may be registered as external media (no javascript:, data:, http:).</summary>
    public static bool IsAllowedExternalUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps && !string.IsNullOrEmpty(uri.Host);
}
