using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.SiteContent.Domain;

/// <summary>Free-form admin-editable global settings: company phone/email/address, social links, footer text.</summary>
public class SiteSetting : AuditableEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueType { get; set; } = "string"; // string | url | email | json
}

public class EnquiryType : AuditableEntity, IOrderable
{
    public string Label { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Normalizes StatItem/WhySummitStat/hseStats - identically-shaped stat blocks scattered across the
/// old static frontend - into one reusable table, distinguished by which page + which group on that
/// page (e.g. "home_stats", "why_summit", "hse") they belong to.
/// </summary>
public class MetricStat : AuditableEntity, IOrderable
{
    public Guid PageId { get; set; }
    public Page Page { get; set; } = null!;
    public string GroupKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
    public string? Note { get; set; }
    public int DisplayOrder { get; set; }
}
