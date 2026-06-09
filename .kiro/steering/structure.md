# Project Structure

## Repository layout

```
AI-Sync/
├── .github/
│   ├── copilot-instructions.md
│   └── skills/                        # Copilot skill files (applyTo-scoped rules)
├── .kiro/
│   └── steering/                      # Kiro steering files
└── Backend/
    ├── AI-Sync-Backend.slnx           # Solution file
    └── src/
        ├── AiSync.Domain/             # Layer 1 — core domain
        │   ├── Entities/              # Plain entity classes
        │   └── Interfaces/            # Repository contracts
        ├── AiSync.Application/        # Layer 2 — use cases
        │   ├── DTOs/                  # Request/response records
        │   ├── Interfaces/            # Service contracts
        │   ├── Services/              # Service implementations
        │   └── DependencyInjection.cs
        ├── AiSync.Infrastructure/     # Layer 3 — data access
        │   ├── Persistence/
        │   │   ├── AppDbContext.cs
        │   │   └── Migrations/
        │   ├── Repositories/          # IRepository implementations
        │   └── DependencyInjection.cs
        ├── AiSync.API/                # Layer 4 — HTTP entry point
        │   ├── Controllers/
        │   ├── Properties/
        │   ├── Program.cs
        │   └── appsettings*.json
        └── AISync.UnitTests/          # Test projects (one per layer)
            ├── AiSync.API.Tests/
            ├── AiSync.Application.Tests/
            ├── AiSync.Domain.Tests/
            └── AiSync.Infrastructure.Tests/
```

## Clean Architecture dependency rules

```
Domain  ←  Application  ←  Infrastructure
                       ←  API  →  (DI only)
```

- `Domain` has zero project references.
- `Application` references only `Domain`.
- `Infrastructure` references only `Domain`.
- `API` references `Application` + `Infrastructure` (wiring in `Program.cs` only).

## Naming conventions

| Artifact | Pattern | Example |
|---|---|---|
| Entity | `<Name>` | `Employee` |
| Repository interface | `I<Name>Repository` | `IEmployeeRepository` |
| Service interface | `I<Name>Service` | `IEmployeeService` |
| Service implementation | `<Name>Service` | `EmployeeService` |
| Repository implementation | `<Name>Repository` | `EmployeeRepository` |
| Response DTO | `<Name>Dto` | `EmployeeDto` |
| Create DTO | `Create<Name>Dto` | `CreateEmployeeDto` |
| Update DTO | `Update<Name>Dto` | `UpdateEmployeeDto` |
| Controller | `<Name>sController` | `EmployeesController` |
| Test class | mirrors source class name | `EmployeeServiceTests` |

## Adding a new entity — checklist

1. `AiSync.Domain/Entities/<Name>.cs` — plain class, no EF annotations
2. `AiSync.Domain/Interfaces/I<Name>Repository.cs` — async interface with `CancellationToken`
3. `AiSync.Application/DTOs/` — three records: `<Name>Dto`, `Create<Name>Dto`, `Update<Name>Dto`
4. `AiSync.Application/Interfaces/I<Name>Service.cs`
5. `AiSync.Application/Services/<Name>Service.cs` — implement service, map via private static method
6. `AiSync.Infrastructure/Repositories/<Name>Repository.cs` — EF Core implementation
7. `AiSync.Infrastructure/Persistence/AppDbContext.cs` — add `DbSet<Name>` and fluent config in `OnModelCreating`
8. `AiSync.Infrastructure/DependencyInjection.cs` — register repository
9. `AiSync.Application/DependencyInjection.cs` — register service
10. `AiSync.API/Controllers/<Name>sController.cs` — CRUD endpoints
