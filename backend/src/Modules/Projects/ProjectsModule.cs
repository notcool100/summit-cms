using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Projects.Api;
using SummitCms.Modules.Projects.Contracts;
using SummitCms.Modules.Projects.Domain;
using SummitCms.Modules.Projects.Infrastructure;
using SummitCms.Shared.Infrastructure.Persistence;

namespace SummitCms.Modules.Projects;

public static class ProjectsModule
{
    public static IServiceCollection AddProjectsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<ProjectsDbContext>(configuration, ProjectsDbContext.Schema);
        services.AddScoped<IProjectCatalog, ProjectCatalog>();
        services.AddScoped<ICrudRepository<ProjectIndustryCategory>, EfCrudRepository<ProjectsDbContext, ProjectIndustryCategory>>();
        return services;
    }

    public static IEndpointRouteBuilder MapProjectsModule(this IEndpointRouteBuilder app)
    {
        ProjectEndpoints.Map(app);
        IndustryCategoryEndpoints.Map(app);
        ProjectPublicEndpoints.Map(app);
        return app;
    }
}
