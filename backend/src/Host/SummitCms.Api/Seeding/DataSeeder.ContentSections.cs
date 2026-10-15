using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Company.Domain;
using SummitCms.Modules.Company.Infrastructure;
using SummitCms.Shared.Kernel.Common;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private sealed record SeedItem(string Title, string Body = "", string Subtitle = "", string Detail = "", string Tags = "");

    private sealed record SeedSection(
        string Key, string PageSlug, string Slot, string Layout, string Eyebrow, string Heading, string Intro, string Note,
        int Columns, bool Numbered, bool Tinted, SeedItem[] Items);

    /// <summary>
    /// Seeds the configurable page sections from the 2026 Capability Statement. Inserts only sections whose Key
    /// does not exist yet, so admin edits (and deletions of items within a section) are never overwritten.
    /// </summary>
    private static async Task SeedContentSectionsAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<CompanyDbContext>();
        var existingKeys = await db.ContentSections.Select(s => s.Key).ToListAsync();

        SeedItem[] whySummit =
        [
            new("Pilbara based and locally responsive", "Operating from the Karratha Industrial Estate, close to the sites we support."),
            new("50+ years of combined experience", "A team that has worked through shutdowns, maintenance campaigns and construction projects."),
            new("Integrated capability", "Scaffolding, access and rigging delivered together, under one coordinated plan."),
            new("Safety focused execution", "Planning, competent people and disciplined methods behind every task."),
            new("Responsive mobilisation", "Practical problem solving when site conditions and priorities change."),
            new("Engineering and design support", "Coordination with qualified professionals for complex access requirements."),
            new("Shutdown to marine experience", "Experience across shutdown, maintenance, marine and project environments."),
            new("Inspection and logistics depth", "Inspection, tagging, logistics and materials management in house.")
        ];

        SeedSection[] sections =
        [
            new("home-why-summit", PageSlugs.Home, "before-safety", ContentLayouts.Cards, "Why Summit", "Local. Integrated.\nSafety-focused.",
                "Summit is a Pilbara based specialist access and rigging provider with more than 50 years of combined industry experience.", "", 4, true, false, whySummit),

            new("about-facts", PageSlugs.About, "after-hero", ContentLayouts.Facts, "Company at a glance", "", "", "", 4, false, false,
            [
                new("Legal entity", "Summit Industrial Services Pty Ltd"),
                new("ABN", "41 693 462 421"),
                new("Base", "Karratha Industrial Estate, WA"),
                new("Coverage", "Greater Pilbara region"),
                new("Experience", "50+ years combined"),
                new("Accreditation", "Working towards ISO")
            ]),

            new("about-why-summit", PageSlugs.About, "before-leadership", ContentLayouts.Cards, "Why Summit", "Why clients\nchoose Summit.",
                "Local presence, integrated capability and a safety first approach to every task.", "", 4, true, false, whySummit),

            new("about-commitment", PageSlugs.About, "before-leadership", ContentLayouts.Quote, "Our commitment", "",
                "Summit Industrial Services is committed to building long term client relationships through dependable service, transparent communication and consistently high standards of workmanship. We aim to become a trusted delivery partner by understanding our clients' operational requirements and providing safe, practical solutions that support productivity and project performance.",
                "One team. Integrated solutions.", 4, false, true, []),

            new("about-key-personnel", PageSlugs.About, "before-leadership", ContentLayouts.People, "Key personnel", "", "", "", 4, false, false,
            [
                new("Jason Matakatea", "As Founder and Managing Director, Jason leads the strategic direction and overall management of Summit Industrial Services. He works closely with clients and the leadership team to guide business growth, operational performance and the delivery of safe, reliable access and rigging solutions across the Pilbara and wider Western Australian market.",
                    "Managing Director", "Dual licensed rigger scaffolder, 25+ years experience", "Leadership\nBusiness Strategy\nClient Relationships\nOperations\nProject Delivery"),
                new("Rebecca Neumann", "Rebecca is a Director of Summit Industrial Services and oversees the company's accounts and financial administration functions. She supports accounts management, supplier and client coordination, commercial administration and the broader development of the business. Rebecca spent many years working closely with Indigenous families and communities across Karratha, Roebourne and the wider Pilbara.",
                    "Director & Accounts", "Degree in social science, masters in learning and teaching in higher education", "Directorship\nAccounts & Financial Administration\nCommercial Support\nBusiness Administration\nClient & Supplier Coordination"),
                new("Travis Kuru", "Travis oversees operational delivery across the North West, supporting the safe and efficient execution of scaffolding, access, rigging, shutdown, maintenance and project works. Working closely with clients, site teams and management, he supports workforce coordination, mobilisation, project planning and day to day execution.",
                    "North West Operations Manager", "Dual licensed rigger scaffolder, 10+ years experience", "Operations Management\nProject Delivery\nWorkforce Coordination\nClient Liaison\nSafety Leadership"),
                new("Nikki Sharpe", "Nikki supports Summit's planning and scheduling functions, coordinating people, equipment, materials and project requirements to support efficient and reliable service delivery. Her role includes forward planning, resource scheduling, mobilisation and coordination of changing operational priorities.",
                    "Planning & Scheduling", "", "Planning & Scheduling\nResource Coordination\nMobilisation\nProject Support\nMaterials Coordination")
            ]),

            new("capabilities-core", PageSlugs.Capabilities, "before-band", ContentLayouts.Cards, "Core capabilities", "Everything on site,\nfrom one team.",
                "Twelve core capabilities covering access, rigging, inspection, engineering coordination and supply.", "", 4, true, false,
            [
                new("Tube & Fit scaffolding systems"), new("AT-PAC Ringlock scaffolding systems"),
                new("Scaffold erection, modification and dismantling"), new("Scaffolding equipment hire and sales"),
                new("Crane and mechanical rigging"), new("Rigging inspections and lifting support"),
                new("Scaffold design and engineering coordination"), new("Working at heights equipment inspection and tagging"),
                new("Logistics and materials management"), new("Shutdown, maintenance and project support"),
                new("High Risk Work Licence and competency verification support"), new("Integrated access and rigging solutions")
            ]),

            new("capabilities-safety", PageSlugs.Capabilities, "before-band", ContentLayouts.Cards, "Safety & compliance", "Every person goes home\nsafely, every day.",
                "A risk based approach to protecting our people, clients and assets, built on planning, competent personnel, fit for purpose equipment and disciplined work methods.",
                "Working at heights activities are managed in accordance with applicable WHS legislation, codes of practice, client procedures and relevant Australian Standards, including applicable requirements of the AS/NZS 1891 series.",
                4, false, true,
            [
                new("Pre start planning", "Pre start planning, SWMS, JHAs and task specific risk assessments before work begins."),
                new("Licences and competency", "High Risk Work Licence, competency and VOC verification for the people on site."),
                new("Inspection and handover", "Scaffold inspection, handover and tagging processes that keep every structure accountable."),
                new("Equipment inspection", "Working at heights equipment inspection and tagging, so gear is fit for purpose."),
                new("Fall arrest and rescue", "Fall arrest, lifeline and retrieval or rescue provisions where required."),
                new("Engineering verification", "Engineering and design verification where the structure or task calls for it."),
                new("Consultation", "Toolbox meetings, consultation and ongoing safety communication with crews and clients."),
                new("Continuous improvement", "Reporting, review and learning that feed back into how we plan the next job.")
            ]),

            new("capabilities-delivery", PageSlugs.Capabilities, "before-band", ContentLayouts.Cards, "Integrated delivery", "From mobilisation\nto demobilisation.",
                "Just In Time sequencing and coordinated mobilisation help the right equipment, materials and resources arrive when and where they are needed, reducing congestion, downtime and unnecessary material movements.",
                "", 3, true, false,
            [
                new("Project planning and mobilisation"), new("Scaffold and access design coordination"),
                new("Rigging and lifting support"), new("Just In Time material and equipment sequencing"),
                new("Logistics and materials management"), new("Workforce and equipment coordination"),
                new("Shutdown, maintenance and project execution"), new("Inspection, tagging and compliance support"),
                new("Demobilisation and materials reconciliation")
            ]),

            new("contact-details", PageSlugs.Contact, "details", ContentLayouts.Facts, "Contact details", "", "", "", 2, false, false,
            [
                new("Jason Matakatea, Managing Director", "0401 174 989\nJason@summit-is.com.au"),
                new("Service area", "Greater Pilbara region, Western Australia")
            ]),

            new("contact-faq", PageSlugs.Contact, "faq", ContentLayouts.Faq, "FAQ", "Before you call.", "", "", 2, false, false,
            [
                new("What services does Summit provide?", "Summit provides Tube & Fit and AT-PAC Ringlock scaffolding, scaffold erection and dismantling, equipment hire and sales, crane and mechanical rigging, inspections, engineering coordination, logistics, and shutdown, maintenance and project support."),
                new("How does Summit approach safety?", "Safety is fundamental to our work. We use pre start planning, SWMS, JHAs and task specific risk assessments, competent personnel, fit for purpose equipment, scaffold handover and tagging processes, and applicable WHS legislation, codes, client procedures and Australian Standards."),
                new("Can Summit support scaffold design?", "We provide tailored scaffold design and engineering coordination. Where engineered design is required, drawings and supporting documentation can be developed and verified by suitably qualified engineers."),
                new("Where does Summit work?", "Summit is based in the Karratha Industrial Estate and services the Greater Pilbara region, supporting clients across energy, resources and minerals, utilities, renewables, rail, marine and construction."),
                new("Which scaffolding systems do you work with?", "We work with traditional Tube & Fit systems and modern modular options including the AT-PAC Ringlock System, tailored to site conditions, operational priorities and project schedules."),
                new("Do you offer equipment hire and sales?", "Yes. Scaffolding equipment hire and sales are supported by our logistics and materials management, including Just In Time sequencing, demobilisation and materials reconciliation."),
                new("Can you verify licences and competencies?", "We support High Risk Work Licence, competency and VOC verification, and carry out working at heights equipment inspection and tagging."),
                new("Does Summit hold ISO accreditation?", "Summit is working towards ISO accreditation as part of our commitment to strengthening safety, quality and continuous improvement systems."),
                new("How can I contact Summit?", "Send an enquiry using the form on this page, call 0401 174 989, or email support@summit-is.com.au.")
            ]),

            // ---- Additional sections (all content drawn from the 2026 Capability Statement) ----
            new("home-manifesto", PageSlugs.Home, "manifesto", ContentLayouts.Cards, "Manifesto", "",
                "Summit Industrial Services is a Pilbara based specialist access and rigging provider. Our team brings more than 50 years of combined industry experience to maintenance, shutdown, construction, marine and project works.\nWe tailor our approach to site conditions, operational priorities and project schedules, supported by coordinated logistics, materials management and equipment supply.",
                "Every person goes home safely, every day.", 2, false, false,
            [
                new("Scaffolding and access", "Tube & Fit and AT-PAC Ringlock systems, erected, modified and dismantled safely and efficiently."),
                new("Rigging and lifting", "Crane and mechanical rigging, rigging inspections and lifting support."),
                new("Inspection and tagging", "Scaffold handover and tagging, plus working at heights equipment inspection."),
                new("Engineering coordination", "Design and engineering coordination for complex structures and specialised access."),
                new("Logistics and materials", "Equipment hire and sales, Just In Time sequencing and materials management."),
                new("Shutdown and project support", "Planning, mobilisation and workforce coordination across the Greater Pilbara.")
            ]),

            new("home-services-overview", PageSlugs.Home, "after-stats", ContentLayouts.Cards, "What we do", "Four ways we\nkeep sites moving.",
                "From traditional Tube & Fit systems to modern modular options, backed by engineering, safety and logistics.", "", 4, true, false,
            [
                new("Scaffolding & Access Solutions", "Tube & Fit and AT-PAC Ringlock systems, with safe and efficient installation, modification, dismantling and ongoing support."),
                new("Scaffold Design & Engineering", "Tailored design and engineering coordination for complex structures and specialised access requirements."),
                new("Rigging & Lifting", "Crane and mechanical rigging, rigging inspections and lifting support, backed by licence and competency verification."),
                new("Inspection, Logistics & Support", "Working at heights equipment inspection and tagging, logistics, materials management and shutdown support.")
            ]),

            new("home-how-we-deliver", PageSlugs.Home, "after-industries", ContentLayouts.Cards, "How we deliver", "One plan,\nfrom start to finish.",
                "Our integrated service model brings scaffolding, access, rigging, inspection, engineering support, logistics and materials management together.", "", 3, true, true,
            [
                new("Plan and mobilise", "Project planning and mobilisation, with scaffold, access and rigging design coordination up front."),
                new("Execute on site", "Workforce and equipment coordination across shutdown, maintenance and project execution."),
                new("Sequence Just In Time", "Material and equipment sequencing so the right resources arrive when and where they are needed."),
                new("Inspect and tag", "Inspection, tagging and compliance support before, during and after the work."),
                new("Supply and manage", "Logistics, materials management and equipment hire and sales."),
                new("Demobilise and reconcile", "Demobilisation and materials reconciliation to close the job out cleanly.")
            ]),

            new("home-commitment", PageSlugs.Home, "after-standard", ContentLayouts.Quote, "Our commitment", "",
                "We aim to become a trusted delivery partner by understanding our clients' operational requirements and providing safe, practical solutions that support productivity and project performance.",
                "Built on trust, driven by excellence.", 4, false, false, []),

            new("home-leadership-contacts", PageSlugs.Home, "after-standard", ContentLayouts.Cards, "Who you will deal with", "Experienced people,\nclose to the work.",
                "Our management and support team provides the operational coordination, planning and commercial support behind every project.", "", 4, false, false,
            [
                new("Jason Matakatea, Managing Director", "Founder and dual licensed rigger scaffolder with 25+ years experience. Leads strategy, client relationships and project delivery."),
                new("Rebecca Neumann, Director & Accounts", "Oversees accounts, financial administration, commercial support and client and supplier coordination."),
                new("Travis Kuru, North West Operations Manager", "Dual licensed rigger scaffolder with 10+ years experience. Oversees operational delivery across the North West."),
                new("Nikki Sharpe, Planning & Scheduling", "Coordinates people, equipment, materials and project requirements through forward planning and resource scheduling.")
            ]),

            new("about-industries", PageSlugs.About, "end", ContentLayouts.Cards, "Industries we serve", "Six sectors,\none integrated team.",
                "Based in the Karratha Industrial Estate, Summit supports clients across the Greater Pilbara region.", "", 3, true, false,
            [
                new("Energy", "Access and rigging support for energy infrastructure, maintenance, construction and project activities."),
                new("Resources & Minerals", "Specialist services supporting mining and mineral processing operations, shutdowns, maintenance and projects."),
                new("Utilities", "Practical access solutions supporting essential infrastructure, upgrades, maintenance and construction."),
                new("Renewables", "Scaffolding, access and rigging support for renewable energy and emerging energy projects."),
                new("Rail", "Specialised access and rigging solutions supporting rail infrastructure, maintenance, upgrades and projects."),
                new("Marine & Construction", "Support for ports, marine infrastructure and industrial construction environments.")
            ]),

            new("about-service-model", PageSlugs.About, "end", ContentLayouts.Cards, "Our service model", "Everything\nwe bring to site.",
                "Successful delivery takes more than skilled people and equipment. It takes planning, coordination and the ability to adapt quickly.", "", 4, false, true,
            [
                new("Scaffolding and access", "Tube & Fit and AT-PAC Ringlock systems, erected, modified and dismantled to suit the site."),
                new("Rigging and lifting", "Crane and mechanical rigging, rigging inspections and lifting support."),
                new("Inspection and engineering", "Equipment inspection and tagging, plus design and engineering coordination."),
                new("Logistics and materials", "Equipment hire and sales, Just In Time sequencing and materials management.")
            ]),

            new("capabilities-systems", PageSlugs.Capabilities, "after-hero", ContentLayouts.Cards, "Scaffolding systems", "Traditional and\nmodular, side by side.",
                "We tailor our approach to site conditions, operational priorities and project schedules.", "", 4, false, false,
            [
                new("Tube & Fit", "Traditional scaffolding systems suited to varied structures and site conditions."),
                new("AT-PAC Ringlock", "A modern modular system for efficient, safe installation."),
                new("Erect, modify, dismantle", "Installation, modification and dismantling for projects of varying size and complexity."),
                new("Hire and sales", "Scaffolding equipment hire and sales, backed by coordinated logistics and materials supply.")
            ]),

            new("capabilities-engineering", PageSlugs.Capabilities, "before-band", ContentLayouts.Cards, "Design & engineering", "Documented, verified,\nsite-ready.",
                "Working with suitably qualified professionals, Summit develops practical solutions for complex structures, challenging work environments and specialised access requirements.",
                "Engineered design is developed and verified by suitably qualified engineers in accordance with applicable Australian Standards, regulatory obligations and site requirements.", 4, false, true,
            [
                new("Detailed drawings", "Drawings prepared where engineered design is required."),
                new("Plans", "Project specific access plans for complex structures."),
                new("Supporting documentation", "Documentation that supports approvals, handover and compliance."),
                new("Engineer verification", "Verification by suitably qualified engineers.")
            ]),

            new("capabilities-rigging", PageSlugs.Capabilities, "before-band", ContentLayouts.Cards, "Rigging & lifting", "Licensed people.\nVerified competency.",
                "Crane and mechanical rigging is supported by licence and competency verification.", "", 4, false, false,
            [
                new("Crane rigging", "Crane rigging for lifting and positioning on site."),
                new("Mechanical rigging", "Mechanical rigging for maintenance, shutdown and construction works."),
                new("Rigging inspections", "Inspection of rigging equipment and lifting support."),
                new("Licence verification", "High Risk Work Licence, competency and VOC verification support.")
            ]),

            new("industries-facts", PageSlugs.Industries, "after-hero", ContentLayouts.Facts, "Coverage", "", "", "", 4, false, false,
            [
                new("Sectors", "Six industry groupings"),
                new("Region", "Greater Pilbara, Western Australia"),
                new("Base", "Karratha Industrial Estate"),
                new("Approach", "Integrated access and rigging")
            ]),

            new("industries-support", PageSlugs.Industries, "before-cta", ContentLayouts.Cards, "Across every sector", "The same standard,\nwherever we work.",
                "Whatever the sector, our teams combine planning, competent personnel, fit for purpose equipment and disciplined work methods.", "", 3, true, false,
            [
                new("Maintenance", "Access, rigging and inspection support for ongoing maintenance activities."),
                new("Shutdowns", "Coordinated mobilisation and Just In Time sequencing for planned shutdowns."),
                new("Construction", "Scaffolding, access and rigging for industrial construction environments."),
                new("Marine", "Support for ports and marine infrastructure."),
                new("Projects", "Planning, workforce coordination and execution for project works."),
                new("Upgrades", "Practical access solutions for upgrades to essential infrastructure.")
            ]),

            new("projects-how-it-works", PageSlugs.Projects, "end", ContentLayouts.Cards, "How a project runs", "From first call\nto demobilisation.",
                "A simple path from enquiry to a closed out job.", "", 4, true, false,
            [
                new("Discuss scope", "Tell us about the site, the work and the schedule."),
                new("Plan and design", "Scaffold, access and rigging design coordination, with engineering where required."),
                new("Mobilise and execute", "Safe, coordinated delivery with inspection, tagging and compliance support."),
                new("Demobilise", "Demobilisation and materials reconciliation to finish the job.")
            ]),

            new("projects-energy", PageSlugs.Projects, "category:Energy", ContentLayouts.Cards, "How an energy project runs", "Energy infrastructure,\nfrom enquiry to close out.",
                "", "", 4, true, false,
            [
                new("Discuss scope", "Access and rigging support for energy infrastructure, maintenance, construction and project activities."),
                new("Plan and design", "Scaffold, access and rigging design coordination, with engineering where required."),
                new("Mobilise and execute", "Supporting operational facilities, planned shutdowns and construction phases."),
                new("Demobilise", "Demobilisation and materials reconciliation to finish the job.")
            ]),

            new("projects-resources", PageSlugs.Projects, "category:Resources & Minerals", ContentLayouts.Cards, "How a resources and minerals project runs", "Mining and processing,\nfrom enquiry to close out.",
                "", "", 4, true, false,
            [
                new("Discuss scope", "Tell us about the operation, the shutdown or maintenance window and the schedule."),
                new("Plan and design", "Integrated scaffolding, access and rigging solutions coordinated with engineering where required."),
                new("Mobilise and execute", "Safe, coordinated delivery where safety, reliability and schedule adherence are critical."),
                new("Demobilise", "Demobilisation and materials reconciliation to finish the job.")
            ]),

            new("projects-utilities", PageSlugs.Projects, "category:Utilities", ContentLayouts.Cards, "How a utilities project runs", "Essential infrastructure,\nfrom enquiry to close out.",
                "", "", 4, true, false,
            [
                new("Discuss scope", "Tell us about the infrastructure, the upgrade or maintenance and the schedule."),
                new("Plan and design", "Practical access solutions planned around the existing infrastructure."),
                new("Mobilise and execute", "Coordinated execution that aims to keep disruption to a minimum."),
                new("Demobilise", "Demobilisation and materials reconciliation to finish the job.")
            ]),

            new("projects-renewables", PageSlugs.Projects, "category:Renewables", ContentLayouts.Cards, "How a renewables project runs", "Renewable projects,\nfrom enquiry to close out.",
                "", "", 4, true, false,
            [
                new("Discuss scope", "Tell us about the renewable or emerging energy project and its construction method."),
                new("Plan and design", "Adaptable scaffolding, access and rigging solutions for evolving technologies."),
                new("Mobilise and execute", "Coordinated delivery with inspection, tagging and compliance support."),
                new("Demobilise", "Demobilisation and materials reconciliation to finish the job.")
            ]),

            new("projects-rail", PageSlugs.Projects, "category:Rail", ContentLayouts.Cards, "How a rail project runs", "Rail infrastructure,\nfrom enquiry to close out.",
                "", "", 4, true, false,
            [
                new("Discuss scope", "Tell us about the rail infrastructure, the upgrade or maintenance and the schedule."),
                new("Plan and design", "Specialised access and rigging solutions, with engineering where required."),
                new("Mobilise and execute", "Safe, coordinated delivery with inspection, tagging and compliance support."),
                new("Demobilise", "Demobilisation and materials reconciliation to finish the job.")
            ]),

            new("projects-marine", PageSlugs.Projects, "category:Marine & Construction", ContentLayouts.Cards, "How a marine and construction project runs", "Ports and construction,\nfrom enquiry to close out.",
                "", "", 4, true, false,
            [
                new("Discuss scope", "Tell us about the port, marine structure or construction site and the schedule."),
                new("Plan and design", "Scaffolding, access and rigging capability planned around the marine and industrial environment."),
                new("Mobilise and execute", "Coordinated delivery with inspection, tagging and compliance support."),
                new("Demobilise", "Demobilisation and materials reconciliation to finish the job.")
            ]),

            new("insights-safety-principles", PageSlugs.Insights, "category:Safety", ContentLayouts.Cards, "How we think about safety", "Every person goes home\nsafely, every day.",
                "Working at heights, scaffolding, rigging and complex access carry inherent risk. We apply a risk based approach to protecting our people, clients and assets.", "", 4, false, false,
            [
                new("Planning", "Pre start planning, SWMS, JHAs and task specific risk assessments."),
                new("Competent people", "High Risk Work Licence, competency and VOC verification."),
                new("Fit for purpose equipment", "Inspection and tagging of scaffold and working at heights equipment."),
                new("Disciplined methods", "Handover processes, toolbox meetings and ongoing safety communication.")
            ]),

            new("insights-overview", PageSlugs.Insights, "end", ContentLayouts.Cards, "What we write about", "Notes from the field,\nby topic.",
                "Pick a topic above to see how Summit approaches it.", "", 4, false, false,
            [
                new("Scaffolding", "Tube & Fit and AT-PAC Ringlock systems, from installation to dismantling."),
                new("Safety", "Risk based planning, competent people and disciplined work methods."),
                new("Project Planning", "Mobilisation, sequencing and coordination for shutdown and maintenance."),
                new("Rigging and Logistics", "Lifting support, equipment supply and Just In Time materials.")
            ]),

            new("insights-scaffolding", PageSlugs.Insights, "category:Scaffolding", ContentLayouts.Cards, "How we think about scaffolding", "Right system.\nRight structure.",
                "We tailor our approach to site conditions, operational priorities and project schedules.", "", 4, false, false,
            [
                new("Tube & Fit", "Traditional scaffolding systems suited to varied structures and site conditions."),
                new("AT-PAC Ringlock", "A modern modular system for efficient, safe installation."),
                new("Erect, modify, dismantle", "Installation, modification and dismantling for projects of varying size and complexity."),
                new("Hire and sales", "Scaffolding equipment hire and sales, supported by logistics and materials management.")
            ]),

            new("insights-rigging", PageSlugs.Insights, "category:Rigging", ContentLayouts.Cards, "How we think about rigging", "Licensed people.\nCoordinated lifts.",
                "Crane and mechanical rigging often sits alongside scaffolding, access and inspection.", "", 4, false, false,
            [
                new("Crane and mechanical rigging", "Rigging for lifting and positioning across maintenance, shutdown and construction works."),
                new("Rigging inspections", "Inspection of rigging equipment and lifting support."),
                new("Licence verification", "High Risk Work Licence, competency and VOC verification support."),
                new("Early coordination", "Bringing access and lifting teams into the planning conversation together.")
            ]),

            new("insights-project-planning", PageSlugs.Insights, "category:Project Planning", ContentLayouts.Cards, "How we think about planning", "Plan the work.\nSequence the access.",
                "Effective planning, responsive coordination and the ability to adapt to changing site conditions.", "", 4, false, false,
            [
                new("Project planning and mobilisation", "Setting up people, equipment and materials before work begins."),
                new("Design coordination", "Scaffold and access design coordination, with engineering where required."),
                new("Workforce coordination", "Coordinating crews and equipment as priorities change."),
                new("Just In Time sequencing", "Having equipment and materials available when and where they are needed.")
            ]),

            new("insights-logistics", PageSlugs.Insights, "category:Logistics", ContentLayouts.Cards, "How we think about logistics", "The right resources,\nat the right time.",
                "Logistics and materials management are part of project delivery, not an afterthought.", "", 4, false, false,
            [
                new("Materials management", "Connecting planning, mobilisation and availability."),
                new("Hire and sales", "Scaffolding equipment hire and sales."),
                new("Just In Time sequencing", "Reducing congestion, downtime and unnecessary material movements."),
                new("Demobilisation", "Demobilisation and materials reconciliation to close out the job.")
            ]),

            new("contact-who-to-call", PageSlugs.Contact, "before-faq", ContentLayouts.Cards, "Who to talk to", "The right person\nfor your question.",
                "", "", 4, false, false,
            [
                new("Project and commercial enquiries", "Jason Matakatea, Managing Director. 0401 174 989, Jason@summit-is.com.au."),
                new("Operations and mobilisation", "Travis Kuru, North West Operations Manager."),
                new("Planning and scheduling", "Nikki Sharpe, Planning & Scheduling."),
                new("Accounts and suppliers", "Rebecca Neumann, Director & Accounts.")
            ]),

            new("contact-enquiry-checklist", PageSlugs.Contact, "before-faq", ContentLayouts.Cards, "Helpful to include", "What to tell us\nin your enquiry.",
                "A few details help us respond faster and plan the right resources.", "", 4, true, true,
            [
                new("Site and location", "Where the work is and any access constraints."),
                new("Scope of work", "Scaffolding, rigging, inspection, engineering support or a combination."),
                new("Timing", "Start date, duration and any shutdown windows."),
                new("Standards and procedures", "Client procedures or specific requirements we should plan around.")
            ])
        ];

        // Insights blocks became category specific: move the original Safety block off the shared "end" slot.
        var legacySafety = await db.ContentSections.FirstOrDefaultAsync(c => c.Key == "insights-safety-principles" && c.Slot == "end");
        if (legacySafety is not null) legacySafety.Slot = "category:Safety";

        var order = await db.ContentSections.AnyAsync() ? await db.ContentSections.MaxAsync(s => s.DisplayOrder) + 1 : 0;
        foreach (var s in sections)
        {
            if (existingKeys.Contains(s.Key)) continue;

            var section = new ContentSection
            {
                Key = s.Key, PageSlug = s.PageSlug, Slot = s.Slot, Layout = s.Layout, Eyebrow = s.Eyebrow, Heading = s.Heading,
                Intro = s.Intro, Note = s.Note, Columns = s.Columns, Numbered = s.Numbered, Tinted = s.Tinted,
                IsActive = true, DisplayOrder = order++
            };
            for (var i = 0; i < s.Items.Length; i++)
            {
                var it = s.Items[i];
                section.Items.Add(new ContentItem { Title = it.Title, Body = it.Body, Subtitle = it.Subtitle, Detail = it.Detail, Tags = it.Tags, DisplayOrder = i });
            }
            db.ContentSections.Add(section);
        }

        await db.SaveChangesAsync();
    }
}
