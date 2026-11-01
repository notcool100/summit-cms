using FluentAssertions;
using SummitCms.Shared.Infrastructure.Endpoints;
using Xunit;

namespace SummitCms.UnitTests.Shared;

/// <summary>QA-10: admin input is validated before it reaches the database.</summary>
public class InputValidationTests
{
    [Theory]
    [InlineData("scaffold-safety", true)]
    [InlineData("representative-rigging-lifting", true)]
    [InlineData("post2026", true)]
    [InlineData("Bad Slug", false)]
    [InlineData("double--hyphen", false)]
    [InlineData("-leading", false)]
    [InlineData("trailing-", false)]
    [InlineData("UPPER", false)]
    [InlineData("../etc/passwd", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void Slugs_are_lowercase_words_joined_by_single_hyphens(string? slug, bool valid) =>
        InputValidation.IsValidSlug(slug).Should().Be(valid);

    [Fact]
    public void Missing_required_field_fails() =>
        InputValidation.Check(new InputValidation.Rule("Title", "  ", 100)).Should().NotBeNull();

    [Fact]
    public void Over_long_field_fails() =>
        InputValidation.Check(new InputValidation.Rule("Title", new string('x', 101), 100)).Should().NotBeNull();

    [Fact]
    public void Optional_empty_field_passes() =>
        InputValidation.Check(new InputValidation.Rule("Excerpt", null, 100, Required: false)).Should().BeNull();

    [Fact]
    public void Valid_fields_pass() =>
        InputValidation.Check(new("Title", "Choosing a scaffold", 100), new("Body", "Text", 1000)).Should().BeNull();
}
