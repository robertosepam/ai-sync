using AiSync.Domain.Dtos;
using AiSync.Domain.Entities;
using AiSync.Domain.Interfaces;

namespace AiSync.Application.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(int userId);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = new User
        {
            Name = createUserDto.Name,
            DateOfBirth = createUserDto.DateOfBirth,
            IsActive = createUserDto.IsActive
        };

        var createdUser = await _userRepository.CreateUserAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
    {
        var exists = await _userRepository.UserExistsAsync(userId);
        if (!exists)
            return null;

        var user = new User
        {
            Name = updateUserDto.Name,
            DateOfBirth = updateUserDto.DateOfBirth,
            IsActive = updateUserDto.IsActive
        };

        var updatedUser = await _userRepository.UpdateUserAsync(userId, user);
        return updatedUser == null ? null : MapToDto(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        return await _userRepository.DeleteUserAsync(userId);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            Name = user.Name,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive
        };
    }
}
