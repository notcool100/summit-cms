using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Capabilities.Contracts;
using SummitCms.Modules.Careers.Contracts;
using SummitCms.Modules.Company.Contracts;
using SummitCms.Modules.Contact.Contracts;
using SummitCms.Modules.Identity.Application;
using SummitCms.Modules.Identity.Contracts;
using SummitCms.Modules.Identity.Domain;
using SummitCms.Modules.Identity.Infrastructure;
using SummitCms.Modules.Industries.Contracts;
using SummitCms.Modules.Media.Contracts;
using SummitCms.Modules.Media.Domain;
using SummitCms.Modules.Media.Infrastructure;
using SummitCms.Modules.Projects.Contracts;
using SummitCms.Modules.SiteContent.Contracts;
using SummitCms.Modules.SiteContent.Domain;
using SummitCms.Modules.SiteContent.Infrastructure;
using SummitCms.Shared.Kernel.Common;
using Serilog;

namespace SummitCms.Api.Seeding;

/// <summary>
/// Idempotent startup seeder: system roles/permissions + a SuperAdmin user, plus the site's actual
/// current copy (ported from the old static frontend data files) so the CMS starts with real content
/// instead of an empty database. Each step checks "do I already have rows" before inserting.
/// </summary>
public static partial class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var config = sp.GetRequiredService<IConfiguration>();

        await SeedIdentityAsync(sp, config);
        var mediaMap = await SeedMediaAsync(sp);
        var pageIds = await SeedPagesAsync(sp);
        await SeedSiteSettingsAndEnquiryTypesAsync(sp);
        await SeedCompanyAsync(sp, pageIds, mediaMap);
        await SeedCapabilitiesAsync(sp, mediaMap);
        var projectIds = await SeedProjectsAsync(sp, mediaMap);
        await SeedIndustriesAsync(sp, mediaMap, projectIds);
        await SeedCareersAsync(sp, pageIds, mediaMap);
        await SeedMetricStatsAsync(sp, pageIds);
    }

    private static async Task SeedIdentityAsync(IServiceProvider sp, IConfiguration config)
    {
        var db = sp.GetRequiredService<IdentityDbContext>();

        var allPermissions = IdentityPermissions.All
            .Concat(MediaPermissions.All)
            .Concat(SiteContentPermissions.All)
            .Concat(CompanyPermissions.All)
            .Concat(CapabilitiesPermissions.All)
            .Concat(IndustriesPermissions.All)
            .Concat(ProjectsPermissions.All)
            .Concat(CareersPermissions.All)
            .Concat(ContactPermissions.All)
            .ToList();

        foreach (var (code, description) in allPermissions)
        {
            if (!await db.Permissions.AnyAsync(p => p.Code == code))
                db.Permissions.Add(new Permission { Code = code, Description = description });
        }
        await db.SaveChangesAsync();

        var allPermissionEntities = await db.Permissions.ToListAsync();

        async Task<Role> EnsureRoleAsync(string name, string description, IEnumerable<string>? permissionCodes)
        {
            var role = await db.Roles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Name == name);
            if (role is null)
            {
                role = new Role { Name = name, Description = description, IsSystemRole = true };
                db.Roles.Add(role);
                await db.SaveChangesAsync();
            }

            if (permissionCodes is not null)
            {
                var codes = permissionCodes.ToHashSet();
                var toGrant = allPermissionEntities.Where(p => codes.Contains(p.Code) && role.RolePermissions.All(rp => rp.PermissionId != p.Id));
                foreach (var permission in toGrant)
                    db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permission.Id });
                await db.SaveChangesAsync();
            }

            return role;
        }

        // SuperAdmin gets no explicit RolePermission rows - PermissionAuthorizationHandler grants it everything by role name.
        var superAdmin = await EnsureRoleAsync("SuperAdmin", "Full, unrestricted access", null);
        await EnsureRoleAsync("Admin", "Manage all content, media, and contact submissions", allPermissions.Select(p => p.Code).Where(c => c != IdentityPermissions.ManageRoles));
        await EnsureRoleAsync("Editor", "Manage marketing content only, not users/roles/media library", allPermissions.Select(p => p.Code).Where(c =>
            !c.StartsWith("identity.") && c != MediaPermissions.Manage));
        await EnsureRoleAsync("Viewer", "Read-only access to the admin panel", []);

        if (!await db.Users.AnyAsync())
        {
            var hasher = sp.GetRequiredService<IPasswordHasherService>();
            var adminEmail = (config["Seed:AdminEmail"] ?? "anjaljoshi6@gmail.com").Trim().ToLowerInvariant();
            var adminPassword = config["Seed:AdminPassword"] ?? GenerateStrongPassword();

            var user = new User
            {
                Email = adminEmail,
                PasswordHash = hasher.Hash(adminPassword),
                FirstName = "Site",
                LastName = "Admin",
                IsActive = true
            };
            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = superAdmin.Id });
            db.Users.Add(user);
            await db.SaveChangesAsync();

            Log.Information("Seeded SuperAdmin user {Email} - {PasswordNote}", adminEmail,
                config["Seed:AdminPassword"] is null
                    ? $"generated password (change immediately): {adminPassword}"
                    : "password from Seed:AdminPassword config");
        }
    }

    private static string GenerateStrongPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(24);
        return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
    }
}
