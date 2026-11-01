using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SummitCms.Modules.Contact.Domain;

namespace SummitCms.Modules.Contact.Application;

/// <summary>
/// In-process queue for enquiry alert emails. The submission is saved to the database first, then
/// the email is queued, so the visitor gets an instant response instead of waiting several seconds
/// on the SMTP server. A failed send never loses the enquiry - it stays in Admin -> Contact leads.
/// </summary>
public sealed class EnquiryEmailQueue
{
    public sealed record Job(ContactSubmission Submission, string EnquiryTypeLabel);

    private readonly Channel<Job> _channel = Channel.CreateBounded<Job>(new BoundedChannelOptions(500)
    {
        FullMode = BoundedChannelFullMode.DropOldest,
        SingleReader = true
    });

    public ValueTask EnqueueAsync(Job job, CancellationToken ct) => _channel.Writer.WriteAsync(job, ct);
    public IAsyncEnumerable<Job> ReadAllAsync(CancellationToken ct) => _channel.Reader.ReadAllAsync(ct);
}

/// <summary>The notifier the request pipeline uses: hands the email to the background queue and returns.</summary>
public sealed class QueuedEnquiryNotifier(EnquiryEmailQueue queue) : IEnquiryNotifier
{
    public Task NotifyAsync(ContactSubmission submission, string enquiryTypeLabel, CancellationToken ct) =>
        queue.EnqueueAsync(new EnquiryEmailQueue.Job(submission, enquiryTypeLabel), CancellationToken.None).AsTask();
}

/// <summary>Background worker that sends queued enquiry emails through the SMTP notifier.</summary>
public sealed class EnquiryEmailWorker(EnquiryEmailQueue queue, IServiceScopeFactory scopes, ILogger<EnquiryEmailWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                await using var scope = scopes.CreateAsyncScope();
                var smtp = scope.ServiceProvider.GetRequiredService<SmtpEnquiryNotifier>();
                await smtp.NotifyAsync(job.Submission, job.EnquiryTypeLabel, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                // SmtpEnquiryNotifier already logs its own send failures; this guards the loop itself.
                logger.LogError(ex, "Enquiry {SubmissionId} alert could not be processed", job.Submission.Id);
            }
        }
    }
}
