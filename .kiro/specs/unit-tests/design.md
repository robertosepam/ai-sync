# Design Document — Unit Tests for Employee CRUD

## Overview

This design covers the implementation of comprehensive unit tests for the Employee CRUD feature across all four Clean Architecture layers. The test suite uses **xUnit** as the test framework, **Moq** for mocking dependencies in the Application and API layers, and **EF Core's in-memory provider** for integration-style repository tests in the Infrastructure layer.

The goal is to verify correctness of each layer in isolation: the Domain layer tests entity property assignment, the Application layer tests service orchestration and mapping logic, the Infrastructure layer tests EF Core repository behavior, and the API layer tests HTTP response shaping.

Each test project already exists under `Backend/src/AISync.UnitTests/`, one per layer. The design adds the missing NuGet packages, removes placeholder files, and specifies the test classes and their structure.

---

## Architecture

The test suite follows the same Clean Architecture layering as the production code. Each test project references only the production project it is testing and its direct dependencies.

```
┌────────────────────────┐     ┌──────────────────────────────┐
│  AiSync.Domain.Tests   │     │  AiSync.Application.Tests    │
│  refs: AiSync.Domain   │     │  refs: AiSync.Application    │
│  tools: xUnit          │     │  tools: xUnit + Moq          │
└────────────────────────┘     └──────────────────────────────┘
┌──────────────────────────────────┐  ┌──────────────────────────┐
│  AiSync.Infrastructure.Tests     │  │  AiSync.API.Tests         │
│  refs: AiSync.Infrastructure     │  │  refs: AiSync.API         │
│  tools: xUnit + EF InMemory      │  │  tools: xUnit + Moq       │
└──────────────────────────────────┘  └──────────────────────────┘
```

### Layer isolation rules

- **Domain tests** — no mocks; direct entity instantiation only.
- **Application tests** — `IEmployeeRepository` mocked via Moq; no EF Core or HTTP context.
- **Infrastructure tests** — real `EmployeeRepository` against `AppDbContext` backed by `UseInMemoryDatabase`; no Moq.
- **API tests** — `IEmployeeService` mocked via Moq; controllers instantiated directly (no `TestServer`); action results inspected via Microsoft.AspNetCore.Mvc types.

---

## Components and Interfaces

### Project dependency additions

| Test project | Package to add | Version |
|---|---|---|
| `AiSync.Application.Tests` | `Moq` | `4.20.72` |
| `AiSync.API.Tests` | `Moq` | `4.20.72` |
| `AiSync.Infrastructure.Tests` | `Microsoft.EntityFrameworkCore.InMemory` | `10.x` (compatible with EF Core 10) |
| `AiSync.Domain.Tests` | *(none)* | — |

### Files to delete

Each test project contains a placeholder `UnitTest1.cs` that must be removed before the real test classes are created.

### Test classes per project

#### `AiSync.Domain.Tests`
- `EmployeeEntityTests` — tests `Employee` entity property assignment and defaults.

#### `AiSync.Application.Tests`
- `EmployeeServiceGetAllAsyncTests` — covers `GetAllAsync`.
- `EmployeeServiceGetByIdAsyncTests` — covers `GetByIdAsync`.
- `EmployeeServiceCreateAsyncTests` — covers `CreateAsync`.
- `EmployeeServiceUpdateAsyncTests` — covers `UpdateAsync`.
- `EmployeeServiceDeleteAsyncTests` — covers `DeleteAsync`.

#### `AiSync.Infrastructure.Tests`
- `EmployeeRepositoryGetAllAsyncTests` — covers `GetAllAsync`.
- `EmployeeRepositoryGetByIdAsyncTests` — covers `GetByIdAsync`.
- `EmployeeRepositoryAddAsyncTests` — covers `AddAsync`.
- `EmployeeRepositoryUpdateAsyncTests` — covers `UpdateAsync`.
- `EmployeeRepositoryDeleteAsyncTests` — covers `DeleteAsync`.

#### `AiSync.API.Tests`
- `EmployeesControllerGetAllTests` — covers `GetAll`.
- `EmployeesControllerGetByIdTests` — covers `GetById`.
- `EmployeesControllerCreateTests` — covers `Create`.
- `EmployeesControllerUpdateTests` — covers `Update`.
- `EmployeesControllerDeleteTests` — covers `Delete`.

