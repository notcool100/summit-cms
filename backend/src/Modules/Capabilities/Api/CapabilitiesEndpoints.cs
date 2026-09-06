using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SummitCms.Modules.Capabilities.Contracts;
using SummitCms.Modules.Capabilities.Domain;
using SummitCms.Shared.Infrastructure.Endpoints;

namespace SummitCms.Modules.Capabilities.Api;

public static class CapabilitiesEndpoints
{
    public sealed record CapabilityDto(
        Guid Id, string Key, string Name, string TeaserTag, string Body, string Stat, string StatLabel,
        string Background, bool TextFirst, Guid? MediaId, string FigureLabel, int DisplayOrder, bool IsActive);

    public sealed record CapabilityWriteDto(
        string Key, string Name, string TeaserTag, string Body, string Stat, string StatLabel,
        CapabilityBackground Background, bool TextFirst, Guid? MediaId, string FigureLabel, int DisplayOrder, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGroup("/api/admin/capabilities").WithTags("Admin.Capabilities")
            .MapAdminCrud<Capability, CapabilityWriteDto, CapabilityWriteDto, CapabilityDto>(
                "", CapabilitiesPermissions.Manage,
                c => new CapabilityDto(c.Id, c.Key, c.Name, c.TeaserTag, c.Body, c.Stat, c.StatLabel, c.Background.ToString(), c.TextFirst, c.MediaId, c.FigureLabel, c.DisplayOrder, c.IsActive),
                dto => new Capability
                {
                    Key = dto.Key, Name = dto.Name, TeaserTag = dto.TeaserTag, Body = dto.Body, Stat = dto.Stat,
                    StatLabel = dto.StatLabel, Background = dto.Background, TextFirst = dto.TextFirst, MediaId = dto.MediaId,
                    FigureLabel = dto.FigureLabel, DisplayOrder = dto.DisplayOrder, IsActive = dto.IsActive
                },
                (c, dto) =>
                {
                    c.Key = dto.Key; c.Name = dto.Name; c.TeaserTag = dto.TeaserTag; c.Body = dto.Body; c.Stat = dto.Stat;
                    c.StatLabel = dto.StatLabel; c.Background = dto.Background; c.TextFirst = dto.TextFirst; c.MediaId = dto.MediaId;
                    c.FigureLabel = dto.FigureLabel; c.DisplayOrder = dto.DisplayOrder; c.IsActive = dto.IsActive;
                });
    }
}
