using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.Media.Domain;

public class MediaFolder : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid? ParentFolderId { get; set; }
    public MediaFolder? ParentFolder { get; set; }
}
