namespace AiSync.Application.DTOs;

public record ProjectDto(
    int Id,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime? EndDate,
    int EmployeeId);
