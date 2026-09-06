using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using SummitCms.Shared.Infrastructure.Security;
using Xunit;

namespace SummitCms.UnitTests.Shared;

public class PermissionAuthorizationHandlerTests
{
    private readonly PermissionAuthorizationHandler _sut = new();

    private static AuthorizationHandlerContext Context(ClaimsPrincipal user, string permission) =>
        new([new PermissionRequirement(permission)], user, null);

    [Fact]
    public async Task SuperAdmin_role_bypasses_any_permission_check()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, "SuperAdmin")], "test"));
        var context = Context(user, "anything.at.all");

        await _sut.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task A_matching_permission_claim_succeeds()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("permission", "projects.manage")], "test"));
        var context = Context(user, "projects.manage");

        await _sut.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task Missing_the_permission_and_not_SuperAdmin_fails()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, "Editor")], "test"));
        var context = Context(user, "projects.manage");

        await _sut.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }
}