### Shared test helpers

#### `EmployeeFactory` (within Application.Tests and Infrastructure.Tests)
A static helper that builds `Employee` instances with caller-supplied values. Keeps test setup readable.

#### `AppDbContextFactory` (within Infrastructure.Tests)
A factory method that creates a fresh `AppDbContext` backed by `UseInMemoryDatabase(Guid.NewGuid().ToString())`. Using a unique database name per test ensures full isolation.

```csharp
internal static AppDbContext CreateContext() =>
    new AppDbContext(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
```

---

## Data Models

No new data models are introduced. Tests operate on the existing types:

| Type | Layer | Role in tests |
|---|---|---|
| `Employee` | Domain | SUT in Domain tests; entity seeded in Infrastructure tests; returned by mocked `IEmployeeRepository` in Application tests |
| `EmployeeDto` | Application | Returned by mocked `IEmployeeService` in API tests; asserted as return value in Application tests |
| `CreateEmployeeDto` | Application | Input to `EmployeeService.CreateAsync` and `EmployeesController.Create` |
| `UpdateEmployeeDto` | Application | Input to `EmployeeService.UpdateAsync` and `EmployeesController.Update` |
| `AppDbContext` | Infrastructure | Backed by in-memory provider in Infrastructure tests |

### Mapping contract under test

`EmployeeService` contains a private `MapToDto(Employee e)` method whose contract is:
```
EmployeeDto.Id         == Employee.Id
EmployeeDto.Name       == Employee.Name
EmployeeDto.DateOfBirth == Employee.DateOfBirth
EmployeeDto.IsActive   == Employee.IsActive
```

This contract is tested through the public service methods; the mapping method itself is not accessed directly.

---

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system — essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

This feature is a unit-test suite itself, not a data-transformation or parsing library. The classes under test are thin wrappers, mappers, and delegates. Several of their behaviors are general across all valid inputs, making them well-suited to property-based testing with data-driven variation using xUnit's `[MemberData]` or hand-rolled generators. A full PBT library (e.g., FsCheck) is not required here; the properties below are validated by parameterized xUnit theories that sweep across multiple representative inputs generated at test construction time.

The following properties survive the redundancy reflection:

- 2.1/2.4 are consolidated → **Property 1**
- 3.1 → **Property 2**
- 4.1/4.3 are consolidated → **Property 3**
- 5.1/5.2 are consolidated → **Property 4**
- 6.1 → **Property 5**
- 7.1 → **Property 6**
- 8.1 → **Property 7**
- 9.1/9.2/9.3 are consolidated → **Property 8**
- 10.1/10.2 are consolidated → **Property 9**
- 11.1/11.2 are consolidated → **Property 10**
- 12.1 → **Property 11**
- 13.1 → **Property 12**
- 14.1 → **Property 13**
- 15.1 → **Property 14**

---

### Property 1: GetAllAsync mapping preserves all Employee fields

*For any* non-empty list of `Employee` objects returned by the repository, `EmployeeService.GetAllAsync` SHALL return an `IEnumerable<EmployeeDto>` of the same length where each element's `Id`, `Name`, `DateOfBirth`, and `IsActive` fields exactly match the corresponding source `Employee`.

**Validates: Requirements 2.1, 2.4**

---

### Property 2: GetByIdAsync maps found Employee to EmployeeDto

*For any* `Employee` returned by `IEmployeeRepository.GetByIdAsync`, `EmployeeService.GetByIdAsync` SHALL return an `EmployeeDto` whose `Id`, `Name`, `DateOfBirth`, and `IsActive` fields exactly match the source `Employee`.

**Validates: Requirements 3.1**

---

### Property 3: CreateAsync maps DTO to entity and result DTO correctly

*For any* `CreateEmployeeDto` with a given `Name`, `DateOfBirth`, and `IsActive`, `EmployeeService.CreateAsync` SHALL (a) call `IEmployeeRepository.AddAsync` with an `Employee` whose `Name`, `DateOfBirth`, and `IsActive` match the DTO, and (b) return an `EmployeeDto` whose fields match the `Employee` instance returned by the repository.

**Validates: Requirements 4.1, 4.3**

