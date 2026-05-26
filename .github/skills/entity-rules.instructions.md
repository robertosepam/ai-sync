---
applyTo: "Backend/src/AiSync.Domain/**,Backend/src/AiSync.Application/**"
---

# Domain Rules — Entities, DTOs, Repositories

## Entities (`AiSync.Domain/Entities`)
- Entities are plain classes with no infrastructure logic.
- Properties use `{ get; set; }` and are initialized with default values to avoid nulls.
- No EF Core annotations on entities — configuration goes in `AppDbContext`.

```csharp
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
```

## Repository interfaces (`AiSync.Domain/Interfaces`)
- Define in `Domain`, implement in `Infrastructure`.
- Async methods with `CancellationToken`.
- Minimum operations: `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`.

## DTOs (`AiSync.Application/DTOs`)
- One DTO per operation: `CreateXDto`, `UpdateXDto`, `XDto` (response).
- No business logic inside DTOs.
- `XDto` (response) includes `Id` and all fields needed by the client.

## Services (`AiSync.Application/Services`)
- Implement the interface defined in `AiSync.Application/Interfaces`.
- Only orchestrate repositories and map between entities and DTOs.
- Do not access EF Core or `DbContext` directly.
