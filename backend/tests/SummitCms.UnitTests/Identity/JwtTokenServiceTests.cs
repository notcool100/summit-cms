using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Options;
using SummitCms.Modules.Identity.Application;
using SummitCms.Modules.Identity.Domain;
using SummitCms.Modules.Identity.Infrastructure;
using Xunit;

namespace SummitCms.UnitTests.Identity;

public class JwtTokenServiceTests
{
    private readonly JwtTokenService _sut = new(
        Options.Create(new JwtOptions { Issuer = "TestIssuer", Audience = "TestAudience", SigningKey = "unit-test-signing-key-at-least-32-bytes-long", AccessTokenMinutes = 5 }),
        TimeProvider.System);

    [Fact]
    public void CreateAccessToken_embeds_role_and_permission_claims()
    {
        var user = new User { Email = "test@example.com", FirstName = "Test", LastName = "User" };

        var result = _sut.CreateAccessToken(user, ["Admin"], ["projects.manage", "media.manage"]);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
        token.Issuer.Should().Be("TestIssuer");
        token.Audiences.Should().Contain("TestAudience");
        token.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Admin");
        token.Claims.Should().Contain(c => c.Type == "permission" && c.Value == "projects.manage");
        token.Claims.Should().Contain(c => c.Type == "permission" && c.Value == "media.manage");
    }

    [Fact]
    public void GenerateRefreshToken_and_HashToken_are_deterministic_and_one_way()
    {
        var raw = _sut.GenerateRefreshToken();
        var hash1 = _sut.HashToken(raw);
        var hash2 = _sut.HashToken(raw);

        hash1.Should().Be(hash2); // hashing the same raw token twice gives the same hash (needed to look it up)
        hash1.Should().NotBe(raw);
    }
}
