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

            entity.HasData(
                new Employee { Id = 1, Name = "Ana García",       DateOfBirth = new DateTime(1990, 3, 15), IsActive = true  },
                new Employee { Id = 2, Name = "Carlos López",     DateOfBirth = new DateTime(1985, 7, 22), IsActive = true  },
                new Employee { Id = 3, Name = "María Rodríguez",  DateOfBirth = new DateTime(1993, 11, 8), IsActive = true  },
                new Employee { Id = 4, Name = "Jorge Martínez",   DateOfBirth = new DateTime(1988, 1, 30), IsActive = false },
                new Employee { Id = 5, Name = "Laura Sánchez",    DateOfBirth = new DateTime(1995, 6, 14), IsActive = true  },
                new Employee { Id = 6, Name = "Pedro Hernández",  DateOfBirth = new DateTime(1982, 9, 5),  IsActive = true  },
                new Employee { Id = 7, Name = "Sofía Torres",     DateOfBirth = new DateTime(1997, 4, 27), IsActive = true  },
                new Employee { Id = 8, Name = "Diego Ramírez",    DateOfBirth = new DateTime(1991, 12, 3), IsActive = false },
                new Employee { Id = 9, Name = "Valentina Cruz",   DateOfBirth = new DateTime(1994, 8, 19), IsActive = true  },
                new Employee { Id = 10, Name = "Andrés Flores",   DateOfBirth = new DateTime(1987, 2, 11), IsActive = true  }
            );
        });
    }
}
