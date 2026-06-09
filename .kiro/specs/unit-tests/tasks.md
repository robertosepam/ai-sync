# Implementation Plan: Unit Tests for Employee CRUD

## Overview

Add comprehensive unit tests for the Employee CRUD feature across all four Clean Architecture layers. The implementation installs missing NuGet packages, removes placeholder files, adds shared test helpers, and creates one test class per production method under test. xUnit `[Fact]` tests cover specific scenarios and edge cases; xUnit `[Theory] + [MemberData]` tests validate the correctness properties defined in the design.

## Tasks

- [x] 1. Configure test project dependencies and remove placeholder files
  - Add `Moq 4.20.72` package reference to `AiSync.Application.Tests.csproj`
  - Add `Moq 4.20.72` package reference to `AiSync.API.Tests.csproj`
  - Add `Microsoft.EntityFrameworkCore.InMemory` (EF Core 10.x compatible) to `AiSync.Infrastructure.Tests.csproj`
  - Delete `UnitTest1.cs` from all four test projects (`AiSync.Domain.Tests`, `AiSync.Application.Tests`, `AiSync.Infrastructure.Tests`, `AiSync.API.Tests`)
  - _Requirements: 17.1, 17.2, 17.3, 17.5_

- [x] 2. Domain layer — Employee entity tests
  - [x] 2.1 Implement `EmployeeEntityTests` in `AiSync.Domain.Tests`
    - Create `EmployeeEntityTests.cs` in `AiSync.Domain.Tests`
    - Write `[Fact]` tests: all four properties (`Id`, `Name`, `DateOfBirth`, `IsActive`) can be set and read back with the same values
    - Write `[Fact]` test: default value of `Name` is `string.Empty`
    - Write `[Fact]` tests: `IsActive` can be explicitly set to `true` and `false`
    - _Requirements: 1.1, 1.2, 1.3_

- [x] 3. Application layer — shared helper and GetAllAsync tests
  - [x] 3.1 Create `EmployeeFactory` helper in `AiSync.Application.Tests`
    - Add `EmployeeFactory.cs` static class with a `Create(int id, string name, DateOnly dateOfBirth, bool isActive)` method that returns a configured `Employee` instance
    - _Requirements: 2.1, 2.4_

  - [x] 3.2 Implement `EmployeeServiceGetAllAsyncTests`
    - Create `EmployeeServiceGetAllAsyncTests.cs` in `AiSync.Application.Tests`
    - Write `[Fact]` test: repository returns empty list → service returns empty `IEnumerable<EmployeeDto>`
    - Write `[Fact]` test: `CancellationToken` is passed through to `IEmployeeRepository.GetAllAsync`
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 1: for any non-empty list of employees the service returns one `EmployeeDto` per employee with all four fields mapped correctly
    - Include comment `// Feature: unit-tests, Property 1: GetAllAsync mapping preserves all Employee fields`
    - _Requirements: 2.1, 2.2, 2.3, 2.4_

  - [ ]* 3.3 Write property test for GetAllAsync mapping (Property 1)
    - **Property 1: GetAllAsync mapping preserves all Employee fields**
    - **Validates: Requirements 2.1, 2.4**

- [x] 4. Application layer — GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync tests
  - [x] 4.1 Implement `EmployeeServiceGetByIdAsyncTests`
    - Create `EmployeeServiceGetByIdAsyncTests.cs` in `AiSync.Application.Tests`
    - Write `[Fact]` test: repository returns `null` → service returns `null`
    - Write `[Fact]` test: correct `id` and `CancellationToken` are forwarded to `IEmployeeRepository.GetByIdAsync`
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 2: for any `Employee` returned by the repository the service returns an `EmployeeDto` with all four fields matching
    - Include comment `// Feature: unit-tests, Property 2: GetByIdAsync maps found Employee to EmployeeDto`
    - _Requirements: 3.1, 3.2, 3.3_

  - [x] 4.2 Implement `EmployeeServiceCreateAsyncTests`
    - Create `EmployeeServiceCreateAsyncTests.cs` in `AiSync.Application.Tests`
    - Write `[Fact]` test: when `IsActive` is not specified in `CreateEmployeeDto`, the entity passed to `AddAsync` has `IsActive == true`
    - Write `[Fact]` test: `CancellationToken` is forwarded to `IEmployeeRepository.AddAsync`
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 3: for any `CreateEmployeeDto`, `AddAsync` receives an entity with matching fields and the service returns a `EmployeeDto` mapped from the repository's returned entity
    - Include comment `// Feature: unit-tests, Property 3: CreateAsync maps DTO to entity and result DTO correctly`
    - _Requirements: 4.1, 4.2, 4.3, 4.4_

  - [x] 4.3 Implement `EmployeeServiceUpdateAsyncTests`
    - Create `EmployeeServiceUpdateAsyncTests.cs` in `AiSync.Application.Tests`
    - Write `[Fact]` test: repository returns `null` for `GetByIdAsync` → service throws `KeyNotFoundException`
    - Write `[Fact]` test: `CancellationToken` is forwarded to both `GetByIdAsync` and `UpdateAsync`
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 4: for any existing employee and `UpdateEmployeeDto`, `UpdateAsync` is called with mutated entity fields matching the DTO and the returned `EmployeeDto` reflects those fields
    - Include comment `// Feature: unit-tests, Property 4: UpdateAsync mutates entity fields and returns correct DTO`
    - _Requirements: 5.1, 5.2, 5.3, 5.4_

  - [x] 4.4 Implement `EmployeeServiceDeleteAsyncTests`
    - Create `EmployeeServiceDeleteAsyncTests.cs` in `AiSync.Application.Tests`
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 5: for any integer `id`, `DeleteAsync` calls `IEmployeeRepository.DeleteAsync` with that exact `id`
    - Include comment `// Feature: unit-tests, Property 5: DeleteAsync passes id through to repository unchanged`
    - Write `[Fact]` test: `CancellationToken` is forwarded to `IEmployeeRepository.DeleteAsync`
    - _Requirements: 6.1, 6.2_

