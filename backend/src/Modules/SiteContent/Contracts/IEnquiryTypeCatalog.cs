namespace SummitCms.Modules.SiteContent.Contracts;

public sealed record EnquiryTypeSummary(Guid Id, string Label);

/// <summary>Lets the Contact module validate/display the enquiry type chosen on a submission.</summary>
public interface IEnquiryTypeCatalog
{
    Task<EnquiryTypeSummary?> GetAsync(Guid id, CancellationToken ct);
    Task<List<EnquiryTypeSummary>> ListActiveAsync(CancellationToken ct);
}
