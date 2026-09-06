using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Industries.Api;
using SummitCms.Modules.Industries.Domain;
using SummitCms.Modules.Industries.Infrastructure;
using SummitCms.Shared.Infrastructure.Persistence;

namespace SummitCms.Modules.Industries;

public static class IndustriesModule
{
    public static IServiceCollection AddIndustriesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<IndustriesDbContext>(configuration, IndustriesDbContext.Schema);
        services.AddScoped<ICrudRepository<Industry>, EfCrudRepository<IndustriesDbContext, Industry>>();
        return services;
    }

    public static IEndpointRouteBuilder MapIndustriesModule(this IEndpointRouteBuilder app)
    {
        IndustryEndpoints.Map(app);
        IndustryPublicEndpoints.Map(app);
        return app;
    }
}
