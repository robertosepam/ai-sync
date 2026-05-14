using AiSync.Application.Services;
using AiSync.Domain.Dtos;
using AiSync.Domain.Entities;
using AiSync.Domain.Interfaces;
using Moq;

namespace AiSync.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ReturnsUserDto()
    {
        // Arrange
        var userId = 1;
        var user = new User
        {
            UserId = userId,
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 1, 15),
            IsActive = true
        };

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("John Doe", result.Name);
        Assert.True(result.IsActive);
        _mockUserRepository.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_ReturnsCreatedUserDto()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Name = "Jane Smith",
            DateOfBirth = new DateTime(1985, 5, 20),
            IsActive = true
        };

        var createdUser = new User
        {
            UserId = 1,
            Name = createUserDto.Name,
            DateOfBirth = createUserDto.DateOfBirth,
            IsActive = createUserDto.IsActive
        };

        _mockUserRepository.Setup(r => r.CreateUserAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _userService.CreateUserAsync(createUserDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal("Jane Smith", result.Name);
        _mockUserRepository.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WithValidData_ReturnsUpdatedUserDto()
    {
        // Arrange
        var userId = 1;
        var updateUserDto = new UpdateUserDto
        {
            Name = "Updated Name",
            DateOfBirth = new DateTime(1990, 6, 10),
            IsActive = false
        };

        var updatedUser = new User
        {
            UserId = userId,
            Name = updateUserDto.Name,
            DateOfBirth = updateUserDto.DateOfBirth,
            IsActive = updateUserDto.IsActive
        };

        _mockUserRepository.Setup(r => r.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.UpdateUserAsync(userId, It.IsAny<User>()))
            .ReturnsAsync(updatedUser);

        // Act
        var result = await _userService.UpdateUserAsync(userId, updateUserDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.False(result.IsActive);
        _mockUserRepository.Verify(r => r.UserExistsAsync(userId), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateUserAsync(userId, It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        _mockUserRepository.Setup(r => r.DeleteUserAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(r => r.DeleteUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(r => r.DeleteUserAsync(userId))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        Assert.False(result);
        _mockUserRepository.Verify(r => r.DeleteUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsListOfUserDtos()
    {
        // Arrange
        var users = new List<User>
        {
            new User { UserId = 1, Name = "User 1", DateOfBirth = new DateTime(1990, 1, 1), IsActive = true },
            new User { UserId = 2, Name = "User 2", DateOfBirth = new DateTime(1991, 2, 2), IsActive = true }
        };

        _mockUserRepository.Setup(r => r.GetAllUsersAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(r => r.GetAllUsersAsync(), Times.Once);
    }
}
