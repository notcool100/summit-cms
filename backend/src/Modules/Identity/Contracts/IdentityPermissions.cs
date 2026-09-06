namespace SummitCms.Modules.Identity.Contracts;

/// <summary>Permission codes owned by the Identity module, seeded into identity.permissions at startup.</summary>
public static class IdentityPermissions
{
    public const string ManageUsers = "identity.users.manage";
    public const string ManageRoles = "identity.roles.manage";
    public const string ReadAuditLog = "identity.audit.read";

    public static readonly IReadOnlyList<(string Code, string Description)> All =
    [
        (ManageUsers, "Create, update, activate/deactivate admin users"),
        (ManageRoles, "Create roles and edit their permissions"),
        (ReadAuditLog, "View the admin audit log")
    ];
}
