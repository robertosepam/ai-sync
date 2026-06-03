using System.ComponentModel.DataAnnotations;

namespace AiSync.Application.DTOs;

public record UpdateProjectDto(
    [Required, MaxLength(200)] string Name,
    [MaxLength(1000)] string Description,
    [Required] DateTime StartDate,
    DateTime? EndDate,
    [Required] int EmployeeId);
