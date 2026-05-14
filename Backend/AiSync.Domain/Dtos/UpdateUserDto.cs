namespace AiSync.Domain.Dtos;

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public bool IsActive { get; set; }
}
