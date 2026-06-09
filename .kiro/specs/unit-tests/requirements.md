# Requirements Document

## Introduction

This feature adds comprehensive unit tests for the existing Employee feature across all four Clean Architecture layers: Domain, Application, Infrastructure, and API. The tests cover CRUD operations and soft-delete (via `IsActive`), using xUnit and Moq for mocking, and EF Core's in-memory provider for repository tests. Each test project targets only the layer it verifies, following the existing project structure under `Backend/src/AISync.UnitTests/`.

## Glossary

- **EmployeesController**: The ASP.NET Core controller in `AiSync.API` that handles HTTP requests for the Employee resource.
- **EmployeeService**: The service in `AiSync.Application` that implements `IEmployeeService` and orchestrates Employee business logic.
- **EmployeeRepository**: The EF Core repository in `AiSync.Infrastructure` that implements `IEmployeeRepository` against `AppDbContext`.
- **Employee**: The domain entity in `AiSync.Domain.Entities` with properties `Id`, `Name`, `DateOfBirth`, and `IsActive`.
- **EmployeeDto**: The response record returned by the service layer containing `Id`, `Name`, `DateOfBirth`, and `IsActive`.
- **CreateEmployeeDto**: The request record used to create a new employee, with `Name`, `DateOfBirth`, and `IsActive` (defaults to `true`).
- **UpdateEmployeeDto**: The request record used to update an existing employee, with `Name`, `DateOfBirth`, and `IsActive`.
- **IEmployeeRepository**: The repository abstraction defined in the Domain layer.
- **IEmployeeService**: The service abstraction defined in the Application layer.
- **Test_Suite**: The collective set of all four xUnit test projects under `AISync.UnitTests/`.
- **SUT**: System Under Test — the class being exercised in a given test.
- **CancellationToken**: A `System.Threading.CancellationToken` passed to every async method.
- **AppDbContext**: The EF Core `DbContext` used by `EmployeeRepository`.

---

## Requirements

### Requirement 1: Domain Layer — Employee Entity Tests

**User Story:** As a developer, I want tests for the `Employee` entity, so that I can verify that its properties are correctly assignable and hold expected values.

#### Acceptance Criteria

1. THE `Employee` Entity_Tests SHALL verify that `Id`, `Name`, `DateOfBirth`, and `IsActive` properties can be set and read back with the same values.
2. THE `Employee` Entity_Tests SHALL verify that the default value of `Name` is an empty string.
3. THE `Employee` Entity_Tests SHALL verify that `IsActive` can be explicitly set to both `true` and `false`.

---

### Requirement 2: Application Layer — EmployeeService.GetAllAsync Tests

**User Story:** As a developer, I want tests for `EmployeeService.GetAllAsync`, so that I can verify the service correctly retrieves and maps all employees from the repository.

#### Acceptance Criteria

1. WHEN `GetAllAsync` is called and the repository returns a non-empty list, THE `EmployeeService` SHALL return an `IEnumerable<EmployeeDto>` containing one `EmployeeDto` per `Employee` returned by the repository.
2. WHEN `GetAllAsync` is called and the repository returns an empty list, THE `EmployeeService` SHALL return an empty `IEnumerable<EmployeeDto>`.
3. WHEN `GetAllAsync` is called, THE `EmployeeService` SHALL pass the `CancellationToken` to `IEmployeeRepository.GetAllAsync`.
4. FOR ALL lists of employees returned by the repository, THE `EmployeeService` SHALL map each `Employee.Id`, `Employee.Name`, `Employee.DateOfBirth`, and `Employee.IsActive` to the corresponding fields of the returned `EmployeeDto` (mapping round-trip property).

---

### Requirement 3: Application Layer — EmployeeService.GetByIdAsync Tests

**User Story:** As a developer, I want tests for `EmployeeService.GetByIdAsync`, so that I can verify the service returns the correct employee or null based on repository results.

#### Acceptance Criteria

