using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Company.Api;
using SummitCms.Modules.Company.Domain;
using SummitCms.Modules.Company.Infrastructure;
using SummitCms.Shared.Infrastructure.Persistence;

namespace SummitCms.Modules.Company;

public static class CompanyModule
{
    public static IServiceCollection AddCompanyModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<CompanyDbContext>(configuration, CompanyDbContext.Schema);

        services.AddScoped<ICrudRepository<Milestone>, EfCrudRepository<CompanyDbContext, Milestone>>();
        services.AddScoped<ICrudRepository<CompanyValue>, EfCrudRepository<CompanyDbContext, CompanyValue>>();
        services.AddScoped<ICrudRepository<TeamMember>, EfCrudRepository<CompanyDbContext, TeamMember>>();
        services.AddScoped<ICrudRepository<OfficeLocation>, EfCrudRepository<CompanyDbContext, OfficeLocation>>();
        services.AddScoped<ICrudRepository<Award>, EfCrudRepository<CompanyDbContext, Award>>();
        services.AddScoped<ICrudRepository<NarrativeBlock>, EfCrudRepository<CompanyDbContext, NarrativeBlock>>();

        return services;
    }

    public static IEndpointRouteBuilder MapCompanyModule(this IEndpointRouteBuilder app)
    {
        CompanyEndpoints.Map(app);
        return app;
    }
}
