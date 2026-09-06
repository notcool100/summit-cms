using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SummitCms.Modules.Company.Contracts;
using SummitCms.Modules.Company.Domain;
using SummitCms.Shared.Infrastructure.Endpoints;

namespace SummitCms.Modules.Company.Api;

public static class CompanyEndpoints
{
    public sealed record MilestoneDto(Guid Id, Guid PageId, string Year, string Title, string Body, int DisplayOrder);
    public sealed record MilestoneWriteDto(Guid PageId, string Year, string Title, string Body, int DisplayOrder);

    public sealed record ValueDto(Guid Id, Guid PageId, string Code, string Name, string Body, int DisplayOrder);
    public sealed record ValueWriteDto(Guid PageId, string Code, string Name, string Body, int DisplayOrder);

    public sealed record TeamMemberDto(Guid Id, Guid PageId, string Name, string Title, Guid? MediaId, int DisplayOrder, bool IsActive);
    public sealed record TeamMemberWriteDto(Guid PageId, string Name, string Title, Guid? MediaId, int DisplayOrder, bool IsActive);

    public sealed record LocationDto(Guid Id, Guid PageId, string City, string RoleDescription, bool IsHeadquarters, int DisplayOrder);
    public sealed record LocationWriteDto(Guid PageId, string City, string RoleDescription, bool IsHeadquarters, int DisplayOrder);

    public sealed record AwardDto(Guid Id, Guid PageId, string Year, string Name, int DisplayOrder);
    public sealed record AwardWriteDto(Guid PageId, string Year, string Name, int DisplayOrder);

    public sealed record NarrativeDto(Guid Id, Guid PageId, string Eyebrow, string TitleLine1, string TitleLine2, string Body, Guid? MediaId, string ImageCaption, bool ImageFirst, int DisplayOrder);
    public sealed record NarrativeWriteDto(Guid PageId, string Eyebrow, string TitleLine1, string TitleLine2, string Body, Guid? MediaId, string ImageCaption, bool ImageFirst, int DisplayOrder);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/company").WithTags("Admin.Company");

        group.MapAdminCrud<Milestone, MilestoneWriteDto, MilestoneWriteDto, MilestoneDto>(
            "/milestones", CompanyPermissions.Manage,
            m => new MilestoneDto(m.Id, m.PageId, m.Year, m.Title, m.Body, m.DisplayOrder),
            dto => new Milestone { PageId = dto.PageId, Year = dto.Year, Title = dto.Title, Body = dto.Body, DisplayOrder = dto.DisplayOrder },
            (m, dto) => { m.Year = dto.Year; m.Title = dto.Title; m.Body = dto.Body; m.DisplayOrder = dto.DisplayOrder; });

        group.MapAdminCrud<CompanyValue, ValueWriteDto, ValueWriteDto, ValueDto>(
            "/values", CompanyPermissions.Manage,
            v => new ValueDto(v.Id, v.PageId, v.Code, v.Name, v.Body, v.DisplayOrder),
            dto => new CompanyValue { PageId = dto.PageId, Code = dto.Code, Name = dto.Name, Body = dto.Body, DisplayOrder = dto.DisplayOrder },
            (v, dto) => { v.Code = dto.Code; v.Name = dto.Name; v.Body = dto.Body; v.DisplayOrder = dto.DisplayOrder; });

        group.MapAdminCrud<TeamMember, TeamMemberWriteDto, TeamMemberWriteDto, TeamMemberDto>(
            "/team", CompanyPermissions.Manage,
            t => new TeamMemberDto(t.Id, t.PageId, t.Name, t.Title, t.MediaId, t.DisplayOrder, t.IsActive),
            dto => new TeamMember { PageId = dto.PageId, Name = dto.Name, Title = dto.Title, MediaId = dto.MediaId, DisplayOrder = dto.DisplayOrder, IsActive = dto.IsActive },
            (t, dto) => { t.Name = dto.Name; t.Title = dto.Title; t.MediaId = dto.MediaId; t.DisplayOrder = dto.DisplayOrder; t.IsActive = dto.IsActive; });

        group.MapAdminCrud<OfficeLocation, LocationWriteDto, LocationWriteDto, LocationDto>(
            "/locations", CompanyPermissions.Manage,
            o => new LocationDto(o.Id, o.PageId, o.City, o.RoleDescription, o.IsHeadquarters, o.DisplayOrder),
            dto => new OfficeLocation { PageId = dto.PageId, City = dto.City, RoleDescription = dto.RoleDescription, IsHeadquarters = dto.IsHeadquarters, DisplayOrder = dto.DisplayOrder },
            (o, dto) => { o.City = dto.City; o.RoleDescription = dto.RoleDescription; o.IsHeadquarters = dto.IsHeadquarters; o.DisplayOrder = dto.DisplayOrder; });

        group.MapAdminCrud<Award, AwardWriteDto, AwardWriteDto, AwardDto>(
            "/awards", CompanyPermissions.Manage,
            a => new AwardDto(a.Id, a.PageId, a.Year, a.Name, a.DisplayOrder),
            dto => new Award { PageId = dto.PageId, Year = dto.Year, Name = dto.Name, DisplayOrder = dto.DisplayOrder },
            (a, dto) => { a.Year = dto.Year; a.Name = dto.Name; a.DisplayOrder = dto.DisplayOrder; });

        group.MapAdminCrud<NarrativeBlock, NarrativeWriteDto, NarrativeWriteDto, NarrativeDto>(
            "/narrative", CompanyPermissions.Manage,
            n => new NarrativeDto(n.Id, n.PageId, n.Eyebrow, n.TitleLine1, n.TitleLine2, n.Body, n.MediaId, n.ImageCaption, n.ImageFirst, n.DisplayOrder),
            dto => new NarrativeBlock { PageId = dto.PageId, Eyebrow = dto.Eyebrow, TitleLine1 = dto.TitleLine1, TitleLine2 = dto.TitleLine2, Body = dto.Body, MediaId = dto.MediaId, ImageCaption = dto.ImageCaption, ImageFirst = dto.ImageFirst, DisplayOrder = dto.DisplayOrder },
            (n, dto) => { n.Eyebrow = dto.Eyebrow; n.TitleLine1 = dto.TitleLine1; n.TitleLine2 = dto.TitleLine2; n.Body = dto.Body; n.MediaId = dto.MediaId; n.ImageCaption = dto.ImageCaption; n.ImageFirst = dto.ImageFirst; n.DisplayOrder = dto.DisplayOrder; });
    }
}
