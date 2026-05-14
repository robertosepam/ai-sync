using AiSync.Application.Services;
using AiSync.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AiSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get a user by ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>The user if found</returns>
    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        return Ok(user);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="createUserDto">The user data</param>
    /// <returns>The created user</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto createUserDto)
    {
        if (string.IsNullOrWhiteSpace(createUserDto.Name) || createUserDto.Name.Length > 50)
            return BadRequest(new { message = "Name must be between 1 and 50 characters" });

        var createdUser = await _userService.CreateUserAsync(createUserDto);
        return CreatedAtAction(nameof(GetById), new { userId = createdUser.UserId }, createdUser);
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="updateUserDto">The updated user data</param>
    /// <returns>The updated user</returns>
    [HttpPut("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> Update(int userId, [FromBody] UpdateUserDto updateUserDto)
    {
        if (string.IsNullOrWhiteSpace(updateUserDto.Name) || updateUserDto.Name.Length > 50)
            return BadRequest(new { message = "Name must be between 1 and 50 characters" });

        var updatedUser = await _userService.UpdateUserAsync(userId, updateUserDto);
        if (updatedUser == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        return Ok(updatedUser);
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int userId)
    {
        var result = await _userService.DeleteUserAsync(userId);
        if (!result)
            return NotFound(new { message = $"User with ID {userId} not found" });

        return NoContent();
    }
}