- [~] 5. Checkpoint — Application layer tests
  - Ensure all tests pass, ask the user if questions arise.

- [x] 6. Infrastructure layer — shared helpers and GetAllAsync / GetByIdAsync tests
  - [x] 6.1 Create `AppDbContextFactory` and `EmployeeFactory` helpers in `AiSync.Infrastructure.Tests`
    - Add `AppDbContextFactory.cs` with `CreateContext()` static method using `UseInMemoryDatabase(Guid.NewGuid().ToString())`
    - Add `EmployeeFactory.cs` static class mirroring the Application.Tests helper for seeding entities
    - _Requirements: 9.1, 7.1_

  - [x] 6.2 Implement `EmployeeRepositoryGetAllAsyncTests`
    - Create `EmployeeRepositoryGetAllAsyncTests.cs` in `AiSync.Infrastructure.Tests`
    - Write `[Fact]` test: empty database → returns empty collection
    - Write `[Fact]` test: returned entities are not attached to the change tracker (`EntityState.Detached`)
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 6: for any non-empty set of seeded employees `GetAllAsync` returns every seeded employee matched by `Id`
    - Include comment `// Feature: unit-tests, Property 6: GetAllAsync returns all seeded employees from the database`
    - _Requirements: 7.1, 7.2, 7.3_

  - [x] 6.3 Implement `EmployeeRepositoryGetByIdAsyncTests`
    - Create `EmployeeRepositoryGetByIdAsyncTests.cs` in `AiSync.Infrastructure.Tests`
    - Write `[Fact]` test: `id` not present in DB → returns `null`
    - Write `[Fact]` test: returned entity is not attached to the change tracker
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 7: for any `Employee` stored in the in-memory DB, `GetByIdAsync(employee.Id)` returns an entity with all four fields matching
    - Include comment `// Feature: unit-tests, Property 7: GetByIdAsync returns the Employee matching the queried id`
    - _Requirements: 8.1, 8.2, 8.3_

- [ ] 7. Infrastructure layer — AddAsync, UpdateAsync, DeleteAsync tests
  - [x] 7.1 Implement `EmployeeRepositoryAddAsyncTests`
    - Create `EmployeeRepositoryAddAsyncTests.cs` in `AiSync.Infrastructure.Tests`
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 8: for any valid `Employee`, `AddAsync` persists the record (verifiable by re-query), returns an entity with `Id > 0`, and increments total count by exactly 1
    - Include comment `// Feature: unit-tests, Property 8: AddAsync persists the employee and assigns a positive Id`
    - _Requirements: 9.1, 9.2, 9.3_

  - [x] 7.2 Implement `EmployeeRepositoryUpdateAsyncTests`
    - Create `EmployeeRepositoryUpdateAsyncTests.cs` in `AiSync.Infrastructure.Tests`
    - Write `[Fact]` test: setting `IsActive` to `false` persists `false` (soft-delete)
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 9: for any seeded employee and new field values, `UpdateAsync` persists all three changed fields and returns an entity with those updated values
    - Include comment `// Feature: unit-tests, Property 9: UpdateAsync persists all modified fields and returns updated entity`
    - _Requirements: 10.1, 10.2, 10.3_

  - [-] 7.3 Implement `EmployeeRepositoryDeleteAsyncTests`
    - Create `EmployeeRepositoryDeleteAsyncTests.cs` in `AiSync.Infrastructure.Tests`
    - Write `[Fact]` test: `id` not in DB → throws `KeyNotFoundException`
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 10: for any seeded employee, `DeleteAsync(employee.Id)` removes the record and reduces total count by exactly 1
    - Include comment `// Feature: unit-tests, Property 10: DeleteAsync removes the employee and decreases count by 1`
    - _Requirements: 11.1, 11.2, 11.3_

