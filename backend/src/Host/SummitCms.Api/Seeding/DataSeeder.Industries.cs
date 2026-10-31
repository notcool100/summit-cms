using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Industries.Domain;
using SummitCms.Modules.Industries.Infrastructure;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task SeedIndustriesAsync(IServiceProvider sp, Dictionary<string, Guid> mediaMap, Dictionary<string, Guid> projectSlugToId)
    {
        var db = sp.GetRequiredService<IndustriesDbContext>();

        Guid Media(string url) => mediaMap[url];

        (string Idx, string Name, string Tag, string Body, string Url, string Fig, (string Label, string Stat, string ProjectSlug)[] Links)[] rows =
        [
            ("01", "Energy", "Infrastructure access and rigging for energy sector",
                "Access and rigging support for energy infrastructure, maintenance, construction and project activities across the Pilbara region. Supporting operational facilities, planned shutdowns and construction phases with integrated scaffolding, rigging and inspection services.",
                Stock("energy-process-plant"), "ENERGY INFRASTRUCTURE",
                []),
            ("02", "Resources & Minerals", "Specialist support for mining and processing operations",
                "Specialist services supporting mining and mineral processing operations, shutdowns, maintenance and projects with integrated scaffolding, access and rigging solutions. Experienced in demanding operational environments where safety, reliability and schedule adherence are critical.",
                Stock("resources-haul-truck"), "RESOURCES & MINERALS PROCESSING",
                []),
            ("03", "Utilities", "Access solutions for essential infrastructure",
                "Practical access solutions supporting essential infrastructure, upgrades, maintenance and construction across the Pilbara. Reliable support for utilities sectors requiring minimal disruption and coordinated execution.",
                Stock("utilities-water-treatment"), "UTILITIES INFRASTRUCTURE",
                []),
            ("04", "Renewables", "Supporting renewable and emerging energy projects",
                "Scaffolding, access and rigging support for renewable energy and emerging energy projects in the Pilbara region. Adaptable solutions for evolving technologies and construction methodologies.",
                Stock("renewables-offshore-wind"), "RENEWABLE ENERGY PROJECTS",
                []),
            ("05", "Rail", "Specialised solutions for rail infrastructure",
                "Specialised access and rigging solutions supporting rail infrastructure, maintenance, upgrades and projects.",
                Stock("rail-freight-wagons"), "RAIL INFRASTRUCTURE",
                []),
            ("06", "Marine & Construction", "Integrated support for marine and industrial construction",
                "Support for ports, marine infrastructure and industrial construction environments with scaffolding, access and rigging capability.",
                Stock("marine-port-cranes"), "MARINE & CONSTRUCTION",
                [])
        ];

        for (var i = 0; i < rows.Length; i++)
        {
            var r = rows[i];
            var industry = await db.Industries.FirstOrDefaultAsync(item => item.Idx == r.Idx);
            if (industry is null)
            {
                industry = new Industry { Idx = r.Idx, MediaId = Media(r.Url) };
                db.Industries.Add(industry);
            }

            if (IsThirdPartyMedia(industry.MediaId)) industry.MediaId = Media(r.Url);
            industry.Name = r.Name;
            industry.Tag = r.Tag;
            industry.Body = r.Body;
            industry.FigureLabel = r.Fig;
            industry.DisplayOrder = i;
            industry.IsActive = true;

            for (var j = 0; j < r.Links.Length; j++)
            {
                var (label, stat, slug) = r.Links[j];
                if (!projectSlugToId.TryGetValue(slug, out var projectId)) continue;

                industry.ProjectLinks.Add(new IndustryProjectLink
                {
                    ProjectId = projectId,
                    CustomLabel = label,
                    CustomStat = stat,
                    DisplayOrder = j
                });
            }

        }

        await db.SaveChangesAsync();
    }
}
