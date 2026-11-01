using System.Text;
using FluentAssertions;
using SummitCms.Modules.Media.Application;
using Xunit;

namespace SummitCms.UnitTests.Media;

/// <summary>QA-02 / QA-12: uploads are identified by content, and only safe types/URLs are accepted.</summary>
public class MediaFileInspectorTests
{
    private static byte[] B(params int[] bytes) => bytes.Select(b => (byte)b).ToArray();

    [Fact]
    public void Detects_jpeg() => MediaFileInspector.Detect(B(0xFF, 0xD8, 0xFF, 0xE0, 0, 0)).Should().Be(new MediaFileInspector.DetectedType(".jpg", "image/jpeg"));

    [Fact]
    public void Detects_png() => MediaFileInspector.Detect(B(0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0)).Should().Be(new MediaFileInspector.DetectedType(".png", "image/png"));

    [Fact]
    public void Detects_gif() => MediaFileInspector.Detect(Encoding.ASCII.GetBytes("GIF89a....")).Should().Be(new MediaFileInspector.DetectedType(".gif", "image/gif"));

    [Fact]
    public void Detects_webp() => MediaFileInspector.Detect(Encoding.ASCII.GetBytes("RIFF\0\0\0\0WEBPVP8 ")).Should().Be(new MediaFileInspector.DetectedType(".webp", "image/webp"));

    [Fact]
    public void Detects_pdf() => MediaFileInspector.Detect(Encoding.ASCII.GetBytes("%PDF-1.7\n")).Should().Be(new MediaFileInspector.DetectedType(".pdf", "application/pdf"));

    [Theory]
    [InlineData("<html><script>alert(1)</script></html>")]
    [InlineData("<?xml version=\"1.0\"?><svg xmlns=\"http://www.w3.org/2000/svg\"><script/></svg>")]
    [InlineData("<svg onload=alert(1)>")]
    [InlineData("MZ\u0090\0 executable")]
    [InlineData("#!/bin/sh\nrm -rf /")]
    [InlineData("")]
    public void Rejects_anything_that_is_not_an_allowed_image_or_pdf(string content) =>
        MediaFileInspector.Detect(Encoding.Latin1.GetBytes(content)).Should().BeNull();

    [Fact]
    public async Task DetectAsync_rewinds_the_stream_so_the_whole_file_is_stored()
    {
        var bytes = B(0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        using var stream = new MemoryStream(bytes);
        (await MediaFileInspector.DetectAsync(stream, CancellationToken.None)).Should().NotBeNull();
        stream.Position.Should().Be(0);
    }

    [Theory]
    [InlineData("https://images.example.com/photo.jpg", true)]
    [InlineData("http://images.example.com/photo.jpg", false)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("data:image/png;base64,AAAA", false)]
    [InlineData("//images.example.com/photo.jpg", false)]
    [InlineData("not a url", false)]
    [InlineData(null, false)]
    public void External_urls_must_be_absolute_https(string? url, bool allowed) =>
        MediaFileInspector.IsAllowedExternalUrl(url).Should().Be(allowed);

    [Theory]
    [InlineData(".svg")]
    [InlineData(".html")]
    [InlineData(".js")]
    [InlineData(".exe")]
    public void Upload_folder_never_serves_active_content_types(string extension) =>
        MediaFileInspector.ServableTypes.Should().NotContainKey(extension);
}
