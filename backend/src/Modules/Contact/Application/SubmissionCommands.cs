using MediatR;
using SummitCms.Modules.Contact.Infrastructure;
using SummitCms.Modules.Identity.Contracts;
using SummitCms.Modules.SiteContent.Contracts;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Modules.Contact.Application;

public sealed record CreateSubmissionCommand(string Name, string Email, string? Phone, string? Company, Guid EnquiryTypeId, string Message)
    : IRequest<Result<Guid>>;

public sealed class CreateSubmissionCommandHandler(ContactDbContext db, IEnquiryTypeCatalog enquiryTypes)
    : IRequestHandler<CreateSubmissionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSubmissionCommand request, CancellationToken ct)
    {
        var enquiryType = await enquiryTypes.GetAsync(request.EnquiryTypeId, ct);
        if (enquiryType is null)
            return Result.Failure<Guid>("Unknown enquiry type.", "invalid_enquiry_type");

        var submission = new Domain.ContactSubmission
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Phone = request.Phone?.Trim(),
            Company = request.Company?.Trim(),
            EnquiryTypeId = request.EnquiryTypeId,
            Message = request.Message.Trim()
        };
        db.Submissions.Add(submission);
        await db.SaveChangesAsync(ct);
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