1. WHEN `GetByIdAsync` is called with a valid `id` and the repository returns a matching `Employee`, THE `EmployeeService` SHALL return an `EmployeeDto` with fields mapped from that `Employee`.
2. WHEN `GetByIdAsync` is called with an `id` for which the repository returns `null`, THE `EmployeeService` SHALL return `null`.
3. WHEN `GetByIdAsync` is called, THE `EmployeeService` SHALL pass the given `id` and `CancellationToken` to `IEmployeeRepository.GetByIdAsync`.

---

### Requirement 4: Application Layer — EmployeeService.CreateAsync Tests

**User Story:** As a developer, I want tests for `EmployeeService.CreateAsync`, so that I can verify the service builds the entity correctly and returns the mapped DTO.

#### Acceptance Criteria

1. WHEN `CreateAsync` is called with a valid `CreateEmployeeDto`, THE `EmployeeService` SHALL call `IEmployeeRepository.AddAsync` with an `Employee` whose `Name`, `DateOfBirth`, and `IsActive` match the DTO.
2. WHEN `CreateAsync` is called with a `CreateEmployeeDto` where `IsActive` is not specified, THE `EmployeeService` SHALL call `IEmployeeRepository.AddAsync` with an `Employee` where `IsActive` is `true`.
3. WHEN `CreateAsync` is called and the repository returns the persisted `Employee`, THE `EmployeeService` SHALL return an `EmployeeDto` with all fields mapped from that persisted entity.
4. WHEN `CreateAsync` is called, THE `EmployeeService` SHALL pass the `CancellationToken` to `IEmployeeRepository.AddAsync`.

---

### Requirement 5: Application Layer — EmployeeService.UpdateAsync Tests

**User Story:** As a developer, I want tests for `EmployeeService.UpdateAsync`, so that I can verify the service applies changes correctly and raises the expected exception when the employee does not exist.

#### Acceptance Criteria

1. WHEN `UpdateAsync` is called with a valid `id` and a valid `UpdateEmployeeDto`, and the repository finds the employee, THE `EmployeeService` SHALL mutate the entity's `Name`, `DateOfBirth`, and `IsActive` to match the DTO before calling `IEmployeeRepository.UpdateAsync`.
2. WHEN `UpdateAsync` is called with a valid `id` and the repository returns the updated `Employee`, THE `EmployeeService` SHALL return an `EmployeeDto` with all fields mapped from the updated entity.
3. WHEN `UpdateAsync` is called with an `id` for which the repository returns `null`, THE `EmployeeService` SHALL throw a `KeyNotFoundException`.
4. WHEN `UpdateAsync` is called, THE `EmployeeService` SHALL pass the `CancellationToken` to `IEmployeeRepository.GetByIdAsync` and `IEmployeeRepository.UpdateAsync`.

---

### Requirement 6: Application Layer — EmployeeService.DeleteAsync Tests

**User Story:** As a developer, I want tests for `EmployeeService.DeleteAsync`, so that I can verify the service delegates deletion to the repository correctly.

#### Acceptance Criteria

1. WHEN `DeleteAsync` is called with a valid `id`, THE `EmployeeService` SHALL call `IEmployeeRepository.DeleteAsync` with that same `id`.
2. WHEN `DeleteAsync` is called, THE `EmployeeService` SHALL pass the `CancellationToken` to `IEmployeeRepository.DeleteAsync`.

---

### Requirement 7: Infrastructure Layer — EmployeeRepository.GetAllAsync Tests

**User Story:** As a developer, I want tests for `EmployeeRepository.GetAllAsync`, so that I can verify that all seeded employees are returned and the query uses `AsNoTracking`.

#### Acceptance Criteria

1. WHEN `GetAllAsync` is called on an `AppDbContext` seeded with multiple employees, THE `EmployeeRepository` SHALL return all seeded employees.
2. WHEN `GetAllAsync` is called on an empty `AppDbContext`, THE `EmployeeRepository` SHALL return an empty collection.
3. WHEN `GetAllAsync` is called, THE `EmployeeRepository` SHALL not attach returned entities to the `AppDbContext` change tracker.

