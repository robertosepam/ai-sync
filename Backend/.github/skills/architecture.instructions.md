---
applyTo: "Backend/**"
---

# Architecture — Clean Architecture

## Layers and responsibilities

| Layer | Project | Responsibility |
|---|---|---|
| Domain | `AiSync.Domain` | Entities, repository interfaces. No external dependencies. |
| Application | `AiSync.Application` | Services, DTOs, service interfaces. Depends only on Domain. |
| Infrastructure | `AiSync.Infrastructure` | EF Core, concrete repositories, DbContext. Depends on Domain. |
| API | `AiSync.API` | Controllers, HTTP configuration. Depends on Application and Infrastructure. |

## Dependency rules
- `Domain` does not reference any other project in the solution.
- `Application` only references `Domain`.
- `Infrastructure` only references `Domain`.
- `API` references `Application` and `Infrastructure` (only for DI in `Program.cs`).

## Dependency registration
- Each layer exposes an extension method `AddX(this IServiceCollection services)` in its own `DependencyInjection.cs`.
- `Program.cs` only calls `builder.Services.AddApplication()` and `builder.Services.AddInfrastructure(config)`.
