using AiSync.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiSync.Infrastructure.Tests;

internal static class AppDbContextFactory
{
    public static AppDbContext CreateContext() =>
        new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);
}