- [~] 8. Checkpoint — Infrastructure layer tests
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 9. API layer — GetAll and GetById controller tests
  - [-] 9.1 Implement `EmployeesControllerGetAllTests`
    - Create `EmployeesControllerGetAllTests.cs` in `AiSync.API.Tests`
    - Instantiate controller directly: `new EmployeesController(mockService.Object)`
    - Write `[Fact]` test: service returns empty list → result is `OkObjectResult` containing empty collection
    - Write `[Fact]` test: `CancellationToken` is forwarded to `IEmployeeService.GetAllAsync`
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 11: for any `IEnumerable<EmployeeDto>` (including empty) returned by the service, `GetAll` returns an `OkObjectResult` whose `Value` is that exact collection
    - Include comment `// Feature: unit-tests, Property 11: GetAll controller wraps any service result in OkObjectResult`
    - _Requirements: 12.1, 12.2, 12.3_

  - [ ] 9.2 Implement `EmployeesControllerGetByIdTests`
    - Create `EmployeesControllerGetByIdTests.cs` in `AiSync.API.Tests`
    - Write `[Fact]` test: service returns `null` → result is `NotFoundResult`
    - Write `[Fact]` test: correct `id` and `CancellationToken` are forwarded to `IEmployeeService.GetByIdAsync`
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 12: for any `EmployeeDto` returned by the service, `GetById` returns an `OkObjectResult` whose `Value` is that exact `EmployeeDto`
    - Include comment `// Feature: unit-tests, Property 12: GetById controller wraps found EmployeeDto in OkObjectResult`
    - _Requirements: 13.1, 13.2, 13.3_

- [ ] 10. API layer — Create, Update, Delete controller tests
  - [~] 10.1 Implement `EmployeesControllerCreateTests`
    - Create `EmployeesControllerCreateTests.cs` in `AiSync.API.Tests`
    - Write `[Fact]` test: `CreateEmployeeDto` and `CancellationToken` are forwarded to `IEmployeeService.CreateAsync`
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 13: for any `EmployeeDto` returned by the service, `Create` returns a `CreatedAtActionResult` with `ActionName == "GetById"` and `RouteValues["id"]` equal to `EmployeeDto.Id`
    - Include comment `// Feature: unit-tests, Property 13: Create controller returns CreatedAtActionResult with correct shape`
    - _Requirements: 14.1, 14.2_

  - [~] 10.2 Implement `EmployeesControllerUpdateTests`
    - Create `EmployeesControllerUpdateTests.cs` in `AiSync.API.Tests`
    - Write `[Fact]` test: service throws `KeyNotFoundException` → result is `NotFoundResult`
    - Write `[Fact]` test: `id`, `UpdateEmployeeDto`, and `CancellationToken` are forwarded to `IEmployeeService.UpdateAsync`
    - Write `[Theory] + [MemberData]` test (≥ 3 cases) implementing Property 14: for any `EmployeeDto` returned by the service, `Update` returns an `OkObjectResult` whose `Value` is that exact `EmployeeDto`
    - Include comment `// Feature: unit-tests, Property 14: Update controller wraps returned EmployeeDto in OkObjectResult`
    - _Requirements: 15.1, 15.2, 15.3_

  - [~] 10.3 Implement `EmployeesControllerDeleteTests`
    - Create `EmployeesControllerDeleteTests.cs` in `AiSync.API.Tests`
    - Write `[Fact]` test: service completes without error → result is `NoContentResult`
    - Write `[Fact]` test: service throws `KeyNotFoundException` → result is `NotFoundResult`
    - Write `[Fact]` test: correct `id` and `CancellationToken` are forwarded to `IEmployeeService.DeleteAsync`
    - _Requirements: 16.1, 16.2, 16.3_

- [~] 11. Final checkpoint — full test suite
  - Run `dotnet test Backend/AI-Sync-Backend.slnx` and ensure all tests compile and pass with no errors.
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation at each layer boundary
- Property tests are `[Theory] + [MemberData]` xUnit theories (≥ 3 distinct input cases each) — no additional PBT library required
- Unit tests validate specific examples and edge cases; theory tests validate universal correctness properties
- Every test class shares one `Mock<T>` setup per class (constructed in a field or constructor), keeping boilerplate minimal
- `AppDbContextFactory.CreateContext()` uses a unique GUID database name per call, guaranteeing test isolation in the Infrastructure layer

## Task Dependency Graph

```json
{
  "waves": [
    { "id": 0, "tasks": ["1"] },
    { "id": 1, "tasks": ["2.1", "3.1", "6.1"] },
    { "id": 2, "tasks": ["3.2", "4.1", "4.2", "4.3", "4.4", "6.2", "6.3"] },
    { "id": 3, "tasks": ["3.3", "7.1", "7.2", "7.3", "9.1", "9.2"] },
    { "id": 4, "tasks": ["10.1", "10.2", "10.3"] }
  ]
}
```
