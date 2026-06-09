using AiSync.Domain.Entities;
using AiSync.Infrastructure.Repositories;

namespace AiSync.Infrastructure.Tests;

// Feature: unit-tests, Property 8: AddAsync persists the employee and assigns a positive Id
public class EmployeeRepositoryAddAsyncTests
{
    // ── Theory: Property 8 — AddAsync persists the employee and assigns a positive Id ──

    public static IEnumerable<object[]> ValidEmployeeCases()
    {
        // Case 1 — active employee
        yield return new object[]
        {
            EmployeeFactory.Create(0, "Alice", new DateTime(1990, 3, 15), true),
        };

        // Case 2 — inactive employee (IsActive = false / soft-delete scenario)
        yield return new object[]
        {
            EmployeeFactory.Create(0, "Bob", new DateTime(1985, 7, 20), false),
        };

        // Case 3 — employee with an older date of birth
        yield return new object[]
        {
            EmployeeFactory.Create(0, "Charlie", new DateTime(1972, 11, 5), true),
        };

        // Case 4 — employee with special characters in the name
        yield return new object[]
        {
            EmployeeFactory.Create(0, "Diana López", new DateTime(2000, 1, 31), true),
        };
    }

    [Theory]
    [MemberData(nameof(ValidEmployeeCases))]
    public async Task AddAsync_ValidEmployee_PersistsRecordAssignsPositiveIdAndIncrementsCountByOne(
        Employee employee)
    {
        // Arrange
        using var context = AppDbContextFactory.CreateContext();
        var repository = new EmployeeRepository(context);
        var countBefore = context.Employees.Count();

        // Act
        var returned = await repository.AddAsync(employee, CancellationToken.None);

        // Assert — Id must be positive (database-assigned)
        Assert.True(returned.Id > 0, "AddAsync must return an entity with Id > 0.");

        // Assert — count incremented by exactly 1
        var countAfter = context.Employees.Count();
        Assert.Equal(countBefore + 1, countAfter);

        // Assert — record is verifiable by re-query
        var persisted = await context.Employees.FindAsync(returned.Id);
        Assert.NotNull(persisted);
        Assert.Equal(employee.Name,        persisted.Name);
        Assert.Equal(employee.DateOfBirth, persisted.DateOfBirth);
        Assert.Equal(employee.IsActive,    persisted.IsActive);
    }
}
