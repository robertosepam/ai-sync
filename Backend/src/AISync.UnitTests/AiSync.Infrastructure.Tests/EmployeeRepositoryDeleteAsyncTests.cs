using AiSync.Domain.Entities;
using AiSync.Infrastructure.Repositories;

namespace AiSync.Infrastructure.Tests;

// Feature: unit-tests, Property 10: DeleteAsync removes the employee and decreases count by 1
public class EmployeeRepositoryDeleteAsyncTests
{
    // ── Fact: id not in DB → throws KeyNotFoundException ─────────────────────

    [Fact]
    public async Task DeleteAsync_IdNotInDatabase_ThrowsKeyNotFoundException()
    {
        // Arrange
        using var context = AppDbContextFactory.CreateContext();
        var repository = new EmployeeRepository(context);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repository.DeleteAsync(999, CancellationToken.None));
    }

    // ── Theory: Property 10 — DeleteAsync removes the record and decreases count by 1 ──

    public static IEnumerable<object[]> SeededEmployeeForDeleteCases()
    {
        // Case 1 — single active employee
        yield return new object[]
        {
            new[]
            {
                EmployeeFactory.Create(1, "Alice", new DateTime(1990, 3, 15), true),
            },
            1, // id to delete
        };

        // Case 2 — two employees; delete the inactive one
        yield return new object[]
        {
            new[]
            {
                EmployeeFactory.Create(1, "Bob",     new DateTime(1985, 7, 20), true),
                EmployeeFactory.Create(2, "Charlie", new DateTime(1992, 11, 5), false),
            },
            2, // id to delete
        };

        // Case 3 — three employees with varied data; delete the middle one
        yield return new object[]
        {
            new[]
            {
                EmployeeFactory.Create(1, "Diana", new DateTime(1978, 2, 28), true),
                EmployeeFactory.Create(2, "Eve",   new DateTime(2000, 9, 10), false),
                EmployeeFactory.Create(3, "Frank", new DateTime(1995, 4, 1),  true),
            },
            2, // id to delete
        };
    }

    [Theory]
    [MemberData(nameof(SeededEmployeeForDeleteCases))]
    public async Task DeleteAsync_SeededEmployee_RemovesRecordAndDecreasesCountByOne(
        Employee[] seededEmployees,
        int idToDelete)
    {
        // Arrange
        using var context = AppDbContextFactory.CreateContext();
        context.Employees.AddRange(seededEmployees);
        await context.SaveChangesAsync();

        var repository = new EmployeeRepository(context);
        int countBefore = context.Employees.Count();

        // Act
        await repository.DeleteAsync(idToDelete, CancellationToken.None);

        // Assert — record is gone and total count decreased by exactly 1
        int countAfter = context.Employees.Count();
        Assert.Equal(countBefore - 1, countAfter);

        var deleted = await context.Employees.FindAsync(idToDelete);
        Assert.Null(deleted);
    }
}
