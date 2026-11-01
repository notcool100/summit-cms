using System.Net;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using SummitCms.Api.Seeding;
using SummitCms.Modules.Blog;
using SummitCms.Modules.Capabilities;
using SummitCms.Modules.Company;
using SummitCms.Modules.Contact;
using SummitCms.Modules.Identity;
using SummitCms.Modules.Identity.Application;
using SummitCms.Modules.Industries;
using SummitCms.Modules.Media;
using SummitCms.Modules.Projects;
using SummitCms.Modules.SiteContent;
using SummitCms.Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Don't advertise the server software.
builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);

// The API is reached through the SvelteKit server (and in production the Coolify proxy). Trust
// X-Forwarded-For only when the direct caller is on a private/loopback network, so the real
// visitor IP is used for rate limiting and the audit log, while a public caller can't spoof it.
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.ForwardLimit = 1;
    o.KnownIPNetworks.Clear();
    o.KnownProxies.Clear();
    foreach (var cidr in new[] { "127.0.0.0/8", "10.0.0.0/8", "172.16.0.0/12", "192.168.0.0/16", "::1/128", "fc00::/7" })
        o.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(cidr));
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<SummitCms.Api.Infrastructure.ApiExceptionHandler>();

builder.Host.UseSerilog((context, config) => config
    .ReadFrom.Configuration(context.Configuration)
    .WriteTo.Console());

// ---- Shared cross-cutting services ----
builder.Services.AddSharedInfrastructure();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(SummitCms.Modules.Identity.IdentityModule).Assembly,
    typeof(SummitCms.Modules.Contact.ContactModule).Assembly));

// ---- Modules (each owns its own DbContext/schema/migrations) ----
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddMediaModule(builder.Configuration);
builder.Services.AddSiteContentModule(builder.Configuration);
builder.Services.AddCompanyModule(builder.Configuration);
builder.Services.AddCapabilitiesModule(builder.Configuration);
builder.Services.AddIndustriesModule(builder.Configuration);
builder.Services.AddProjectsModule(builder.Configuration);
builder.Services.AddContactModule(builder.Configuration);
builder.Services.AddBlogModule(builder.Configuration);

// ---- Auth ----
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
        // A deactivated or deleted account loses access immediately, not when its token expires.
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                var sub = ctx.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(sub, out var userId)) { ctx.Fail("Invalid subject."); return; }
                var cache = ctx.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
                var active = await cache.GetOrCreateAsync(SummitCms.Modules.Identity.Application.Security.IdentityAccessGuard.ActiveCacheKey(userId), async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15);
                    var db = ctx.HttpContext.RequestServices.GetRequiredService<SummitCms.Modules.Identity.Infrastructure.IdentityDbContext>();
                    return await db.Users.AsNoTracking().AnyAsync(u => u.Id == userId && u.IsActive);
                });
                if (!active) ctx.Fail("Account is inactive.");
            }
        };
    });
builder.Services.AddAuthorization();

// ---- Rate limiting ----
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    // Both limits are per visitor IP (resolved by the forwarded-headers middleware above), never
    // global - one attacker must not be able to lock every admin out of sign-in, or every visitor
    // out of the contact form. Per-account sign-in lockout is handled by LoginThrottle.
    options.AddPolicy("auth", http =>
        RateLimitPartition.GetFixedWindowLimiter(http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
    options.AddPolicy("contact", http =>
    {
        var clientIp = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(10),
            QueueLimit = 0
        });
    });
});

// ---- CORS ----
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
{
    if (allowedOrigins.Length > 0)
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
}));

builder.Services.AddOpenApi();

var app = builder.Build();

// ---- Migrate every module's schema, then seed roles/permissions/admin user + real site content ----
await DatabaseMigrator.MigrateAllAsync(app.Services);
await DataSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.Use(async (http, next) =>
{
    var h = http.Response.Headers;
    h.XContentTypeOptions = "nosniff";
    h.XFrameOptions = "DENY";
    h["Referrer-Policy"] = "no-referrer";
    await next();
});
app.UseSerilogRequestLogging();
app.UseCors("Frontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

var uploadsRoot = Path.Combine(builder.Environment.ContentRootPath, builder.Configuration["FileStorage:RootPath"] ?? "App_Data/uploads");
Directory.CreateDirectory(uploadsRoot);
// Serve only the media types uploads are allowed to be (see MediaFileInspector). Anything else on
// disk - e.g. a file stored before upload validation existed - is not served at all, and what is
// served is sandboxed so it can never run script in the site's origin.
var uploadTypes = new FileExtensionContentTypeProvider();
uploadTypes.Mappings.Clear();
foreach (var (ext, mime) in SummitCms.Modules.Media.Application.MediaFileInspector.ServableTypes)
    uploadTypes.Mappings[ext] = mime;
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsRoot),
    RequestPath = builder.Configuration["FileStorage:PublicBaseUrl"] ?? "/uploads",
    ContentTypeProvider = uploadTypes,
    ServeUnknownFileTypes = false,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.ContentSecurityPolicy = "default-src 'none'; img-src 'self'; style-src 'unsafe-inline'; sandbox";
        ctx.Context.Response.Headers.CacheControl = "public, max-age=604800";
    }
});

app.MapGet("/", () => Results.Ok(new { service = "SummitCms.Api", status = "ok" }));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapIdentityModule();
app.MapMediaModule();
app.MapSiteContentModule();
app.MapCompanyModule();
app.MapCapabilitiesModule();
app.MapIndustriesModule();
app.MapProjectsModule();
app.MapContactModule();
app.MapBlogModule();

app.Run();
