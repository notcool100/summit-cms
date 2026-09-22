using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Blog.Api;
using SummitCms.Modules.Blog.Infrastructure;
using SummitCms.Shared.Infrastructure.Persistence;

namespace SummitCms.Modules.Blog;

public static class BlogModule
{
    public static IServiceCollection AddBlogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModuleDbContext<BlogDbContext>(configuration, BlogDbContext.Schema);
        return services;
    }

    public static IEndpointRouteBuilder MapBlogModule(this IEndpointRouteBuilder app)
    {
        BlogEndpoints.Map(app);
        BlogPublicEndpoints.Map(app);
        return app;
    }
}
