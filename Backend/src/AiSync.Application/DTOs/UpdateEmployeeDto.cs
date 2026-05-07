using System.ComponentModel.DataAnnotations;

namespace AiSync.Application.DTOs;

public record UpdateEmployeeDto(
    [Required, MaxLength(200)] string Name,
    DateTime DateOfBirth,
    bool IsActive);
