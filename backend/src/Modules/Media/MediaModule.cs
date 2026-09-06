using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Media.Api;
using SummitCms.Modules.Media.Application;
using SummitCms.Modules.Media.Contracts;
using SummitCms.Modules.Media.Infrastructure;
using SummitCms.Shared.Infrastructure.Persistence;

namespace SummitCms.Modules.Media;

public static class MediaModule
{
    public static IServiceCollection AddMediaModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<MediaDbContext>(configuration, MediaDbContext.Schema);
        services.Configure<LocalFileStorageOptions>(configuration.GetSection(LocalFileStorageOptions.SectionName));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IMediaCatalog, MediaCatalog>();
        return services;
    }

    public static IEndpointRouteBuilder MapMediaModule(this IEndpointRouteBuilder app)
    {
        MediaEndpoints.Map(app);
        return app;
    }
}
