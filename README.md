# OrderProcessingSystem

A .NET 8 order processing service that accepts, validates, persists and advances orders through lifecycle states (Pending → Processing → Shipped → Delivered). The solution is layered (Api, Application, Data), uses MediatR for request/command handling (CQRS-style), and EF Core for persistence.

## Key features
- Layered architecture: `Api`, `Application`, `Data`.
- MediatR for commands/handlers and orchestration.
- EF Core DbContext in `OrderProcessingSystem.Data`.
- Background processing via a hosted service: `OrderProcessorBackgroundService` (advances order statuses).
- Business rules implemented in `OrderProcessingSystem.Application.Rules.OrderRules` (e.g., update/cancel rules).

## Projects
- `OrderProcessingSystem.Api` — HTTP API, DI, hosted/background services, configuration.
- `OrderProcessingSystem.Application` — MediatR requests/handlers, validation, business rules and DTOs.
- `OrderProcessingSystem.Data` — EF Core entities, DbContext, persistence.

## Getting started
1. Restore and build:
   - `dotnet restore`
   - `dotnet build`
2. Run the API:
   - `dotnet run --project OrderProcessingSystem.Api`
3. Database:
   - Use EF Core migrations and your preferred database provider. Typical commands:
     - `dotnet ef migrations add InitialCreate --project OrderProcessingSystem.Data`
     - `dotnet ef database update --project OrderProcessingSystem.Data`

## Configuration
Important configuration key used by the background service:
- `OrderProcessBackgroundServiceIntervalMs` — interval in milliseconds for the `OrderProcessorBackgroundService` loop.

Example `appsettings.json` snippet: