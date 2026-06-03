using AiSync.Application.DTOs;
using AiSync.Application.Interfaces;
using AiSync.Domain.Entities;
using AiSync.Domain.Interfaces;

namespace AiSync.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;

    public ProjectService(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var projects = await _repository.GetAllAsync(cancellationToken);

        return projects.Select(MapToDto);
    }

    public async Task<ProjectDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await _repository.GetByIdAsync(id, cancellationToken);

        return project is null ? null : MapToDto(project);
    }

    public async Task<IEnumerable<ProjectDto>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var projects = await _repository.GetByEmployeeIdAsync(employeeId, cancellationToken);

        return projects.Select(MapToDto);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            EmployeeId = dto.EmployeeId
        };

        var created = await _repository.AddAsync(project, cancellationToken);

        return MapToDto(created);
    }

    public async Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto dto, CancellationToken cancellationToken = default)
    {
        var project = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Project with id {id} not found.");

        project.Name = dto.Name;
        project.Description = dto.Description;
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;
        project.EmployeeId = dto.EmployeeId;

        var updated = await _repository.UpdateAsync(project, cancellationToken);

        return MapToDto(updated);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }

    private static ProjectDto MapToDto(Project p) =>
        new(p.Id, p.Name, p.Description, p.StartDate, p.EndDate, p.EmployeeId);
}
