using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Contact.Api;
using SummitCms.Modules.Contact.Application;
using SummitCms.Modules.Contact.Infrastructure;
using SummitCms.Shared.Infrastructure.Persistence;

namespace SummitCms.Modules.Contact;

public static class ContactModule
{
    public static IServiceCollection AddContactModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<ContactDbContext>(configuration, ContactDbContext.Schema);
        services.Configure<EnquiryEmailOptions>(configuration.GetSection(EnquiryEmailOptions.Section));
        // Requests enqueue the alert; EnquiryEmailWorker sends it in the background (no visitor wait).
        services.AddScoped<SmtpEnquiryNotifier>();
        services.AddSingleton<EnquiryEmailQueue>();
        services.AddScoped<IEnquiryNotifier, QueuedEnquiryNotifier>();
        services.AddHostedService<EnquiryEmailWorker>();
        return services;
    }

    public static IEndpointRouteBuilder MapContactModule(this IEndpointRouteBuilder app)
    {
        ContactEndpoints.Map(app);
        return app;
    }
}
