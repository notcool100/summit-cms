using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.Identity.Domain;

public class Role : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>System roles (SuperAdmin/Admin/Editor/Viewer) can't be deleted; admin-created custom roles can.</summary>
    public bool IsSystemRole { get; set; }

    public List<RolePermission> RolePermissions { get; set; } = [];
    public List<UserRole> UserRoles { get; set; } = [];
}

public class Permission : AuditableEntity
{
    /// <summary>Dot-separated code checked against the "permission" claim, e.g. "projects.manage".</summary>
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class RolePermission
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}

public class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
