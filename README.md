# ZoneSync

## About the Project
ZoneSync is a full-stack Smart Agriculture Management System that connects the farm owner, the agricultural engineer, and the field worker in a single operational loop. Farms are digitally structured into Zones, each with its own supervisor and assigned workforce, so responsibility is always clear. Sensor readings and agricultural alerts are automatically turned into time-bound tasks assigned to specific workers, closing the loop between "something needs attention" and "someone is handling it, and we can prove it." Every action, crop change, and workforce movement is logged — without permanent deletion — preserving the farm's full operational history instead of losing it the moment someone forgets to write it down.

The goal isn't to replace farmers and engineers with automation — it's to give them a shared, reliable system that protects crops, saves time, and turns years of hard-won field experience into something the whole team can rely on, long after any one person moves on.

## Project Structure

```
backend/
  ZoneSync.Core/       # Entities and data (EF Core Code-First)
  ZoneSync.Service/    # Contracts (interfaces) and module implementations
  ZoneSync.Web/        # Controllers and views (ASP.NET Core MVC)
frontend/
  index.html           # Frontend prototype
database/
docs/
ZoneSync.sln
```

## Tech Stack

- **Backend:** ASP.NET Core MVC, Entity Framework Core (Code-First), SQL Server
- **Auth:** ASP.NET Core Identity
- **Frontend:** HTML/CSS/JS prototype

## Modules

- **Users & Identity** — accounts, roles, invitation-based onboarding
- **Farm & Zone** — farms, zones, memberships, supervisors
- **Crop & Growth Stage** — crop catalog, growth stages, stage requirements
- **Sensors** — sensor models, sensor instances, sensor readings, requirement checks
- **Alerts & Tasks** — alerts generated from sensor data or manual reports, converted into assigned tasks with action logs

## Getting Started

1. Clone the repo.
2. Update the connection string in `backend/ZoneSync.Web/appsettings.json` to point at your local SQL Server instance.
3. Run the EF Core migrations:
   ```bash
   dotnet ef database update --project backend/ZoneSync.Core --startup-project backend/ZoneSync.Web
   ```
4. Build and run:
   ```bash
   dotnet build
   dotnet run --project backend/ZoneSync.Web
   ```

## Phase-Based Team Setup
- **Users & Identity** — Merna Mohamed El_Atafey
- **Farm & Zone** — Engy Mohammed Elyamany
- **Crop & Growth Stage** — Rana Hassan Moussa
- **Sensors** — AbdelRahman Abdelhamid Khamis Mohamed
- **Alerts & Tasks** — Kholoud Rashed Abdulsalam Roshdy
- **Testing & Integration** — Abeer Fawzy Rashad Ismail