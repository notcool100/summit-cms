using SummitCms.Shared.Kernel.Entities;

namespace SummitCms.Modules.Contact.Domain;

public enum SubmissionStatus { New = 0, InReview = 1, Resolved = 2, Archived = 3 }

/// <summary>Turns the old static enquiry-type dropdown into a working lead pipeline for the sales/HR team.</summary>
public class ContactSubmission : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    /// <summary>Soft reference to content.enquiry_types(Id) - validated at write time via ISiteContentCatalog.</summary>
    public Guid EnquiryTypeId { get; set; }
    public string Message { get; set; } = string.Empty;
    public SubmissionStatus Status { get; set; } = SubmissionStatus.New;
    /// <summary>Soft reference to identity.users(Id) - validated via IUserCatalog.</summary>
    public Guid? AssignedUserId { get; set; }
}
