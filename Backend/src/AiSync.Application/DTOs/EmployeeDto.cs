namespace AiSync.Application.DTOs;

public record EmployeeDto(
    int Id,
    string Name,
    DateTime DateOfBirth,
    bool IsActive);
