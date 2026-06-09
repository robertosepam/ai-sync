using AiSync.Domain.Entities;
using AiSync.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiSync.Infrastructure.Tests;

// Feature: unit-tests, Property 7: GetByIdAsync returns the Employee matching the queried id
public class EmployeeRepositoryGetByIdAsyncTests
{
    // ── Fact: id not present in DB returns null ───────────────────────────────

    [Fact]
    public async Task GetByIdAsync_IdNotInDatabase_ReturnsNull()
    {
        // Arrange
        using var context = AppDbContextFactory.CreateContext();
        var repository = new EmployeeRepository(context);

        // Act
        var result = await repository.GetByIdAsync(999, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    // ── Fact: returned entity is not attached to the change tracker ───────────

    [Fact]
    public async Task GetByIdAsync_ReturnedEntity_IsDetachedFromChangeTracker()
    {
        // Arrange
        using var context = AppDbContextFactory.CreateContext();
        context.Employees.Add(EmployeeFactory.Create(1, "Alice", new DateTime(1990, 1, 15), true));
        await context.SaveChangesAsync();

        var repository = new EmployeeRepository(context);

        // Act
        var result = await repository.GetByIdAsync(1, CancellationToken.None);

        // Assert — AsNoTracking means the returned entity is Detached
        Assert.NotNull(result);
        Assert.Equal(EntityState.Detached, context.Entry(result).State);
    }

    // ── Theory: Property 7 — GetByIdAsync returns the Employee matching the queried id ──

    public static IEnumerable<object[]> StoredEmployeeCases()
    {
        // Case 1 — active employee with a recent date of birth
        yield return new object[]
        {
            EmployeeFactory.Create(1, "Alice", new DateTime(1990, 3, 15), true),
        };

        // Case 2 — inactive employee (soft-deleted via IsActive = false)
        yield return new object[]
        {
            EmployeeFactory.Create(2, "Bob", new DateTime(1985, 7, 20), false),
        };

        // Case 3 — employee with an older date of birth and IsActive = true
        yield return new object[]
        {
            EmployeeFactory.Create(3, "Charlie", new DateTime(1972, 11, 5), true),
        };
    }

    [Theory]
    [MemberData(nameof(StoredEmployeeCases))]
    public async Task GetByIdAsync_StoredEmployee_ReturnsEntityWithAllFieldsMatching(
        Employee seededEmployee)
    {
        // Arrange
        using var context = AppDbContextFactory.CreateContext();
        context.Employees.Add(seededEmployee);
        await context.SaveChangesAsync();

        var repository = new EmployeeRepository(context);

        // Act
        var result = await repository.GetByIdAsync(seededEmployee.Id, CancellationToken.None);

        // Assert — all four fields must match the seeded record
        Assert.NotNull(result);
        Assert.Equal(seededEmployee.Id, result.Id);
        Assert.Equal(seededEmployee.Name, result.Name);
        Assert.Equal(seededEmployee.DateOfBirth, result.DateOfBirth);
        Assert.Equal(seededEmployee.IsActive, result.IsActive);
    }
}
