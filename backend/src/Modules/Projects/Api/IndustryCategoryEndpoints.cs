using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SummitCms.Modules.Projects.Contracts;
using SummitCms.Modules.Projects.Domain;
using SummitCms.Shared.Infrastructure.Endpoints;

namespace SummitCms.Modules.Projects.Api;

public static class IndustryCategoryEndpoints
{
    public sealed record CategoryDto(Guid Id, string Name, int DisplayOrder);
    public sealed record CategoryWriteDto(string Name, int DisplayOrder);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGroup("/api/admin/projects/industry-categories").WithTags("Admin.Projects")
            .MapAdminCrud<ProjectIndustryCategory, CategoryWriteDto, CategoryWriteDto, CategoryDto>(
                "", ProjectsPermissions.Manage,
                c => new CategoryDto(c.Id, c.Name, c.DisplayOrder),
                dto => new ProjectIndustryCategory { Name = dto.Name, DisplayOrder = dto.DisplayOrder },
                (c, dto) => { c.Name = dto.Name; c.DisplayOrder = dto.DisplayOrder; });
    }
}
