using System.ComponentModel.DataAnnotations;

namespace AiSync.Application.DTOs;

public record CreateEmployeeDto(
    [Required, MaxLength(200)] string Name,
    DateTime DateOfBirth,
    bool IsActive = true);
