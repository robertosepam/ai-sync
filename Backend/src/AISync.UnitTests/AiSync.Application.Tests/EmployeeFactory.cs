using AiSync.Domain.Entities;

namespace AiSync.Application.Tests;

internal static class EmployeeFactory
{
    public static Employee Create(int id, string name, DateTime dateOfBirth, bool isActive) =>
        new()
        {
            Id = id,
            Name = name,
            DateOfBirth = dateOfBirth,
            IsActive = isActive,
        };
}
