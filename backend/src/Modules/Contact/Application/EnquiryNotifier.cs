using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SummitCms.Modules.Contact.Domain;

namespace SummitCms.Modules.Contact.Application;

/// <summary>Alerts the business mailbox when a new website enquiry arrives.</summary>
public interface IEnquiryNotifier
{
    Task NotifyAsync(ContactSubmission submission, string enquiryTypeLabel, CancellationToken ct);
}

/// <summary>Bound from the "Email" config section. Leave Smtp:Host blank to disable email alerts.</summary>
public sealed class EnquiryEmailOptions
{
    public const string Section = "Email";

    /// <summary>Mailbox that receives enquiry alerts, e.g. support@summit-is.com.au.</summary>
    public string NotifyTo { get; set; } = string.Empty;
    /// <summary>Sender address; must be a mailbox the SMTP account is allowed to send as.</summary>
    public string From { get; set; } = string.Empty;
    public string FromName { get; set; } = "Summit Industrial Services Website";
    public SmtpSettings Smtp { get; set; } = new();

    public sealed class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Smtp.Host) && !string.IsNullOrWhiteSpace(NotifyTo) && !string.IsNullOrWhiteSpace(From);
}

public sealed class SmtpEnquiryNotifier(IOptions<EnquiryEmailOptions> options, ILogger<SmtpEnquiryNotifier> logger)
    : IEnquiryNotifier
{
    public async Task NotifyAsync(ContactSubmission submission, string enquiryTypeLabel, CancellationToken ct)
    {
        var opts = options.Value;
        if (!opts.IsConfigured)
        {
            logger.LogWarning(
                "Enquiry {SubmissionId} saved but no email alert sent: Email:Smtp:Host, Email:NotifyTo and Email:From must be configured",
                submission.Id);
            return;
        }

        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(opts.From, opts.FromName),
                Subject = $"Website enquiry: {enquiryTypeLabel} from {submission.Name}",
                Body = BuildBody(submission, enquiryTypeLabel),
                IsBodyHtml = false,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };
            foreach (var to in opts.NotifyTo.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                message.To.Add(to);
            // Replying from the inbox goes straight back to the person who enquired.
            message.ReplyToList.Add(new MailAddress(submission.Email, submission.Name));

            using var client = new SmtpClient(opts.Smtp.Host, opts.Smtp.Port)
            {
                EnableSsl = opts.Smtp.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 15000
            };
            if (!string.IsNullOrWhiteSpace(opts.Smtp.Username))
                client.Credentials = new NetworkCredential(opts.Smtp.Username, opts.Smtp.Password);

            await client.SendMailAsync(message, ct);
            logger.LogInformation("Enquiry {SubmissionId} alert emailed to {NotifyTo}", submission.Id, opts.NotifyTo);
        }
        catch (Exception ex)
        {
            // Never fail the visitor's submission because of mail problems - it is already stored.
            logger.LogError(ex, "Enquiry {SubmissionId} saved but the email alert failed", submission.Id);
        }
    }

    private static string BuildBody(ContactSubmission s, string enquiryTypeLabel)
    {
        var sb = new StringBuilder();
        sb.AppendLine("A new enquiry was submitted through the website contact form.");
        sb.AppendLine();
        sb.AppendLine($"Enquiry type: {enquiryTypeLabel}");
        sb.AppendLine($"Name:         {s.Name}");
        sb.AppendLine($"Company:      {s.Company ?? "-"}");
        sb.AppendLine($"Email:        {s.Email}");
        sb.AppendLine($"Phone:        {s.Phone ?? "-"}");
        sb.AppendLine($"Received:     {s.CreatedAt:yyyy-MM-dd HH:mm} UTC");
        sb.AppendLine();
        sb.AppendLine("Message:");
        sb.AppendLine(s.Message);
        sb.AppendLine();
        sb.AppendLine("Reply to this email to respond directly. The enquiry is also in the admin panel under Enquiries.");
        return sb.ToString();
    }
}