---

### Requirement 8: Infrastructure Layer — EmployeeRepository.GetByIdAsync Tests

**User Story:** As a developer, I want tests for `EmployeeRepository.GetByIdAsync`, so that I can verify the repository retrieves the correct entity or returns null.

#### Acceptance Criteria

1. WHEN `GetByIdAsync` is called with an `id` that exists in the database, THE `EmployeeRepository` SHALL return the `Employee` whose `Id` matches the given `id`.
2. WHEN `GetByIdAsync` is called with an `id` that does not exist in the database, THE `EmployeeRepository` SHALL return `null`.
3. WHEN `GetByIdAsync` is called, THE `EmployeeRepository` SHALL not attach the returned entity to the `AppDbContext` change tracker.

---

### Requirement 9: Infrastructure Layer — EmployeeRepository.AddAsync Tests

**User Story:** As a developer, I want tests for `EmployeeRepository.AddAsync`, so that I can verify the entity is persisted and the assigned Id is returned.

#### Acceptance Criteria

1. WHEN `AddAsync` is called with a valid `Employee`, THE `EmployeeRepository` SHALL persist the employee to the database.
2. WHEN `AddAsync` is called and the entity is saved, THE `EmployeeRepository` SHALL return the same `Employee` instance with a generated `Id` greater than `0`.
3. WHEN `AddAsync` is called with a valid `Employee`, THE `EmployeeRepository` SHALL increment the total employee count in the database by exactly `1`.

---

### Requirement 10: Infrastructure Layer — EmployeeRepository.UpdateAsync Tests

**User Story:** As a developer, I want tests for `EmployeeRepository.UpdateAsync`, so that I can verify the entity's changes are persisted correctly.

#### Acceptance Criteria

1. WHEN `UpdateAsync` is called with a modified `Employee`, THE `EmployeeRepository` SHALL persist the updated `Name`, `DateOfBirth`, and `IsActive` values to the database.
2. WHEN `UpdateAsync` is called with a modified `Employee`, THE `EmployeeRepository` SHALL return the same `Employee` instance with the updated values.
3. WHEN `UpdateAsync` is called to set `IsActive` to `false`, THE `EmployeeRepository` SHALL persist `IsActive` as `false` (soft-delete via flag).

---

### Requirement 11: Infrastructure Layer — EmployeeRepository.DeleteAsync Tests

**User Story:** As a developer, I want tests for `EmployeeRepository.DeleteAsync`, so that I can verify the entity is removed and the correct exception is thrown when it does not exist.

#### Acceptance Criteria

1. WHEN `DeleteAsync` is called with an `id` that exists in the database, THE `EmployeeRepository` SHALL remove the corresponding `Employee` from the database.
2. WHEN `DeleteAsync` is called with an `id` that exists, THE `EmployeeRepository` SHALL reduce the total employee count in the database by exactly `1`.
3. WHEN `DeleteAsync` is called with an `id` that does not exist in the database, THE `EmployeeRepository` SHALL throw a `KeyNotFoundException`.

---

### Requirement 12: API Layer — EmployeesController.GetAll Tests

**User Story:** As a developer, I want tests for `EmployeesController.GetAll`, so that I can verify the controller returns HTTP 200 with the service's result.

#### Acceptance Criteria

1. WHEN `GetAll` is called and the service returns a list of employees, THE `EmployeesController` SHALL return an `OkObjectResult` containing the list.
2. WHEN `GetAll` is called and the service returns an empty list, THE `EmployeesController` SHALL return an `OkObjectResult` containing an empty collection.
3. WHEN `GetAll` is called, THE `EmployeesController` SHALL pass the `CancellationToken` to `IEmployeeService.GetAllAsync`.

---

