using AiSync.Application.DTOs;

namespace AiSync.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default);
    Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
