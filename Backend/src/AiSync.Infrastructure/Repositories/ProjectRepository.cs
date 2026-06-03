using AiSync.Domain.Entities;
using AiSync.Domain.Interfaces;
using AiSync.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiSync.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Projects.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _context.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IEnumerable<Project>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default) =>
        await _context.Projects.AsNoTracking().Where(p => p.EmployeeId == employeeId).ToListAsync(cancellationToken);

    public async Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);

        return project;
    }

    public async Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync(cancellationToken);

        return project;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new KeyNotFoundException($"Project with id {id} not found.");
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
