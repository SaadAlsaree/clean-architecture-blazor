# clean-architecture-blazor-template


A Blazor-based data analysis and management platform implemented with Clean Architecture, CQRS patterns, and a modern MudBlazor UI. This repository targets .NET 9 / C# 13 and supports interactive Razor Components in both server and WebAssembly render modes.

## Highlights & Techniques Used
- Clean Architecture (separation into `Domain`, `Application`, `Persistence`, `WebApp` projects).
- CQRS-style request handling using `Cortex.Mediator` (MediatR-style messaging).
- Validation via `FluentValidation`.
- Data access with Entity Framework Core and PostgreSQL provider (`Npgsql`).
- Responsive UI built with `MudBlazor` and `FormCraft.ForMudBlazor`.
- Theme and settings persistence using `IJSRuntime` + `localStorage`.
- Interactive Razor Components supporting both server and WebAssembly render modes (`AddInteractiveServerComponents`, `AddInteractiveWebAssemblyComponents`).
- UI extras: `Heron.MudCalendar`, `MudBlazor.ThemeManager`, and MudBlazor RTL support for right-to-left layouts.
- Project-level configuration through extension methods to keep `Program.cs` concise and testable.

## Solution Structure (high level)
- `src/Domain`  
  Domain entities, value objects, common utilities — pure business model.
- `src/Application`  
  Business logic, use-cases/commands/queries, validators, DTOs, and mediator handlers.
- `src/Persistence`  
  EF Core DbContext, migrations, and repository implementations (Npgsql/Postgres).
- `src/WebApp`  
  Host application that wires services, maps APIs, and serves the Blazor UI. Contains:
  - `WebApp.Client` / client-side components (interactive Razor components).
  - `Components` — shared Razor components and layout (e.g., `MainLayout.razor`, `NavMenu.razor`, `App.razor`).
  - `Services` — UI services (e.g., `ThemeSettings`).
  - `StartupExtensions.cs` — encapsulates service registration and pipeline configuration.

## Important files / responsibilities
- `Program.cs`  
  Minimal entry point that calls extension helpers to register services and build the pipeline. Database reset and seed calls are present but commented out for safety.
- `StartupExtensions.cs` (`WebApp`)  
  Registers application services, persistence, MudBlazor, FormCraft, controllers (custom UTC DateTime model binder), JSON options, and maps Razor components:
  - `MapRazorComponents<App>()` with `.AddInteractiveServerRenderMode()` and `.AddInteractiveWebAssemblyRenderMode()` — enables both server and WASM render modes.
- `ThemeSettings.cs` (`WebApp/Services`)  
  Persists UI settings (dark mode, RTL) using `IJSRuntime` and `localStorage`.
- `MainLayout.razor` & `AppBar.razor`  
  Provide the top-level layout, drawer navigation, theme manager integration, RTL support, and dark/light mode toggling.
- `NavMenu.razor`  
  Drawer navigation using `MudNavMenu` and `MudNavLink`.
- `Home.razor`  
  Dashboard demo page showing quick stats and features list; placeholder logic to load demo statistics.

## Runtime & Dev Requirements
- .NET 9 SDK
- Visual Studio 2022 (or later) with Blazor tooling OR the `dotnet` CLI
- PostgreSQL (or other configured database) if you run EF Core migrations / connect to an actual store

## Run locally
1. Ensure prerequisites are installed (.NET 9 SDK, PostgreSQL if needed).
2. From the repository root:
   - Restore and build:
     - dotnet: `dotnet restore` then `dotnet build`
     - Visual Studio: open the solution and use __Build > Build Solution__
   - Run:
     - dotnet: `dotnet run --project src/WebApp/WebApp`
     - Visual Studio: set `src/WebApp/WebApp` as the startup project and run via __Debug > Start Debugging__ (F5) or __Debug > Start Without Debugging__ (Ctrl+F5).
3. The app maps interactive Razor Components enabling both server and WASM render modes. Open the root URL (usually `https://localhost:5001`) to view the UI.

Notes:
- `Program.cs` contains commented lines to seed the DB (`SeedSuperAdminAsync`) and reset DB. Uncomment and run if you want to enable local seeding — ensure database configuration in `appsettings.*` is correct.
- If you run into CORS / API connectivity issues, check controller mapping under `StartupExtensions.cs` and any API endpoints.

## Configuration
- Connection strings and other environment-specific settings should be configured in `appsettings.Development.json` / `appsettings.json` or via environment variables.
- Theme and UI settings persisted in browser `localStorage` under key `app-settings`. Change defaults in `ThemeSettings` or `MainLayout.razor`.

## Extending & Contributing
- Add new features to `Application` (commands/queries) and implement persistence in `Persistence`.
- Keep UI components in `WebApp/Components` and register any new services in `StartupExtensions.cs`.
- Follow existing patterns for command/query handlers (Cortex.Mediator), and for validation use `FluentValidation`.

## Recommended next steps
- Add CI that runs `dotnet build` and unit tests (if added).
- Add EF Core migrations and sample seed data in `Persistence`.
- Add end-to-end tests for critical UI flows and API endpoints.
- Consider enabling Serilog (commented in `Program.cs`) for structured logging in non-development environments.

## Quick Reference
- UI framework: MudBlazor + FormCraft
- Mediation: Cortex.Mediator
- Validation: FluentValidation
- ORM: EF Core with Npgsql (Postgres)
- Render modes: Interactive Server & WebAssembly Razor Components
- .NET Target: net9.0 (C# 13)

---

If you want, I can:
- Generate a concise project diagram.
- Produce a CONTRIBUTING.md with coding rules and PR checklist.
- Add example `appsettings.Development.json` and EF Core migration scripts.