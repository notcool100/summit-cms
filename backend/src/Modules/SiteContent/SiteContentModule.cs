using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.SiteContent.Api;
using SummitCms.Modules.SiteContent.Contracts;
using SummitCms.Modules.SiteContent.Domain;
using SummitCms.Modules.SiteContent.Infrastructure;
using SummitCms.Shared.Infrastructure.Persistence;

namespace SummitCms.Modules.SiteContent;

public static class SiteContentModule
{
    public static IServiceCollection AddSiteContentModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<SiteContentDbContext>(configuration, SiteContentDbContext.Schema);
        services.AddScoped<IPageCatalog, PageCatalog>();
        services.AddScoped<IEnquiryTypeCatalog, EnquiryTypeCatalog>();

        services.AddScoped<ICrudRepository<SiteSetting>, EfCrudRepository<SiteContentDbContext, SiteSetting>>();
        services.AddScoped<ICrudRepository<EnquiryType>, EfCrudRepository<SiteContentDbContext, EnquiryType>>();
        services.AddScoped<ICrudRepository<MetricStat>, EfCrudRepository<SiteContentDbContext, MetricStat>>();

        return services;
    }

    public static IEndpointRouteBuilder MapSiteContentModule(this IEndpointRouteBuilder app)
    {
        SiteContentEndpoints.Map(app);
        return app;
    }
}
