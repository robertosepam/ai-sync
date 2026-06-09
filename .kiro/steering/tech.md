# Tech Stack

## Runtime & Framework
- **.NET 10** — all projects target `net10.0`
- **ASP.NET Core** — HTTP pipeline, controllers, DI
- **Entity Framework Core 10** with **SQL Server** provider
- **Swashbuckle / Swagger** — API docs in development

## Language features in use
- Nullable reference types enabled (`<Nullable>enable</Nullable>`)
- Implicit usings enabled
- `record` types for DTOs
- `CancellationToken` on every async method
- `AsNoTracking()` on all read queries

## Testing
- **xUnit** — unit test framework
- **coverlet** — code coverage collection
- Test projects live under `Backend/src/AISync.UnitTests/`, one project per layer

## Common Commands

```bash
# Restore & build
dotnet build Backend/AI-Sync-Backend.slnx

# Run the API
dotnet run --project Backend/src/AiSync.API/AiSync.API.csproj

# Run all tests
dotnet test Backend/AI-Sync-Backend.slnx

# EF Core migrations (run from solution root)
dotnet ef migrations add <MigrationName> \
  --project Backend/src/AiSync.Infrastructure \
  --startup-project Backend/src/AiSync.API

dotnet ef database update \
  --project Backend/src/AiSync.Infrastructure \
  --startup-project Backend/src/AiSync.API
```

## Connection string
Configured via `appsettings.json` / `appsettings.Development.json` under the key `ConnectionStrings:DefaultConnection`.