### Requirement 13: API Layer — EmployeesController.GetById Tests

**User Story:** As a developer, I want tests for `EmployeesController.GetById`, so that I can verify the controller returns HTTP 200 for found employees and HTTP 404 for missing ones.

#### Acceptance Criteria

1. WHEN `GetById` is called with a valid `id` and the service returns an `EmployeeDto`, THE `EmployeesController` SHALL return an `OkObjectResult` containing that `EmployeeDto`.
2. WHEN `GetById` is called with an `id` for which the service returns `null`, THE `EmployeesController` SHALL return a `NotFoundResult`.
3. WHEN `GetById` is called, THE `EmployeesController` SHALL pass the given `id` and `CancellationToken` to `IEmployeeService.GetByIdAsync`.

---

### Requirement 14: API Layer — EmployeesController.Create Tests

**User Story:** As a developer, I want tests for `EmployeesController.Create`, so that I can verify the controller returns HTTP 201 with the correct location header.

#### Acceptance Criteria

1. WHEN `Create` is called with a valid `CreateEmployeeDto` and the service returns the created `EmployeeDto`, THE `EmployeesController` SHALL return a `CreatedAtActionResult` with `ActionName` equal to `"GetById"` and the new employee's `Id` in the route values.
2. WHEN `Create` is called, THE `EmployeesController` SHALL pass the `CreateEmployeeDto` and `CancellationToken` to `IEmployeeService.CreateAsync`.

---

### Requirement 15: API Layer — EmployeesController.Update Tests

**User Story:** As a developer, I want tests for `EmployeesController.Update`, so that I can verify the controller returns HTTP 200 on success and HTTP 404 when the employee is not found.

#### Acceptance Criteria

1. WHEN `Update` is called with a valid `id` and `UpdateEmployeeDto` and the service returns the updated `EmployeeDto`, THE `EmployeesController` SHALL return an `OkObjectResult` containing that `EmployeeDto`.
2. WHEN `Update` is called and the service throws a `KeyNotFoundException`, THE `EmployeesController` SHALL return a `NotFoundResult`.
3. WHEN `Update` is called, THE `EmployeesController` SHALL pass the `id`, `UpdateEmployeeDto`, and `CancellationToken` to `IEmployeeService.UpdateAsync`.

---

### Requirement 16: API Layer — EmployeesController.Delete Tests

**User Story:** As a developer, I want tests for `EmployeesController.Delete`, so that I can verify the controller returns HTTP 204 on success and HTTP 404 when the employee is not found.

#### Acceptance Criteria

1. WHEN `Delete` is called with a valid `id` and the service completes without error, THE `EmployeesController` SHALL return a `NoContentResult`.
2. WHEN `Delete` is called and the service throws a `KeyNotFoundException`, THE `EmployeesController` SHALL return a `NotFoundResult`.
3. WHEN `Delete` is called, THE `EmployeesController` SHALL pass the given `id` and `CancellationToken` to `IEmployeeService.DeleteAsync`.

---

### Requirement 17: Test Project Setup and Dependencies

**User Story:** As a developer, I want the test projects properly configured with the right dependencies, so that all tests can be run with `dotnet test`.

#### Acceptance Criteria

1. THE `AiSync.Application.Tests` project SHALL reference the `Moq` package at version `4.20.72` or later for mocking `IEmployeeRepository`.
2. THE `AiSync.API.Tests` project SHALL reference the `Moq` package at version `4.20.72` or later for mocking `IEmployeeService`.
3. THE `AiSync.Infrastructure.Tests` project SHALL reference `Microsoft.EntityFrameworkCore.InMemory` at a version compatible with EF Core `10.x` for in-memory database testing.
4. WHEN `dotnet test Backend/AI-Sync-Backend.slnx` is executed, THE Test_Suite SHALL compile without errors and all tests SHALL pass.
5. THE Test_Suite SHALL remove the placeholder `UnitTest1.cs` files from all four test projects before adding real test classes.
