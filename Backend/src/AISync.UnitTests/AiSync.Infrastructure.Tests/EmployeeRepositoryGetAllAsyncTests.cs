using AiSync.Domain.Entities;
using AiSync.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiSync.Infrastructure.Tests;

// Feature: unit-tests, Property 6: GetAllAsync returns all seeded employees from the database
public class EmployeeRepositoryGetAllAsyncTests
{
    // ── Fact: empty database returns empty collection ─────────────────────────

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyCollection()
    {
        // Arrange
        using var context = AppDbContextFactory.CreateContext();
        var repository = new EmployeeRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    // ── Fact: returned entities are not attached to the change tracker ────────

    [Fact]
    public async Task GetAllAsync_ReturnedEntities_AreDetachedFromChangeTracker()
    {
        // Arrange
        using var context = AppDbContextFactory.CreateContext();
        context.Employees.AddRange(
            EmployeeFactory.Create(1, "Alice", new DateTime(1990, 1, 15), true),
            EmployeeFactory.Create(2, "Bob",   new DateTime(1985, 6, 20), false));
        await context.SaveChangesAsync();

        var repository = new EmployeeRepository(context);

        // Act
        var employees = (await repository.GetAllAsync()).ToList();

        // Assert — AsNoTracking means every entity is Detached
        foreach (var employee in employees)
            Assert.Equal(EntityState.Detached, context.Entry(employee).State);
    }

    // ── Theory: Property 6 — GetAllAsync returns every seeded employee by Id ──

    public static IEnumerable<object[]> SeededEmployeeCases()
    {
        // Case 1 — single active employee
        yield return new object[]
        {
            new[]
            {
                EmployeeFactory.Create(1, "Alice", new DateTime(1990, 3, 15), true),
            }
        };

        // Case 2 — two employees, mixed IsActive
        yield return new object[]
        {
            new[]
            {
                EmployeeFactory.Create(1, "Bob",     new DateTime(1985, 7, 20), true),
                EmployeeFactory.Create(2, "Charlie", new DateTime(1992, 11, 5), false),
            }
        };

        // Case 3 — three employees with varied data
        yield return new object[]
        {
            new[]
            {
                EmployeeFactory.Create(1, "Diana", new DateTime(1978, 2, 28), true),
                EmployeeFactory.Create(2, "Eve",   new DateTime(2000, 9, 10), false),
                EmployeeFactory.Create(3, "Frank", new DateTime(1995, 4, 1),  true),
            }
        };
    }

    [Theory]
    [MemberData(nameof(SeededEmployeeCases))]
    public async Task GetAllAsync_SeededEmployees_ReturnsEverySeededEmployeeById(
        Employee[] seededEmployees)
    {
        // Arrange
        using var context = AppDbContextFactory.CreateContext();
        context.Employees.AddRange(seededEmployees);
        await context.SaveChangesAsync();

        var repository = new EmployeeRepository(context);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert — every seeded Id must appear in the result
        var returnedIds = result.Select(e => e.Id).ToHashSet();
        foreach (var seeded in seededEmployees)
            Assert.Contains(seeded.Id, returnedIds);

        Assert.Equal(seededEmployees.Length, result.Count);
    }
}
