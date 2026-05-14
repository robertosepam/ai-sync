using AiSync.Domain.Entities;
using AiSync.Domain.Interfaces;

namespace AiSync.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    // In-memory storage for demo purposes
    private static readonly List<User> Users = new();
    private static int _nextUserId = 1;

    public UserRepository()
    {
        // Initialize with sample data if empty
        if (Users.Count == 0)
        {
            Users.Add(new User 
            { 
                UserId = _nextUserId++, 
                Name = "John Doe", 
                DateOfBirth = new DateTime(1990, 1, 15), 
                IsActive = true 
            });
            Users.Add(new User 
            { 
                UserId = _nextUserId++, 
                Name = "Jane Smith", 
                DateOfBirth = new DateTime(1985, 5, 20), 
                IsActive = true 
            });
        }
    }

    public Task<User?> GetUserByIdAsync(int userId)
    {
        var user = Users.FirstOrDefault(u => u.UserId == userId);
        return Task.FromResult(user);
    }

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var users = Users.AsEnumerable();
        return Task.FromResult(users);
    }

    public Task<User> CreateUserAsync(User user)
    {
        user.UserId = _nextUserId++;
        Users.Add(user);
        return Task.FromResult(user);
    }

    public Task<User?> UpdateUserAsync(int userId, User user)
    {
        var existingUser = Users.FirstOrDefault(u => u.UserId == userId);
        if (existingUser == null)
            return Task.FromResult<User?>(null);

        existingUser.Name = user.Name;
        existingUser.DateOfBirth = user.DateOfBirth;
        existingUser.IsActive = user.IsActive;

        return Task.FromResult<User?>(existingUser);
    }

    public Task<bool> DeleteUserAsync(int userId)
    {
        var user = Users.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
            return Task.FromResult(false);

        Users.Remove(user);
        return Task.FromResult(true);
    }

    public Task<bool> UserExistsAsync(int userId)
    {
        return Task.FromResult(Users.Any(u => u.UserId == userId));
    }
}
