namespace SummitCms.Modules.Identity.Application.Security;

/// <summary>
/// Admin password rules (NIST SP 800-63B style): length matters more than character classes, so
/// require 12+ characters, reject well-known weak passwords, and reject passwords built from the
/// account's own email name.
/// </summary>
public static class PasswordPolicy
{
    public const int MinLength = 12;
    public const int MaxLength = 128;

    private static readonly HashSet<string> Common = new(StringComparer.OrdinalIgnoreCase)
    {
        "password1234", "password12345", "password123456", "123456789012", "1234567890123",
        "qwertyuiop12", "qwerty123456", "letmein12345", "welcome12345", "iloveyou1234",
        "administrator", "admin1234567", "changeme1234", "summit123456", "summitsummit",
        "passwordpassword", "aaaaaaaaaaaa", "111111111111", "abc123abc123", "p@ssw0rd1234"
    };

    /// <returns>An error message, or null when the password is acceptable.</returns>
    public static string? Validate(string? password, string? email = null)
    {
        if (string.IsNullOrEmpty(password) || password.Length < MinLength)
            return $"Password must be at least {MinLength} characters.";
        if (password.Length > MaxLength)
            return $"Password must be at most {MaxLength} characters.";
        if (password.Distinct().Count() < 4)
            return "Password is too simple. Use a longer mix of words, numbers or symbols.";
        if (Common.Contains(password))
            return "That password is too common. Choose something less predictable.";

        var local = email?.Split('@')[0];
        if (!string.IsNullOrWhiteSpace(local) && local.Length >= 4 &&
            password.Contains(local, StringComparison.OrdinalIgnoreCase))
            return "Password must not contain your email name.";
        return null;
    }
}
