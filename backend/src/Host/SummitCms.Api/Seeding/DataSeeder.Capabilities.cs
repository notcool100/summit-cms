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
        if (await db.Capabilities.AnyAsync()) return;

        Guid Media(string url) => mediaMap[url];

        (string Key, string Name, string Tag, string Body, string Stat, string StatLabel, CapabilityBackground Bg, bool TextFirst, string Url, string Fig)[] rows =
        [
            ("scaffolding-access", "Scaffolding & Access Solutions", "Traditional and modular systems for diverse access needs",
                "Summit Industrial Services provides a diverse range of high-quality scaffolding and access solutions, from traditional Tube & Fit systems through to modern modular options including the AT-PAC Ringlock System. Our experienced team provides safe and efficient installation, modification, dismantling and ongoing support for projects of varying size and complexity. We tailor our approach to site conditions, operational priorities and project schedules, supported by coordinated logistics, materials management and equipment supply.",
                "50+", "Years combined experience", CapabilityBackground.Paper, true,
                "https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg", "TUBE & FIT / RINGLOCK SYSTEMS"),
            ("scaffold-design", "Scaffold Design & Engineering", "Engineered solutions for complex structures",
                "We provide tailored scaffold design and engineering coordination to meet project-specific access requirements. Working with suitably qualified professionals, Summit develops practical solutions for complex structures, challenging work environments and specialised access requirements. Where engineered design is required, detailed drawings, plans and supporting documentation can be developed and verified by suitably qualified engineers in accordance with applicable Australian Standards, regulatory obligations and site requirements.",
                "100%", "Standards compliant", CapabilityBackground.Panel, false,
                "https://summit.us/wp-content/uploads/2022/05/Intel-Rendering-PNG-edited.png", "ENGINEERED DESIGN COORDINATION"),
            ("rigging", "Crane & Mechanical Rigging", "Licensed rigging operations with competency verification",
                "Crane and mechanical rigging operations supported by High Risk Work Licence holders and competency verification. Our teams combine planning, competent personnel, fit-for-purpose equipment and disciplined work methods. Rigging inspections and lifting support are managed in accordance with applicable WHS legislation, codes of practice, client procedures and relevant Australian Standards.",
                "Licensed", "HRWL holders", CapabilityBackground.Paper, true,
                "https://summit.us/wp-content/uploads/2022/04/IMG_1679-scaled-e1649188273711.jpg", "PRECISION RIGGING OPERATIONS"),
            ("inspection-tagging", "Inspection, Tagging & Compliance", "Height safety equipment and scaffold inspection",
                "Working-at-heights equipment inspection and tagging processes ensure all equipment meets safety requirements. Scaffold inspection, handover and tagging processes are integrated into our operations. Continuous improvement through reporting, review and learning drives our safety culture. All inspections comply with AS/NZS 1891 series and relevant Australian Standards.",
                "AS/NZS 1891", "Compliant systems", CapabilityBackground.Panel, false,
                "https://summit.us/wp-content/uploads/2022/04/IMG_1373-scaled-e1649187636615-875x1024.jpg", "INSPECTION & TAGGING PROCESSES"),
            ("shutdown-support", "Shutdown, Maintenance & Project Support", "Integrated solutions for planned shutdowns",
                "Integrated access and rigging solutions for shutdown, maintenance and project works. Project planning and mobilisation, workforce and equipment coordination, scaffold and access design coordination, rigging and lifting support, inspection, tagging and compliance support — all coordinated through Just-In-Time material and equipment sequencing to minimize downtime and ensure safe, efficient execution.",
                "JIT", "Sequenced delivery", CapabilityBackground.Paper, true,
                "https://summit.us/wp-content/uploads/2023/01/1.-OSM-Yard-61-Modular-Assembly.jpg", "SHUTDOWN & MAINTENANCE SUPPORT"),
            ("logistics-materials", "Logistics & Materials Management", "Equipment hire, sales and coordinated supply",
                "Scaffolding equipment hire and sales supported by logistics and materials management. Just-In-Time material and equipment sequencing, demobilisation and materials reconciliation ensure the right resources are available when and where required, helping reduce site congestion, downtime and unnecessary material movements. Coordinated supply chain management for optimal project efficiency.",
                "Coordinated", "Supply chain", CapabilityBackground.Panel, false,
                "https://summit.us/wp-content/uploads/2022/04/IMG_0239-scaled-e1649186987916-873x1024.jpg", "LOGISTICS & MATERIALS MANAGEMENT")
        ];

        for (var i = 0; i < rows.Length; i++)
        {
            var r = rows[i];
            db.Capabilities.Add(new Capability
            {
                Key = r.Key, Name = r.Name, TeaserTag = r.Tag, Body = r.Body, Stat = r.Stat, StatLabel = r.StatLabel,
                Background = r.Bg, TextFirst = r.TextFirst, MediaId = Media(r.Url), FigureLabel = r.Fig, DisplayOrder = i, IsActive = true
            });
        }

        await db.SaveChangesAsync();
    }
}
