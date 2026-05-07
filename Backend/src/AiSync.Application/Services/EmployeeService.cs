using AiSync.Application.DTOs;
using AiSync.Application.Interfaces;
using AiSync.Domain.Entities;
using AiSync.Domain.Interfaces;

namespace AiSync.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _repository.GetAllAsync(cancellationToken);
        return employees.Select(MapToDto);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        return employee is null ? null : MapToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var employee = new Employee
        {
            Name = dto.Name,
            DateOfBirth = dto.DateOfBirth,
            IsActive = dto.IsActive
        };

        var created = await _repository.AddAsync(employee, cancellationToken);
        return MapToDto(created);
    }

    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Employee with id {id} not found.");

        employee.Name = dto.Name;
        employee.DateOfBirth = dto.DateOfBirth;
        employee.IsActive = dto.IsActive;

        var updated = await _repository.UpdateAsync(employee, cancellationToken);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }

    private static EmployeeDto MapToDto(Employee e) =>
        new(e.Id, e.Name, e.DateOfBirth, e.IsActive);
}
