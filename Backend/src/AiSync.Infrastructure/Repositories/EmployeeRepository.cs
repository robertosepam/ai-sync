using AiSync.Domain.Entities;
using AiSync.Domain.Interfaces;
using AiSync.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiSync.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Employees.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new KeyNotFoundException($"Employee with id {id} not found.");
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
