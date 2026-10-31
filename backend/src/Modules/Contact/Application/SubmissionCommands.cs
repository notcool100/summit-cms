using MediatR;
using SummitCms.Modules.Contact.Infrastructure;
using SummitCms.Modules.Identity.Contracts;
using SummitCms.Modules.SiteContent.Contracts;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Modules.Contact.Application;

public sealed record CreateSubmissionCommand(string Name, string Email, string? Phone, string? Company, Guid EnquiryTypeId, string Message)
    : IRequest<Result<Guid>>;

public sealed class CreateSubmissionCommandHandler(ContactDbContext db, IEnquiryTypeCatalog enquiryTypes, IEnquiryNotifier notifier)
    : IRequestHandler<CreateSubmissionCommand, Result<Guid>>
{
    // Generous enough for real enquiries, small enough to stop someone pasting a novel or a payload.
    private const int MaxName = 120, MaxEmail = 254, MaxPhone = 40, MaxCompany = 160, MaxMessage = 5000;

    private static readonly System.Text.RegularExpressions.Regex EmailPattern =
        new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", System.Text.RegularExpressions.RegexOptions.Compiled);
    private static readonly System.Text.RegularExpressions.Regex PhonePattern =
        new(@"^[0-9+()\-\s]{6,40}$", System.Text.RegularExpressions.RegexOptions.Compiled);

    public async Task<Result<Guid>> Handle(CreateSubmissionCommand request, CancellationToken ct)
    {
        var name = request.Name?.Trim() ?? string.Empty;
        var email = request.Email?.Trim() ?? string.Empty;
        var phone = request.Phone?.Trim() ?? string.Empty;
        var company = string.IsNullOrWhiteSpace(request.Company) ? null : request.Company.Trim();
        var message = request.Message?.Trim() ?? string.Empty;

        if (name.Length == 0 || email.Length == 0 || phone.Length == 0 || message.Length == 0)
            return Result.Failure<Guid>("Name, email, phone and message are required.", "validation");
        if (name.Length > MaxName || email.Length > MaxEmail || phone.Length > MaxPhone
            || (company?.Length ?? 0) > MaxCompany || message.Length > MaxMessage)
            return Result.Failure<Guid>("One or more fields are too long.", "validation");
        if (!EmailPattern.IsMatch(email))
            return Result.Failure<Guid>("Please enter a valid email address.", "validation");
        if (!PhonePattern.IsMatch(phone))
            return Result.Failure<Guid>("Please enter a valid phone number.", "validation");

        var enquiryType = await enquiryTypes.GetAsync(request.EnquiryTypeId, ct);
        if (enquiryType is null)
            return Result.Failure<Guid>("Unknown enquiry type.", "invalid_enquiry_type");

        var submission = new Domain.ContactSubmission
        {
            Name = name,
            Email = email.ToLowerInvariant(),
            Phone = phone,
            Company = company,
            EnquiryTypeId = request.EnquiryTypeId,
            Message = message
        };
        db.Submissions.Add(submission);
        await db.SaveChangesAsync(ct);

        // The enquiry is already safely stored; email is a best-effort alert on top, so a mail outage
        // never loses a lead (it still shows in the admin Enquiries inbox).
        await notifier.NotifyAsync(submission, enquiryType.Label, ct);

        return Result.Success(submission.Id);
    }
}

public sealed record SetSubmissionStatusCommand(Guid SubmissionId, Domain.SubmissionStatus Status) : IRequest<Result>;

public sealed class SetSubmissionStatusCommandHandler(ContactDbContext db) : IRequestHandler<SetSubmissionStatusCommand, Result>
{
    public async Task<Result> Handle(SetSubmissionStatusCommand request, CancellationToken ct)
    {
        var submission = await db.Submissions.FindAsync([request.SubmissionId], ct);
        if (submission is null) return Result.Failure("Submission not found.", "not_found");

        submission.Status = request.Status;
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public sealed record AssignSubmissionCommand(Guid SubmissionId, Guid? UserId) : IRequest<Result>;

public sealed class AssignSubmissionCommandHandler(ContactDbContext db, IUserCatalog users) : IRequestHandler<AssignSubmissionCommand, Result>
{
    public async Task<Result> Handle(AssignSubmissionCommand request, CancellationToken ct)
    {
        var submission = await db.Submissions.FindAsync([request.SubmissionId], ct);
        if (submission is null) return Result.Failure("Submission not found.", "not_found");

        if (request.UserId is { } userId && !await users.ExistsAsync(userId, ct))
            return Result.Failure("Assigned user does not exist.", "invalid_user");

        submission.AssignedUserId = request.UserId;
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
