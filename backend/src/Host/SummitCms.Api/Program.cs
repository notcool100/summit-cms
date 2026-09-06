using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using SummitCms.Api.Seeding;
using SummitCms.Modules.Capabilities;
using SummitCms.Modules.Careers;
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
builder.Services.AddCareersModule(builder.Configuration);
builder.Services.AddContactModule(builder.Configuration);

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
    });
builder.Services.AddAuthorization();

// ---- Rate limiting ----
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("auth", o => { o.PermitLimit = 10; o.Window = TimeSpan.FromMinutes(1); o.QueueLimit = 0; });
    options.AddFixedWindowLimiter("contact", o => { o.PermitLimit = 5; o.Window = TimeSpan.FromMinutes(1); o.QueueLimit = 0; });
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

app.UseSerilogRequestLogging();
app.UseCors("Frontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

var uploadsRoot = Path.Combine(builder.Environment.ContentRootPath, builder.Configuration["FileStorage:RootPath"] ?? "App_Data/uploads");
Directory.CreateDirectory(uploadsRoot);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsRoot),
    RequestPath = builder.Configuration["FileStorage:PublicBaseUrl"] ?? "/uploads"
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
app.MapCareersModule();
app.MapContactModule();

app.Run();
