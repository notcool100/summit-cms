using FluentAssertions;
using SummitCms.Modules.Identity.Infrastructure;
using Xunit;

namespace SummitCms.UnitTests.Identity;

public class PasswordHasherServiceTests
{
    private readonly PasswordHasherService _sut = new();

    [Fact]
    public void Hash_produces_a_value_different_from_the_input_and_from_itself_each_time()
    {
        var hash1 = _sut.Hash("Str0ng-Passw0rd!");
        var hash2 = _sut.Hash("Str0ng-Passw0rd!");

        hash1.Should().NotBe("Str0ng-Passw0rd!");
        hash1.Should().NotBe(hash2); // salted - two hashes of the same password must differ
    }

    [Fact]
    public void Verify_succeeds_for_the_correct_password_and_fails_for_a_wrong_one()
    {
        var hash = _sut.Hash("Str0ng-Passw0rd!");

        _sut.Verify(hash, "Str0ng-Passw0rd!").Should().BeTrue();
        _sut.Verify(hash, "wrong-password").Should().BeFalse();
    }
}