---

### Property 4: UpdateAsync mutates entity fields and returns correct DTO

*For any* existing `Employee` and `UpdateEmployeeDto` with given `Name`, `DateOfBirth`, and `IsActive`, `EmployeeService.UpdateAsync` SHALL (a) call `IEmployeeRepository.UpdateAsync` with the entity mutated so that `Name`, `DateOfBirth`, and `IsActive` match the DTO, and (b) return an `EmployeeDto` whose fields match the updated entity.

**Validates: Requirements 5.1, 5.2**

---

### Property 5: DeleteAsync passes id through to repository unchanged

*For any* integer `id`, `EmployeeService.DeleteAsync(id)` SHALL call `IEmployeeRepository.DeleteAsync` with that exact same `id`.

**Validates: Requirements 6.1**

---

### Property 6: GetAllAsync returns all seeded employees from the database

*For any* non-empty collection of `Employee` records seeded into the in-memory database, `EmployeeRepository.GetAllAsync` SHALL return a collection containing every seeded employee (matched by `Id`).

**Validates: Requirements 7.1**

---

### Property 7: GetByIdAsync returns the Employee matching the queried id

*For any* `Employee` stored in the in-memory database, `EmployeeRepository.GetByIdAsync(employee.Id)` SHALL return an `Employee` whose `Id`, `Name`, `DateOfBirth`, and `IsActive` match the seeded record.

**Validates: Requirements 8.1**

---

### Property 8: AddAsync persists the employee and assigns a positive Id

*For any* valid `Employee` added via `EmployeeRepository.AddAsync`, the repository SHALL persist the record to the database (verifiable by a direct re-query), return the entity with an `Id > 0`, and increase the total employee count in the database by exactly `1`.

**Validates: Requirements 9.1, 9.2, 9.3**

---

### Property 9: UpdateAsync persists all modified fields and returns updated entity

*For any* `Employee` seeded in the database and any new values for `Name`, `DateOfBirth`, and `IsActive`, calling `EmployeeRepository.UpdateAsync` with the modified entity SHALL persist all three changed fields to the database and return an `Employee` instance with those same updated values.

**Validates: Requirements 10.1, 10.2**

---

### Property 10: DeleteAsync removes the employee and decreases count by 1

*For any* `Employee` seeded in the database, calling `EmployeeRepository.DeleteAsync(employee.Id)` SHALL remove that record from the database and reduce the total employee count by exactly `1`.

**Validates: Requirements 11.1, 11.2**

---

### Property 11: GetAll controller wraps any service result in OkObjectResult

*For any* `IEnumerable<EmployeeDto>` (including empty) returned by `IEmployeeService.GetAllAsync`, `EmployeesController.GetAll` SHALL return an `OkObjectResult` whose `Value` is that exact collection.

**Validates: Requirements 12.1, 12.2**

---

### Property 12: GetById controller wraps found EmployeeDto in OkObjectResult

*For any* `EmployeeDto` returned by `IEmployeeService.GetByIdAsync`, `EmployeesController.GetById` SHALL return an `OkObjectResult` whose `Value` is that exact `EmployeeDto`.

**Validates: Requirements 13.1**

---

### Property 13: Create controller returns CreatedAtActionResult with correct shape

*For any* `EmployeeDto` returned by `IEmployeeService.CreateAsync`, `EmployeesController.Create` SHALL return a `CreatedAtActionResult` with `ActionName == "GetById"` and `RouteValues["id"]` equal to the `EmployeeDto.Id`.

**Validates: Requirements 14.1**

---

### Property 14: Update controller wraps returned EmployeeDto in OkObjectResult

*For any* `EmployeeDto` returned by `IEmployeeService.UpdateAsync`, `EmployeesController.Update` SHALL return an `OkObjectResult` whose `Value` is that exact `EmployeeDto`.

**Validates: Requirements 15.1**

---

## Error Handling

### Application layer

| Scenario | Method | Behavior |
|---|---|---|
| Repository returns `null` for `GetByIdAsync` | `EmployeeService.GetByIdAsync` | Returns `null` |
| Repository returns `null` for `GetByIdAsync` during update | `EmployeeService.UpdateAsync` | Throws `KeyNotFoundException` |

### Infrastructure layer

