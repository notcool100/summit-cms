using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Careers.Api;
using SummitCms.Modules.Careers.Domain;
using SummitCms.Modules.Careers.Infrastructure;
using SummitCms.Shared.Infrastructure.Persistence;

namespace SummitCms.Modules.Careers;

public static class CareersModule
{
    public static IServiceCollection AddCareersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<CareersDbContext>(configuration, CareersDbContext.Schema);
        services.AddScoped<ICrudRepository<JobTrack>, EfCrudRepository<CareersDbContext, JobTrack>>();
        services.AddScoped<ICrudRepository<JobOpening>, EfCrudRepository<CareersDbContext, JobOpening>>();
        return services;
    }

    public static IEndpointRouteBuilder MapCareersModule(this IEndpointRouteBuilder app)
    {
        CareersEndpoints.Map(app);
        CareersPublicEndpoints.Map(app);
        return app;
    }
}
