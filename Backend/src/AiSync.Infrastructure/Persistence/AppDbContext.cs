using AiSync.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiSync.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DateOfBirth).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        });
    }
}
