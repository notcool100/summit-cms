using FluentAssertions;
using SummitCms.Modules.Identity.Application.Auth;
using Xunit;

namespace SummitCms.UnitTests.Identity;

/// <summary>QA-04: per-account lockout after repeated failed sign-ins.</summary>
public class LoginThrottleTests
{
    private readonly LoginThrottle _sut = new(TimeProvider.System);

    [Fact]
    public void Four_failures_do_not_lock_the_account()
    {
        for (var i = 0; i < LoginThrottle.MaxFailures - 1; i++) _sut.RecordFailure("a@example.com");
        _sut.IsLocked("a@example.com", out _).Should().BeFalse();
    }

    [Fact]
    public void Fifth_failure_locks_the_account_for_about_15_minutes()
    {
        for (var i = 0; i < LoginThrottle.MaxFailures; i++) _sut.RecordFailure("a@example.com");
        _sut.IsLocked("a@example.com", out var remaining).Should().BeTrue();
        remaining.Should().BeCloseTo(LoginThrottle.Lockout, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Lockout_is_per_account_and_case_insensitive()
    {
        for (var i = 0; i < LoginThrottle.MaxFailures; i++) _sut.RecordFailure("Victim@Example.com");
        _sut.IsLocked("victim@example.com", out _).Should().BeTrue();
        _sut.IsLocked("someone-else@example.com", out _).Should().BeFalse();
    }

    [Fact]
    public void Successful_sign_in_clears_earlier_failures()
    {
        for (var i = 0; i < LoginThrottle.MaxFailures - 1; i++) _sut.RecordFailure("a@example.com");
        _sut.RecordSuccess("a@example.com");
        _sut.RecordFailure("a@example.com");
        _sut.IsLocked("a@example.com", out _).Should().BeFalse();
    }
}
