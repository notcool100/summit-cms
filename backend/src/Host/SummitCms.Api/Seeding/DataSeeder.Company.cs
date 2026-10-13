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
                    TitleLine1 = "Pilbara-based", TitleLine2 = "specialist access & rigging.",
                    Body = "Summit Industrial Services is a Pilbara-based specialist access and rigging provider delivering scaffolding, rigging, inspection, engineering support and integrated industrial services across demanding operational environments. With more than 50 years of combined industry experience, our team supports maintenance, shutdown, construction, marine and project works with a practical focus on safety, reliability, responsiveness and efficient execution. Based in the Karratha Industrial Estate and servicing the greater Pilbara region, Summit is positioned to support clients across energy, resources, minerals processing, utilities, renewables, rail, marine and construction sectors.",
                    MediaId = Media("https://summit.us/wp-content/uploads/2022/05/20140514_161421-edited-scaled.jpg"),
                    ImageCaption = "KARRATHA INDUSTRIAL ESTATE", ImageFirst = true
                },
                new NarrativeBlock
                {
                    PageId = aboutPageId, DisplayOrder = 1, Eyebrow = "Our Approach",
                    TitleLine1 = "Integrated delivery,", TitleLine2 = "coordinated execution.",
                    Body = "Successful project delivery requires more than skilled personnel and equipment. It requires effective planning, responsive coordination and the ability to adapt quickly to changing site conditions and operational priorities. Our integrated service model brings together scaffolding, access, rigging, inspection, engineering support, logistics and materials management. Through Just-In-Time sequencing and coordinated mobilisation, we ensure the right equipment, materials and resources are available when and where required, helping reduce site congestion, minimize downtime and eliminate unnecessary material movements.",
                    MediaId = Media("https://summit.us/wp-content/uploads/2022/04/IMG_0239-scaled-e1649186987916-873x1024.jpg"),
                    ImageCaption = "COORDINATED EXECUTION", ImageFirst = false
                });
        }

        if (!await db.Milestones.AnyAsync())
        {
            (string Year, string Title, string Body)[] milestones =
            [
                ("2026", "Capability Statement Published", "Summit Industrial Services releases updated capability statement covering scaffolding, rigging, inspection and integrated industrial services across the Pilbara."),
                ("2020s", "Pilbara Expansion", "Expanded operations across energy, resources & minerals, utilities, renewables, rail, marine and construction sectors in the greater Pilbara region."),
                ("2010s", "Team Growth", "Built a team with 50+ years of combined industry experience in scaffolding, rigging and industrial services."),
                ("Founding", "Established in Karratha", "Founded as a Pilbara-based specialist access and rigging provider operating from Karratha Industrial Estate.")
            ];
            for (var i = 0; i < milestones.Length; i++)
                db.Milestones.Add(new Milestone { PageId = aboutPageId, DisplayOrder = i, Year = milestones[i].Year, Title = milestones[i].Title, Body = milestones[i].Body });
        }

        if (!await db.CompanyValues.AnyAsync())
        {
            (string Code, string Name, string Body)[] values =
            [
                ("01", "Safety First", "Safety is fundamental to every aspect of our operations. We apply a risk-based approach to protecting our people, clients and assets in accordance with WHS legislation, codes of practice and Australian Standards including AS/NZS 1891 series. All personnel hold relevant High Risk Work Licences and competency verification."),
                ("02", "Integrated Service Model", "Our integrated service model brings together scaffolding, access, rigging, inspection, engineering support, logistics and materials management for seamless project delivery. One provider, coordinated execution, reduced interfaces."),
                ("03", "Local Knowledge", "Pilbara-based and locally responsive with deep understanding of regional operational challenges. Coordinated mobilisation, Just-In-Time sequencing and practical problem solving backed by local presence and regional expertise."),
                ("04", "Built on Trust, Driven by Excellence", "Committed to dependable service, transparent communication and consistently high standards of workmanship. We deliver what we promise, when we promise it, to the standard expected in demanding industrial environments.")
            ];
            for (var i = 0; i < values.Length; i++)
                db.CompanyValues.Add(new CompanyValue { PageId = aboutPageId, DisplayOrder = i, Code = values[i].Code, Name = values[i].Name, Body = values[i].Body });
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

        if (!await db.OfficeLocations.AnyAsync())
        {
            (string City, string Role, bool Hq)[] locations =
            [
                ("Karratha Industrial Estate, WA 6714", "HQ · Servicing the Greater Pilbara Region", true)
            ];
            for (var i = 0; i < locations.Length; i++)
                db.OfficeLocations.Add(new OfficeLocation { PageId = aboutPageId, DisplayOrder = i, City = locations[i].City, RoleDescription = locations[i].Role, IsHeadquarters = locations[i].Hq });
        }

        if (!await db.Awards.AnyAsync())
        {
            (string Year, string Name)[] awards =
            [
                ("2026", "Working towards ISO accreditation for safety, quality and continuous improvement systems"),
                ("Ongoing", "Compliance with WHS legislation, codes of practice and AS/NZS 1891 series requirements"),
                ("Ongoing", "High Risk Work Licence, competency and VOC verification programs")
            ];
            for (var i = 0; i < awards.Length; i++)
                db.Awards.Add(new Award { PageId = aboutPageId, DisplayOrder = i, Year = awards[i].Year, Name = awards[i].Name });
        }

        await db.SaveChangesAsync();
    }
}
