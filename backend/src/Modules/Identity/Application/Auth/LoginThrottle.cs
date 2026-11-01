using System.Collections.Concurrent;

namespace SummitCms.Modules.Identity.Application.Auth;

/// <summary>
/// Per-account brute-force protection: after <see cref="MaxFailures"/> failed sign-ins for the same
/// email within <see cref="Window"/>, that email is locked for <see cref="Lockout"/>. Tracked by the
/// submitted email whether or not the account exists, so the lockout reveals nothing about which
/// accounts are real. In-memory (single API instance); a restart simply clears the counters.
/// </summary>
public sealed class LoginThrottle(TimeProvider time)
{
    public const int MaxFailures = 5;
    public static readonly TimeSpan Window = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan Lockout = TimeSpan.FromMinutes(15);

    private sealed class Entry
    {
        public readonly List<DateTimeOffset> Failures = [];
        public DateTimeOffset? LockedUntil;
    }

    private readonly ConcurrentDictionary<string, Entry> _entries = new(StringComparer.OrdinalIgnoreCase);

    public bool IsLocked(string email, out TimeSpan remaining)
    {
        remaining = TimeSpan.Zero;
        if (!_entries.TryGetValue(email, out var e)) return false;
        lock (e)
        {
            var now = time.GetUtcNow();
            if (e.LockedUntil is { } until && until > now)
            {
                remaining = until - now;
                return true;
            }
            return false;
        }
    }

    public void RecordFailure(string email)
    {
        var e = _entries.GetOrAdd(email, _ => new Entry());
        lock (e)
        {
            var now = time.GetUtcNow();
            e.Failures.RemoveAll(t => now - t > Window);
            e.Failures.Add(now);
            if (e.Failures.Count >= MaxFailures)
            {
                e.LockedUntil = now + Lockout;
                e.Failures.Clear();
            }
        }
    }

    public void RecordSuccess(string email) => _entries.TryRemove(email, out _);
}
