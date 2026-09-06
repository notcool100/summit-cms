# SummitCms.Api

A .NET 10 Clean Modular Monolith backend for the Summit marketing site — full admin CRUD over every
piece of content the frontend renders, plus auth, media, and a contact-form lead pipeline, backed by
a 3NF-normalized PostgreSQL database.

## Architecture

One ASP.NET Core Web API host (`src/Host/SummitCms.Api`) composed of 9 independent modules under
`src/Modules/*`, plus a `src/Shared` kernel/infrastructure layer. Each module:

- owns its **own EF Core `DbContext`**, mapped to its **own PostgreSQL schema** (`identity`, `media`,
  `content`, `company`, `capabilities`, `industries`, `projects`, `careers`, `contact`) and its own
  migrations folder — modules can be migrated/evolved independently even though they share one database;
- is organized internally into `Domain` / `Application` / `Infrastructure` / `Api` folders, plus a
  `Contracts` folder — the **only** thing another module is allowed to reference. Cross-module reads go
  through a small `I*Catalog` interface (e.g. `IMediaCatalog`, `IProjectCatalog`); there are no
  cross-schema foreign keys, so no module's migrations depend on another's.

Identity and Media are the two "foundational" modules everything else is allowed to depend on downward
(auth, and resolving a `MediaId` into a URL). Peer business modules never reference each other's
Domain/Infrastructure — e.g. Industries links to Projects via a plain `Guid` validated through
`IProjectCatalog`, not a database foreign key.

Two API surfaces:

- **`/api/admin/**`** — JWT + role/permission protected, full CRUD on every entity.
- **`/api/public/**`** — anonymous, read-only, shaped for what the frontend actually renders per page
  (`/api/public/home`-equivalent data lives across a few endpoints — see below), plus
  `POST /api/public/contact` for the contact form.

Reference-data entities (capabilities, milestones, awards, team members, ...) are wired up through a
generic admin-CRUD helper (`CrudEndpointExtensions.MapAdminCrud`) instead of hand-written MediatR
commands for each one — every mutation still runs through `ICurrentUser`/audit logging. Modules with
real business logic (auth, media upload, contact triage, project/industry linking) have hand-written
Application-layer handlers.

Every admin mutation — generic-CRUD or bespoke — publishes an `EntityAuditEvent` (MediatR notification);
the Identity module is the sole subscriber and writes it to `identity.audit_logs`. This gives a full
cross-module audit trail without any module taking a DB dependency on Identity.

## Running locally

```bash
cd src/Host/SummitCms.Api
dotnet run
```

On startup the host:

1. Applies every module's pending EF Core migrations against the configured database.
2. Seeds (idempotently — safe to run on every startup): system roles (`SuperAdmin`, `Admin`, `Editor`,
   `Viewer`) and their permissions, a `SuperAdmin` user, and the site's real current copy (home stats,
   capabilities, industries, all 8 projects incl. the full Phoenix case-study detail, about-page content,
   career tracks, enquiry types) — ported from the old static frontend data files — so the database
   isn't empty on day one.

The seeded admin's password is either read from `Seed:AdminPassword` config, or (if unset) randomly
generated and printed **once** to the startup log — copy it immediately, it is never shown again. Set
`Seed:AdminPassword` in `appsettings.Development.json` (or the `Seed__AdminPassword` env var) for a
reproducible local login.

Swagger/Scalar API explorer is available at `/scalar/v1` in Development.

### Configuration

`appsettings.json` is committed with placeholder values only. Real values go in
`appsettings.Development.json` (git-ignored) for local dev, or environment variables in any other
environment:

| Setting | Env var | Purpose |
|---|---|---|
| `ConnectionStrings:Default` | `ConnectionStrings__Default` | Postgres connection string |
| `Jwt:SigningKey` | `Jwt__SigningKey` | HMAC-SHA256 key for access tokens (32+ bytes) |
| `Seed:AdminEmail` / `Seed:AdminPassword` | `Seed__AdminEmail` / `Seed__AdminPassword` | Seeded SuperAdmin credentials |
| `Cors:AllowedOrigins` | `Cors__AllowedOrigins__0`, `...__1`, ... | Frontend origin(s) allowed to call the API |
| `FileStorage:RootPath` / `PublicBaseUrl` | `FileStorage__RootPath` / `FileStorage__PublicBaseUrl` | Local-disk media upload storage (swap `IFileStorageService` for S3 later without touching callers) |

### Adding a migration after changing an entity

Each module manages its own migrations independently:

```bash
cd src/Modules/<ModuleName>
dotnet ef migrations add <Name> -o Infrastructure/Migrations --context <ModuleName>DbContext
dotnet ef database update --context <ModuleName>DbContext
```

(`Infrastructure/*DbContextFactory.cs` reads the design-time connection string from the
`SUMMITCMS_CONNECTION` env var — it throws with a clear message if unset, rather than falling back to
any hardcoded value — so set it first: `export SUMMITCMS_CONNECTION="Host=...;Port=...;Database=...;Username=...;Password=..."`.)

## Tests

```bash
dotnet test tests/SummitCms.UnitTests
```

Covers password hashing, JWT claim generation, and the permission-authorization handler (including the
SuperAdmin bypass).

## Project layout

```
src/
  Host/SummitCms.Api/            Program.cs, appsettings, Seeding/
  Shared/
    SummitCms.Shared.Kernel/     Entity base types, Result<T>, pagination, PageSlugs
    SummitCms.Shared.Infrastructure/  Generic CRUD repo + endpoint mapper, audit events,
                                       permission auth, per-module DbContext registration helper
  Modules/
    Identity/     users, roles, permissions, refresh tokens, audit log, JWT issuance
    Media/        media library (local-disk today, swappable), folders
    SiteContent/  page hero/SEO content, global site settings, enquiry types, cross-page metric stats
    Company/      About-page content: milestones, values, team, offices, awards, narrative
    Capabilities/ capability panels (shared by the home teaser strip and capabilities page)
    Industries/   industries served, soft-linked to Projects
    Projects/     projects, gallery, scope facts, narrative, quotes, industry-category lookup
    Careers/      job tracks + tags, real job openings
    Contact/      contact-form submissions / lead triage
tests/
  SummitCms.UnitTests/
```
