using Microsoft.AspNetCore.Identity;
using SummitCms.Modules.Identity.Application;
using SummitCms.Modules.Identity.Domain;

namespace SummitCms.Modules.Identity.Infrastructure;

public sealed class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(null!, password);

    public bool Verify(string hash, string password) =>
        _hasher.VerifyHashedPassword(null!, hash, password) != PasswordVerificationResult.Failed;
}
