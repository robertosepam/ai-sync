using AiSync.Domain.Entities;

namespace AiSync.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(int userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> CreateUserAsync(User user);
    Task<User?> UpdateUserAsync(int userId, User user);
    Task<bool> DeleteUserAsync(int userId);
    Task<bool> UserExistsAsync(int userId);
}
