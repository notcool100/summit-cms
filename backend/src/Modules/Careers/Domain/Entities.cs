using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.Careers.Domain;

public enum CareerTrackType { Craft = 0, Professional = 1 }
public enum EmploymentType { FullTime = 0, PartTime = 1, Contract = 2, Seasonal = 3 }

public class JobTrack : AuditableEntity, IOrderable
{
    public Guid PageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PathLabel { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Guid? MediaId { get; set; }
    public string CtaLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public List<JobTrackTag> Tags { get; set; } = [];
}

public class JobTrackTag : AuditableEntity, IOrderable
{
    public Guid JobTrackId { get; set; }
    public JobTrack JobTrack { get; set; } = null!;
    public string Tag { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

/// <summary>Real, postable job openings - beyond what the old static frontend rendered, since a CMS
/// for a careers page should let an admin actually publish and close reqs.</summary>
public class JobOpening : AuditableEntity, IOrderable
{
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; }
    public CareerTrackType TrackType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ApplyContact { get; set; } = string.Empty; // email or URL
    public bool IsActive { get; set; } = true;
    public DateTimeOffset PostedAt { get; set; }
    public DateTimeOffset? ClosesAt { get; set; }
    public int DisplayOrder { get; set; }
}
