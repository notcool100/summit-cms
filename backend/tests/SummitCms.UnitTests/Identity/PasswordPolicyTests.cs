using FluentAssertions;
using SummitCms.Modules.Identity.Application.Security;
using Xunit;

namespace SummitCms.UnitTests.Identity;

/// <summary>QA-05: admin passwords must be long, not common and not built from the email name.</summary>
public class PasswordPolicyTests
{
    [Theory]
    [InlineData("a")]
    [InlineData("short")]
    [InlineData("elevenchars")]
    public void Rejects_passwords_shorter_than_12_characters(string password) =>
        PasswordPolicy.Validate(password).Should().Contain("at least 12");

    [Theory]
    [InlineData("password1234")]
    [InlineData("Password1234")]
    [InlineData("qwerty123456")]
    public void Rejects_common_passwords_regardless_of_case(string password) =>
        PasswordPolicy.Validate(password).Should().Contain("too common");

    [Fact]
    public void Rejects_passwords_with_almost_no_variety() =>
        PasswordPolicy.Validate("aaaaaaaaaaaaab").Should().Contain("too simple");

    [Fact]
    public void Rejects_passwords_containing_the_email_name() =>
        PasswordPolicy.Validate("jason-Pilbara-2026", "jason@summit-is.com.au").Should().Contain("email name");

    [Fact]
    public void Rejects_passwords_longer_than_128_characters() =>
        PasswordPolicy.Validate(new string('x', 60) + new string('y', 60) + "abcdefghij").Should().Contain("at most 128");

    [Theory]
    [InlineData("correct-horse-battery")]
    [InlineData("Karratha!Scaffold7")]
    [InlineData("QaTest!ABCDEFGHIJKLMNOP")]
    public void Accepts_long_varied_passwords(string password) =>
        PasswordPolicy.Validate(password, "someone@example.com").Should().BeNull();
}
