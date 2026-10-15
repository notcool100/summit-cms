using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SummitCms.Modules.Capabilities.Domain;
using SummitCms.Modules.Capabilities.Infrastructure;

namespace SummitCms.Api.Seeding;

public static partial class DataSeeder
{
    private static async Task SeedCapabilitiesAsync(IServiceProvider sp, Dictionary<string, Guid> mediaMap)
    {
        var db = sp.GetRequiredService<CapabilitiesDbContext>();
        Guid Media(string url) => mediaMap[url];

        (string Key, string Name, string Tag, string Body, string Stat, string StatLabel, CapabilityBackground Bg, bool TextFirst, string Url, string Fig)[] rows =
        [
            ("scaffolding-access", "Scaffolding & Access Solutions", "Traditional and modular systems for diverse access needs",
                "Summit Industrial Services provides a diverse range of high quality scaffolding and access solutions, from traditional Tube & Fit systems through to modern modular options including the AT-PAC Ringlock System. Our experienced team provides safe and efficient installation, modification, dismantling and ongoing support for projects of varying size and complexity. We tailor our approach to site conditions, operational priorities and project schedules, supported by coordinated logistics, materials management and equipment supply.",
                "Tube & Fit", "AT-PAC Ringlock", CapabilityBackground.Paper, true,
                "https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg", "TUBE & FIT / RINGLOCK SYSTEMS"),
            ("scaffold-design", "Scaffold Design & Engineering", "Engineered solutions for complex structures",
                "We provide tailored scaffold design and engineering coordination to meet project specific access requirements. Working with suitably qualified professionals, Summit develops practical solutions for complex structures, challenging work environments and specialised access requirements. Where engineered design is required, detailed drawings, plans and supporting documentation can be developed and verified by suitably qualified engineers in accordance with applicable Australian Standards, regulatory obligations and site requirements.",
                "Design", "Engineering coordination", CapabilityBackground.Panel, false,
                "https://summit.us/wp-content/uploads/2022/05/Intel-Rendering-PNG-edited.png", "ENGINEERED DESIGN COORDINATION"),
            ("rigging", "Crane & Mechanical Rigging", "Licensed rigging operations with competency verification",
                "Crane and mechanical rigging operations supported by High Risk Work Licence holders and competency verification. Our teams combine planning, competent personnel, fit for purpose equipment and disciplined work methods. Rigging inspections and lifting support are managed in accordance with applicable WHS legislation, codes of practice, client procedures and relevant Australian Standards.",
                "Rigging", "Inspection & lifting support", CapabilityBackground.Paper, true,
                "https://summit.us/wp-content/uploads/2022/04/IMG_1679-scaled-e1649188273711.jpg", "PRECISION RIGGING OPERATIONS"),
            ("inspection-tagging", "Inspection, Tagging & Compliance", "Height safety equipment and scaffold inspection",
                "We provide working at heights equipment inspection and tagging, together with scaffold inspection, handover and tagging processes. Activities are managed in accordance with applicable WHS legislation, client procedures and relevant Australian Standards, including applicable requirements of the AS/NZS 1891 series.",
                "Inspect & tag", "Scaffold & height safety", CapabilityBackground.Panel, false,
                "https://summit.us/wp-content/uploads/2022/04/IMG_1373-scaled-e1649187636615-875x1024.jpg", "INSPECTION & TAGGING PROCESSES"),
            ("shutdown-support", "Shutdown, Maintenance & Project Support", "Integrated solutions for planned shutdowns",
                "We support shutdown, maintenance and project works through project planning and mobilisation, workforce and equipment coordination, scaffold and access design coordination, rigging and lifting support, inspection and compliance support, and Just In Time sequencing.",
                "Shutdowns", "Maintenance & project support", CapabilityBackground.Paper, true,
                "https://summit.us/wp-content/uploads/2023/01/1.-OSM-Yard-61-Modular-Assembly.jpg", "SHUTDOWN & MAINTENANCE SUPPORT"),
            ("logistics-materials", "Logistics & Materials Management", "Equipment hire, sales and coordinated supply",
                "Scaffolding equipment hire and sales supported by logistics and materials management. Just In Time material and equipment sequencing, demobilisation and materials reconciliation help make the right resources available when and where required, reducing congestion, downtime and unnecessary material movements.",
                "Hire & sales", "Logistics & materials management", CapabilityBackground.Panel, false,
                "https://summit.us/wp-content/uploads/2022/04/IMG_0239-scaled-e1649186987916-873x1024.jpg", "LOGISTICS & MATERIALS MANAGEMENT")
        ];

        for (var i = 0; i < rows.Length; i++)
        {
            var r = rows[i];
            var capability = await db.Capabilities.FirstOrDefaultAsync(c => c.Key == r.Key);
            if (capability is null)
            {
                capability = new Capability { Key = r.Key, MediaId = Media(r.Url) };
                db.Capabilities.Add(capability);
            }

            capability.Name = r.Name;
            capability.TeaserTag = r.Tag;
            capability.Body = r.Body;
            capability.Stat = r.Stat;
            capability.StatLabel = r.StatLabel;
            capability.Background = r.Bg;
            capability.TextFirst = r.TextFirst;
            capability.FigureLabel = r.Fig;
            capability.DisplayOrder = i;
            capability.IsActive = true;
        }

        await db.SaveChangesAsync();
    }
}
