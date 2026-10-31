using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Company.Domain;
using SummitCms.Modules.Company.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task SeedCompanyAsync(IServiceProvider sp, Dictionary<string, Guid> pageIds, Dictionary<string, Guid> mediaMap)
    {
        var db = sp.GetRequiredService<CompanyDbContext>();
        var aboutPageId = pageIds[PageSlugs.About];

        Guid Media(string url) => mediaMap[url];

        if (!await db.NarrativeBlocks.AnyAsync())
        {
            db.NarrativeBlocks.AddRange(
                new NarrativeBlock
                {
                    PageId = aboutPageId, DisplayOrder = 0, Eyebrow = "Who We Are",
                    TitleLine1 = "Pilbara based", TitleLine2 = "specialist access & rigging.",
                    Body = "Summit Industrial Services is a Pilbara based specialist access and rigging provider delivering scaffolding, rigging, inspection, engineering support and integrated industrial services across demanding operational environments. With more than 50 years of combined industry experience, our team supports maintenance, shutdown, construction, marine and project works with a practical focus on safety, reliability, responsiveness and efficient execution. Based in the Karratha Industrial Estate and servicing the greater Pilbara region, Summit is positioned to support clients across energy, resources, minerals processing, utilities, renewables, rail, marine and construction sectors.",
                    MediaId = Media(Stock("site-planning-engineers")),
                    ImageCaption = "ILLUSTRATIVE IMAGE", ImageFirst = true
                },
                new NarrativeBlock
                {
                    PageId = aboutPageId, DisplayOrder = 1, Eyebrow = "Our Approach",
                    TitleLine1 = "Integrated delivery,", TitleLine2 = "coordinated execution.",
                    Body = "Successful project delivery requires more than skilled personnel and equipment. It requires effective planning, responsive coordination and the ability to adapt to changing site conditions and operational priorities. Our integrated service model brings together scaffolding, access, rigging, inspection, engineering support, logistics and materials management. Through Just In Time sequencing and coordinated mobilisation, we aim to have the right equipment, materials and resources available when and where required, helping reduce congestion, downtime and unnecessary material movements.",
                    MediaId = Media(Stock("road-train-logistics")),
                    ImageCaption = "ILLUSTRATIVE IMAGE", ImageFirst = false
                });
        }

            var integratedNarrative = await db.NarrativeBlocks.FirstOrDefaultAsync(n => n.Eyebrow == "Our Approach");
            if (integratedNarrative is not null)
            {
                integratedNarrative.Body = integratedNarrative.Body.Replace(
                "Through Just In Time sequencing and coordinated mobilisation, we ensure the right equipment, materials and resources are available when and where required, helping reduce site congestion, minimize downtime and eliminate unnecessary material movements.",
                "Through Just In Time sequencing and coordinated mobilisation, we aim to have the right equipment, materials and resources available when and where required, helping reduce congestion, downtime and unnecessary material movements.");
            }

        var seededMilestoneTitles = new[]
        {
            "Capability Statement Published", "Pilbara Expansion", "Team Growth", "Established in Karratha"
        };
        var unsupportedMilestones = await db.Milestones
            .Where(m => seededMilestoneTitles.Contains(m.Title))
            .ToListAsync();
        db.Milestones.RemoveRange(unsupportedMilestones);
        if (!await db.Milestones.AnyAsync(m => m.Title == "2026 Capability Statement"))
        {
            db.Milestones.Add(new Milestone
            {
                PageId = aboutPageId,
                DisplayOrder = 0,
                Year = "2026",
                Title = "2026 Capability Statement",
                Body = "Updated company profile, capabilities, safety commitments and Greater Pilbara service coverage."
            });
        }

        (string Code, string Name, string Body)[] values =
        [
            ("01", "Safety First", "Safety is fundamental to our work. We apply a risk based approach using pre start planning, SWMS, JHAs, task specific risk assessments, competent personnel and fit for purpose equipment, following applicable WHS requirements and relevant standards."),
            ("02", "Integrated Service Model", "Our service model brings together scaffolding, access, rigging, inspection, engineering support, logistics and materials management for coordinated project delivery."),
            ("03", "Local Knowledge", "Pilbara based and locally responsive, Summit services the Greater Pilbara region from its Karratha Industrial Estate base."),
            ("04", "Built on Trust, Driven by Excellence", "We are committed to dependable service, transparent communication and consistently high standards of workmanship.")
        ];
        for (var i = 0; i < values.Length; i++)
        {
            var value = values[i];
            var existing = await db.CompanyValues.FirstOrDefaultAsync(item => item.Code == value.Code);
            if (existing is null)
            {
                existing = new CompanyValue { PageId = aboutPageId, Code = value.Code };
                db.CompanyValues.Add(existing);
            }

            existing.PageId = aboutPageId;
            existing.DisplayOrder = i;
            existing.Name = value.Name;
            existing.Body = value.Body;
        }

        if (!await db.TeamMembers.AnyAsync())
        {
            (string Name, string Title)[] leaders =
            [
                ("Jason Matakatea", "Managing Director"),
                ("Rebecca Neumann", "Director & Accounts"),
                ("Travis Kuru", "North West Operations Manager"),
                ("Nikki Sharpe", "Planning & Scheduling")
            ];
            for (var i = 0; i < leaders.Length; i++)
                db.TeamMembers.Add(new TeamMember { PageId = aboutPageId, DisplayOrder = i, Name = leaders[i].Name, Title = leaders[i].Title, MediaId = null, IsActive = true });
        }

        var officeLocations = await db.OfficeLocations.OrderBy(l => l.DisplayOrder).ToListAsync();
        var headquarters = officeLocations.FirstOrDefault();
        if (headquarters is null)
        {
            headquarters = new OfficeLocation();
            db.OfficeLocations.Add(headquarters);
        }
        headquarters.PageId = aboutPageId;
        headquarters.DisplayOrder = 0;
        headquarters.City = "Karratha Industrial Estate, WA 6714";
        headquarters.RoleDescription = "HQ · Servicing the Greater Pilbara Region";
        headquarters.IsHeadquarters = true;
        db.OfficeLocations.RemoveRange(officeLocations.Skip(1));

        if (!await db.Awards.AnyAsync())
        {
            (string Year, string Name)[] awards =
            [
                ("2026", "Working towards ISO accreditation for safety, quality and continuous improvement systems"),
                ("Ongoing", "Work at heights activities managed in accordance with applicable WHS requirements and relevant Australian Standards"),
                ("Ongoing", "High Risk Work Licence, competency and VOC verification programs")
            ];
            for (var i = 0; i < awards.Length; i++)
                db.Awards.Add(new Award { PageId = aboutPageId, DisplayOrder = i, Year = awards[i].Year, Name = awards[i].Name });
        }

        var outdatedStandardAward = await db.Awards.FirstOrDefaultAsync(a =>
            a.Name == "Compliance with WHS legislation, codes of practice and AS/NZS 1891 series requirements");
        if (outdatedStandardAward is not null)
            outdatedStandardAward.Name = "Work at heights activities managed in accordance with applicable WHS requirements and relevant Australian Standards";

        await db.SaveChangesAsync();
    }
}
