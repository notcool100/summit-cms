using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Projects.Domain;
using SummitCms.Modules.Projects.Infrastructure;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    // Client review (Oct 2026, P0-03): these entries are capability illustrations, not completed client
    // projects, and every label must make that unmistakable.
    internal const string WorkScopeStat = "Capability example, not a completed project";
    internal const string WorkScopeFactLabel = "Status";
    internal const string WorkScopeFactValue = "Typical work scope. Illustrates how our services can support a project; not an individual completed client project.";

    private static async Task<Dictionary<string, Guid>> SeedRepresentativeProjectsAsync(
        IServiceProvider sp,
        Dictionary<string, Guid> mediaMap)
    {
        var db = sp.GetRequiredService<ProjectsDbContext>();
        var categories = new Dictionary<string, Guid>();

        string[] categoryNames = ["Energy", "Resources & Minerals", "Utilities", "Renewables", "Rail", "Marine & Construction"];
        for (var index = 0; index < categoryNames.Length; index++)
        {
            var category = await db.IndustryCategories.FirstOrDefaultAsync(c => c.Name == categoryNames[index]);
            if (category is null)
            {
                category = new ProjectIndustryCategory { Name = categoryNames[index], DisplayOrder = index };
                db.IndustryCategories.Add(category);
            }

            category.DisplayOrder = index;
            categories[category.Name] = category.Id;
        }
        await db.SaveChangesAsync();

        (string Slug, string Name, string Industry, string MediaUrl, string Alt, string Figure,
            (string Label, string Value)[] Facts,
            (string Title, string First, string Second)[] Sections)[] profiles =
        [
            ("representative-scaffolding-access", "Scaffolding & access work scope", "Resources & Minerals",
                Stock("structural-access-frames"), "Scaffolding access structure", "TUBE & FIT / AT-PAC RINGLOCK",
                [(WorkScopeFactLabel, WorkScopeFactValue), ("Base", "Karratha Industrial Estate"), ("Service area", "Greater Pilbara Region")],
                [("Access requirements", "Tube & Fit and AT-PAC Ringlock systems support access needs across projects of varying size and complexity.", "Summit provides scaffold erection, modification and dismantling, with coordinated equipment supply, logistics and materials management."),
                 ("Delivery approach", "Each scope is tailored to site conditions, operational priorities and project schedules.", "Where required, scaffold design and engineering coordination can support complex structures and specialised access requirements.")]),
            ("representative-rigging-lifting", "Crane & mechanical rigging work scope", "Energy",
                Stock("pick-and-carry-crane"), "Rigging and lifting support", "CRANE & MECHANICAL RIGGING",
                [(WorkScopeFactLabel, WorkScopeFactValue), ("Base", "Karratha Industrial Estate"), ("Service area", "Greater Pilbara Region")],
                [("Rigging support", "Crane and mechanical rigging services support maintenance, shutdown, construction, marine and project works.", "Rigging inspections and lifting support are coordinated with project requirements and site procedures."),
                 ("Safety and competency", "Work is planned around competent personnel, fit for purpose equipment and disciplined work methods.", "High Risk Work Licence and competency verification support forms part of Summit's safety and compliance approach.")]),
            ("representative-shutdown-maintenance", "Shutdown & maintenance support work scope", "Utilities",
                Stock("shutdown-process-facility"), "Industrial maintenance access support", "SHUTDOWN & MAINTENANCE SUPPORT",
                [(WorkScopeFactLabel, WorkScopeFactValue), ("Base", "Karratha Industrial Estate"), ("Service area", "Greater Pilbara Region")],
                [("Coordinated scope", "Scaffolding, access and rigging support can be brought together for shutdown and maintenance requirements.", "Project planning, mobilisation, workforce and equipment coordination help align resources with operational priorities."),
                 ("Integrated delivery", "Inspection, tagging, lifting support, logistics and materials management contribute to a coordinated service model.", "Just In Time sequencing aims to make the required equipment and materials available when and where they are needed.")]),
            ("representative-marine-construction", "Marine & construction access work scope", "Marine & Construction",
                Stock("marine-wharf-cranes"), "Marine and construction infrastructure", "MARINE & INDUSTRIAL CONSTRUCTION",
                [(WorkScopeFactLabel, WorkScopeFactValue), ("Base", "Karratha Industrial Estate"), ("Service area", "Greater Pilbara Region")],
                [("Access and rigging", "Summit supports ports, marine infrastructure and industrial construction environments with scaffolding, access and rigging capability.", "Work methods and equipment are selected to suit the operational environment and project requirements."),
                 ("Planning and coordination", "Mobilisation, logistics and materials management help coordinate people and equipment.", "Inspection and handover processes support safe, practical access throughout the work scope.")]),
            ("representative-renewables-access", "Renewables access support work scope", "Renewables",
                Stock("renewables-platform-crew"), "Renewable energy project structure", "RENEWABLE ENERGY PROJECT SUPPORT",
                [(WorkScopeFactLabel, WorkScopeFactValue), ("Base", "Karratha Industrial Estate"), ("Service area", "Greater Pilbara Region")],
                [("Project support", "Scaffolding, access and rigging support is available for renewable energy and emerging energy projects.", "Tube & Fit or modular access options can be selected to suit site conditions and project requirements."),
                 ("Coordinated resources", "Planning, equipment supply and logistics support mobilisation and project delivery.", "Working at heights equipment inspection, scaffold handover and tagging processes form part of the broader service capability.")])
        ];

        var ids = new Dictionary<string, Guid>();
        for (var index = 0; index < profiles.Length; index++)
        {
            var profile = profiles[index];
            var categoryId = categories[profile.Industry];
            var project = await db.Projects.FirstOrDefaultAsync(p => p.Slug == profile.Slug);
            if (project is null)
            {
                project = new Project { Slug = profile.Slug };
                db.Projects.Add(project);
            }

            project.Name = profile.Name;
            project.IndustryCategoryId = categoryId;
            project.Stat = WorkScopeStat;
            project.IsFeatured = true;
            project.DisplayOrder = index;
            if (project.HeroMediaId is null || IsThirdPartyMedia(project.HeroMediaId)) project.HeroMediaId = mediaMap[profile.MediaUrl];
            project.Ratio = index % 2 == 0 ? "16/10" : "4/5";
            project.Span = index == 0 ? 7 : 5;
            ids[profile.Slug] = project.Id;
            await db.SaveChangesAsync();

            var existingFacts = await db.ScopeFacts.Where(f => f.ProjectId == project.Id).ToListAsync();
            var existingSections = await db.NarrativeSections.Where(s => s.ProjectId == project.Id).ToListAsync();
            if (existingFacts.Count > 0 || existingSections.Count > 0) continue;

            for (var factIndex = 0; factIndex < profile.Facts.Length; factIndex++)
            {
                var fact = profile.Facts[factIndex];
                db.ScopeFacts.Add(new ProjectScopeFact
                {
                    ProjectId = project.Id,
                    Label = fact.Label,
                    Value = fact.Value,
                    DisplayOrder = factIndex
                });
            }

            for (var sectionIndex = 0; sectionIndex < profile.Sections.Length; sectionIndex++)
            {
                var sectionData = profile.Sections[sectionIndex];
                var section = new ProjectNarrativeSection
                {
                    ProjectId = project.Id,
                    Idx = (sectionIndex + 1).ToString("D2"),
                    Title = sectionData.Title,
                    DisplayOrder = sectionIndex
                };
                section.Paragraphs.Add(new ProjectNarrativeParagraph { ParagraphOrder = 0, Body = sectionData.First });
                section.Paragraphs.Add(new ProjectNarrativeParagraph { ParagraphOrder = 1, Body = sectionData.Second });
                db.NarrativeSections.Add(section);
            }

            db.GalleryImages.Add(new ProjectGalleryImage
            {
                ProjectId = project.Id,
                MediaId = mediaMap[profile.MediaUrl],
                Role = ProjectImageRole.Break,
                Caption = $"ILLUSTRATIVE IMAGE · {profile.Figure}",
                DisplayOrder = 0
            });
            await db.SaveChangesAsync();
        }

        return ids;
    }
}
