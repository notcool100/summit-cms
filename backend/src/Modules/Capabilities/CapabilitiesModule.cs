using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Capabilities.Api;
using SummitCms.Modules.Capabilities.Domain;
using SummitCms.Modules.Capabilities.Infrastructure;
using SummitCms.Shared.Infrastructure.Persistence;

namespace SummitCms.Modules.Capabilities;

public static class CapabilitiesModule
{
    public static IServiceCollection AddCapabilitiesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<CapabilitiesDbContext>(configuration, CapabilitiesDbContext.Schema);
        services.AddScoped<ICrudRepository<Capability>, EfCrudRepository<CapabilitiesDbContext, Capability>>();
        return services;
    }

    public static IEndpointRouteBuilder MapCapabilitiesModule(this IEndpointRouteBuilder app)
    {
        CapabilitiesEndpoints.Map(app);
        return app;
    }
}