| Scenario | Method | Behavior |
|---|---|---|
| Entity with given `id` not found in DB | `EmployeeRepository.DeleteAsync` | Throws `KeyNotFoundException` |

### API layer

| Scenario | Method | Behavior |
|---|---|---|
| Service returns `null` | `EmployeesController.GetById` | Returns `NotFoundResult` (HTTP 404) |
| Service throws `KeyNotFoundException` | `EmployeesController.Update` | Catches exception, returns `NotFoundResult` (HTTP 404) |
| Service throws `KeyNotFoundException` | `EmployeesController.Delete` | Catches exception, returns `NotFoundResult` (HTTP 404) |

Tests for these error paths use xUnit's `Assert.ThrowsAsync<KeyNotFoundException>` and action-result type assertions (`Assert.IsType<NotFoundResult>`).

---

## Testing Strategy

### Dual testing approach

Each layer combines:
- **Example-based tests** for specific, fixed scenarios (default values, wiring/delegation checks, null/exception edge cases).
- **Theory-based parameterized tests** for properties that must hold across multiple representative inputs (mapping fidelity, delegation of id, result shapes).

The properties in the Correctness Properties section are implemented as xUnit `[Theory]` tests with `[MemberData]` providing multiple representative input sets (typically 3–5 varied cases per theory). This approach validates universal properties without introducing a full PBT library dependency, keeping the test setup lightweight while still exercising the input space meaningfully.

### Per-layer strategy

#### Domain layer (`AiSync.Domain.Tests`)
- One test class: `EmployeeEntityTests`.
- All tests are `[Fact]` — the entity is a simple POCO with no behavior beyond property storage.
- Cover: default `Name` is `string.Empty`, `IsActive` can be set to `true` and `false`, all properties round-trip correctly.

#### Application layer (`AiSync.Application.Tests`)
- One test class per service method.
- `IEmployeeRepository` is mocked via `Mock<IEmployeeRepository>`.
- Property tests use `[Theory] + [MemberData]` with 3–5 representative `Employee` / DTO instances.
- Wiring tests use `Mock.Verify` to assert correct arguments (including `CancellationToken`) were passed.
- `CancellationToken` is tested by passing a non-default token (e.g., `new CancellationTokenSource().Token`) and verifying it reaches the mock.

#### Infrastructure layer (`AiSync.Infrastructure.Tests`)
- One test class per repository method.
- Each test creates a fresh `AppDbContext` via `AppDbContextFactory.CreateContext()` (unique in-memory database name per test).
- Property tests seed 3–5 employee records with varied field values and assert retrieval behavior.
- `AsNoTracking` is verified by asserting `context.Entry(entity).State == EntityState.Detached` after a read.
- No Moq — only the real `EmployeeRepository` and `AppDbContext`.

#### API layer (`AiSync.API.Tests`)
- One test class per controller action.
- `IEmployeeService` is mocked via `Mock<IEmployeeService>`.
- Controllers are instantiated directly: `new EmployeesController(mockService.Object)`.
- Action results are cast/asserted using `Assert.IsType<OkObjectResult>`, `Assert.IsType<NotFoundResult>`, `Assert.IsType<NoContentResult>`, `Assert.IsType<CreatedAtActionResult>`.
- Property tests use `[Theory] + [MemberData]` with 3–5 representative `EmployeeDto` instances to validate result shapes.

### What unit tests focus on

| Focus | Covered by |
|---|---|
| Field mapping fidelity | Application layer `[Theory]` tests (Properties 1–4) |
| Delegation of parameters (id, token) | `[Fact]` wiring tests in Application and API layers |
| EF Core persistence and retrieval | Infrastructure `[Theory]` tests (Properties 6–10) |
| HTTP result type and value | API `[Theory]` and `[Fact]` tests (Properties 11–14) |
| Exception propagation | `[Fact]` edge-case tests in Application, Infrastructure, API layers |
| Default values | `[Fact]` tests in Domain layer |

### Property test configuration

Each `[Theory]` implementing a correctness property must include a comment in the following format:

```
// Feature: unit-tests, Property N: <property title>
```

Minimum **3 distinct input cases** per `[Theory]` to exercise varied field values (e.g., different names, dates, `IsActive` combinations).
