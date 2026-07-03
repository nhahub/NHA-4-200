# ZoneSync FullStack

Clean repository start for the ZoneSync graduation/training project.

## Stack

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- ASP.NET Identity

## Team Workflow

- Work on feature branches only.
- Do not commit directly to `main`.
- Build before pushing.
- Keep the project as one shared solution.

## Planned Modules

- Foundation / Identity
- Farm & Zone
- Crop / Stages / CropPlan
- Sensors / Readings
- Alerts / Tasks / Action Logs
- Integration / UI / Presentation

## Phase 1 - Foundation / Identity

Current branch:

`feature/foundation-identity`

Implemented:

- `ZoneSync.Core` for entities and `ApplicationDbContext`.
- `ZoneSync.Service` for identity business logic.
- `ZoneSync.Web` for MVC controllers and views.
- ASP.NET Identity with custom `ApplicationUser` and `ApplicationRole`.
- Domain profile table mapped to `[User]`.
- `Invitation` workflow with token and verification code.
- `FarmMembership` table for per-farm roles: Owner, Engineer, Farmer.
- Initial EF migration: `InitialIdentityFoundation`.

Run checks:

```powershell
dotnet restore ZoneSync.sln --configfile NuGet.Config
dotnet build ZoneSync.sln
```

Apply database migration:

```powershell
dotnet ef database update --project backend\ZoneSync.Core --startup-project backend\ZoneSync.Web
```

Run the MVC app:

```powershell
dotnet run --project backend\ZoneSync.Web
```

Phase 2 should add `Farm` and `Zone` entities in `ZoneSync.Core`, then connect `FarmMembership.FarmId` to the new `Farm` table using Fluent API.

Previous repository contents were backed up in:

`backup/before-clean-2026-07-03`
