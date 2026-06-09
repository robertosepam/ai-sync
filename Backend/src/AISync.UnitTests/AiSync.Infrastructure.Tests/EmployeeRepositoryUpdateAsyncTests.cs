using AiSync.Domain.Entities;
using AiSync.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiSync.Infrastructure.Tests;

// Feature: unit-tests, Property 9: UpdateAsync persists all modified fields and returns updated entity
public class EmployeeRepositoryUpdateAsyncTests
{
    // ── Fact: setting IsActive to false persists false (soft-delete) ──────────

    [Fact]
    public async Task UpdateAsync_SetIsActiveToFalse_PersistsFalse()
    {
        // Arrange — seed via a short-lived context, then open a fresh one for the SUT
        using var seedContext = AppDbContextFactory.CreateContext();
        var seed = EmployeeFactory.Create(0, "Alice", new DateTime(1990, 1, 15), true);
        seedContext.Employees.Add(seed);
        await seedContext.SaveChangesAsync();
        int assignedId = seed.Id;

        using var context = AppDbContextFactory.CreateContext();
        // Seed the same record into the test context
        var seeded = EmployeeFactory.Create(assignedId, seed.Name, seed.DateOfBirth, true);
        context.Employees.Add(seeded);
        await context.SaveChangesAsync();

        var repository = new EmployeeRepository(context);

        // Fetch the tracked entity, mutate only IsActive
        var tracked = await context.Employees.FindAsync(assignedId);
        Assert.NotNull(tracked);
        tracked.IsActive = false;

        // Act
        var returned = await repository.UpdateAsync(tracked, CancellationToken.None);

        // Assert — returned entity reflects the change
        Assert.False(returned.IsActive);

        // Re-query within the same context to confirm persistence
        context.ChangeTracker.Clear();
        var persisted = await context.Employees.FindAsync(assignedId);
        Assert.NotNull(persisted);
        Assert.False(persisted.IsActive);
    }

    // ── Theory: Property 9 — UpdateAsync persists all modified fields ─────────

    public static IEnumerable<object[]> UpdateCases()
    {
        // Case 1 — rename + change DateOfBirth + deactivate
        yield return new object[]
        {
            "Alice", new DateTime(1990, 3, 15), true,
            "Alice Updated", new DateTime(1991, 6, 20), false,
        };

        // Case 2 — rename only, keep DateOfBirth, keep IsActive = false
        yield return new object[]
        {
            "Bob", new DateTime(1985, 7, 20), false,
            "Robert", new DateTime(1985, 7, 20), false,
        };

        // Case 3 — reactivate + new DateOfBirth
        yield return new object[]
        {
            "Charlie", new DateTime(1972, 11, 5), false,
            "Charlie", new DateTime(1973, 1, 1), true,
        };

        // Case 4 — all three fields change simultaneously
        yield return new object[]
        {
            "Diana", new DateTime(1978, 2, 28), true,
            "Diana Prince", new DateTime(1980, 5, 15), false,
        };
    }

    [Theory]
    [MemberData(nameof(UpdateCases))]
    public async Task UpdateAsync_ModifiedEmployee_PersistsAllChangedFieldsAndReturnsUpdatedEntity(
        string seedName, DateTime seedDob, bool seedIsActive,
        string newName, DateTime newDob, bool newIsActive)
    {
        // Arrange — use a single context; seed the entity first
        using var context = AppDbContextFactory.CreateContext();
        var seed = new Employee { Name = seedName, DateOfBirth = seedDob, IsActive = seedIsActive };
        context.Employees.Add(seed);
        await context.SaveChangesAsync();

        var repository = new EmployeeRepository(context);

        // Fetch the tracked entity and mutate its fields
        var tracked = await context.Employees.FindAsync(seed.Id);
        Assert.NotNull(tracked);
        tracked.Name = newName;
        tracked.DateOfBirth = newDob;
        tracked.IsActive = newIsActive;

        // Act
        var returned = await repository.UpdateAsync(tracked, CancellationToken.None);

        // Assert — returned entity has the updated values
        Assert.Equal(seed.Id, returned.Id);
        Assert.Equal(newName, returned.Name);
        Assert.Equal(newDob, returned.DateOfBirth);
        Assert.Equal(newIsActive, returned.IsActive);

        // Clear tracker and re-query to confirm persistence
        context.ChangeTracker.Clear();
        var persisted = await context.Employees.FindAsync(seed.Id);
        Assert.NotNull(persisted);
        Assert.Equal(newName, persisted.Name);
        Assert.Equal(newDob, persisted.DateOfBirth);
        Assert.Equal(newIsActive, persisted.IsActive);
    }
}
