using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Blog.Domain;
using SummitCms.Modules.Blog.Infrastructure;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task SeedVerifiedInsightsAsync(IServiceProvider sp, Dictionary<string, Guid> mediaMap)
    {
        var db = sp.GetRequiredService<BlogDbContext>();

        (string Slug, string Title, string Excerpt, string Category, string CoverUrl,
            DateTimeOffset PublishedAt, string[] Paragraphs)[] posts =
        [
            ("scaffold-erection-modification-and-dismantling", "Erection, Modification and Dismantling: Planning the Whole Scaffold Lifecycle",
                "A scaffold is not finished when it is built. Modification, ongoing support and dismantling deserve the same planning as installation.",
                "Scaffolding", "https://summit.us/wp-content/uploads/2022/05/IMG_5350-scaled.jpg",
                new DateTimeOffset(2026, 10, 15, 8, 0, 0, TimeSpan.Zero),
                [
                    "Scaffolding is often discussed in terms of the day it goes up. In practice the work spans a longer lifecycle: installation, modification as the job develops, ongoing support while crews rely on it, and dismantling once access is no longer needed.",
                    "Summit's 2026 capability statement describes safe and efficient installation, modification, dismantling and ongoing support for projects of varying size and complexity. The approach is tailored to site conditions, operational priorities and project schedules, using systems that range from traditional Tube & Fit to the AT-PAC Ringlock modular system.",
                    "Modification is where planning is most often tested. Work scopes change, other trades arrive, and access needs shift. Treating a change as a managed task, with the same risk assessment, competent personnel and inspection and tagging as the original build, helps keep the structure accountable for its intended use.",
                    "Dismantling benefits from the same discipline. Coordinated logistics and materials management, including demobilisation and materials reconciliation, help make sure equipment is accounted for and removed in a sequence that suits the wider site.",
                    "Where a structure is complex or the access requirement is specialised, scaffold design and engineering coordination can support the plan. Where engineered design is required, drawings and supporting documentation can be developed and verified by suitably qualified engineers.",
                    "A useful starting point is to plan all four stages at once: how the scaffold will be installed, how it may need to change, who will inspect and tag it, and how it will come down. The specific requirements should always be confirmed against the project, the site procedures and the applicable standards."
                ]),
            ("choosing-a-scaffolding-system", "Choosing a Scaffolding System for the Work",
                "Tube & Fit and modular Ringlock systems each have a place. Start with the access requirement, site conditions and project sequence.",
                "Scaffolding", "https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg",
                new DateTimeOffset(2026, 10, 13, 8, 0, 0, TimeSpan.Zero),
                [
                    "Scaffolding selection starts with the work that needs to be reached. The structure, access points, site conditions and operational priorities all shape the solution. A system that suits one task may not be the best fit for another.",
                    "Summit's capability includes traditional Tube & Fit scaffolding and the AT-PAC Ringlock modular system. The 2026 capability statement describes both as part of a diverse access offering, with installation, modification and dismantling support.",
                    "For complex structures or specialised access requirements, scaffold design and engineering coordination can help develop a project specific approach. Where engineered design is required, drawings and supporting documents can be developed and verified by suitably qualified engineers.",
                    "Discuss the access requirement, site constraints and schedule early. That gives the project team a clearer basis for selecting a system and coordinating equipment, materials and installation." 
                ]),
            ("scaffold-handover-inspection-and-tagging", "Scaffold Handover, Inspection and Tagging",
                "Clear handover and tagging processes help teams understand access status and keep inspection responsibilities visible.",
                "Safety", "https://summit.us/wp-content/uploads/2022/04/IMG_1373-scaled-e1649187636615-875x1024.jpg",
                new DateTimeOffset(2026, 10, 10, 8, 0, 0, TimeSpan.Zero),
                [
                    "Scaffold inspection, handover and tagging are identified in Summit's capability statement as part of its operating processes. These steps sit alongside planning, competent personnel, fit for purpose equipment and disciplined work methods.",
                    "A handover should make the status and intended use of the access equipment clear to the people who rely on it. Project teams should follow the applicable site procedure, identify who is responsible for inspection, and act on any change in condition or work scope through the agreed process.",
                    "Working at heights activities must be managed in accordance with applicable WHS legislation, codes of practice, client procedures and relevant Australian Standards. The statement references applicable requirements of the AS/NZS 1891 series for working at heights activities.",
                    "The practical lesson is to treat inspection, handover and tagging as part of the work plan, not as an afterthought. The specific checks and responsibilities should be confirmed for each project and site." 
                ]),
            ("planning-access-for-shutdown-work", "Planning Access for Shutdown and Maintenance Work",
                "Access needs, mobilisation, equipment and the work sequence are easier to coordinate when they are considered together.",
                "Project Planning", "https://summit.us/wp-content/uploads/2023/01/1.-OSM-Yard-61-Modular-Assembly.jpg",
                new DateTimeOffset(2026, 10, 7, 8, 0, 0, TimeSpan.Zero),
                [
                    "Shutdown and maintenance work depends on effective planning, responsive coordination and the ability to adapt to site conditions and operational priorities. Access requirements affect how crews, equipment and materials move through the work area.",
                    "An integrated scope may bring together scaffolding, scaffold and access design coordination, rigging and lifting support, inspection, tagging, workforce coordination and equipment mobilisation. Considering these needs as one sequence helps clarify interfaces and timing.",
                    "Just In Time sequencing is part of Summit's delivery model. The aim is to have the required equipment and materials available when and where they are needed, while helping reduce congestion, downtime and unnecessary material movements.",
                    "Start with the work scope and operational constraints. Confirm access points, sequence, equipment requirements, inspection and handover needs, mobilisation arrangements and the site procedures that apply before work begins." 
                ]),
            ("integrating-rigging-and-access-support", "Coordinating Rigging and Access Support",
                "When lifting and access needs meet in the same work area, early coordination helps align planning, people and equipment.",
                "Rigging", "https://summit.us/wp-content/uploads/2022/04/IMG_1679-scaled-e1649188273711.jpg",
                new DateTimeOffset(2026, 10, 4, 8, 0, 0, TimeSpan.Zero),
                [
                    "Crane and mechanical rigging often sits alongside scaffolding, access, inspection and other project support. Coordinating these activities can help teams plan around shared work areas, operational priorities and mobilisation requirements.",
                    "Summit's stated capabilities include rigging inspections and lifting support, High Risk Work Licence and competency verification support, and scaffold design and engineering coordination. The exact requirements depend on the lift, site and client procedures.",
                    "Safety planning should identify the work method, roles, equipment and relevant interfaces before execution. Summit describes a risk based approach supported by pre start planning, SWMS, JHAs, task specific risk assessments, toolbox meetings and ongoing communication.",
                    "Bring the access and lifting teams into the planning conversation early, confirm the applicable site controls, and coordinate inspection, mobilisation and work sequencing with the wider project team." 
                ]),
            ("just-in-time-materials-for-industrial-projects", "Just In Time Materials and Equipment Coordination",
                "Logistics and materials management are part of project delivery: the aim is to have the right resources available at the right time.",
                "Logistics", "https://summit.us/wp-content/uploads/2022/04/IMG_0239-scaled-e1649186987916-873x1024.jpg",
                new DateTimeOffset(2026, 10, 1, 8, 0, 0, TimeSpan.Zero),
                [
                    "Materials management is more than moving equipment to site. It connects planning, mobilisation, storage, availability and the sequence in which work is ready to proceed.",
                    "Summit's integrated service model includes logistics and materials management, equipment hire and sales, Just In Time material and equipment sequencing, demobilisation and materials reconciliation.",
                    "Coordinated sequencing aims to make the required equipment and materials available when and where they are needed. It can also help reduce congestion, downtime and unnecessary material movements, particularly where multiple work activities share a site or laydown area.",
                    "Useful planning begins with a clear scope and schedule, an understanding of site access and laydown requirements, and regular coordination as priorities change. Close out should also account for demobilisation and materials reconciliation." 
                ])
        ];

        foreach (var postData in posts)
        {
            if (await db.Posts.AnyAsync(post => post.Slug == postData.Slug)) continue;

            db.Posts.Add(new BlogPost
            {
                Slug = postData.Slug,
                Title = postData.Title,
                Excerpt = postData.Excerpt,
                Body = string.Join("\n\n", postData.Paragraphs),
                Category = postData.Category,
                AuthorName = "Summit Industrial Services",
                AuthorRole = "Capability insights",
                CoverMediaId = mediaMap.TryGetValue(postData.CoverUrl, out var mediaId) ? mediaId : null,
                Status = BlogPostStatus.Published,
                PublishedAt = postData.PublishedAt,
                IsFeatured = postData.Slug == "choosing-a-scaffolding-system"
            });
        }

        await db.SaveChangesAsync();
    }
}
