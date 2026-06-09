using AiSync.API.Controllers;
using AiSync.Application.DTOs;
using AiSync.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AiSync.API.Tests;

// Feature: unit-tests, Property 12: GetById controller wraps found EmployeeDto in OkObjectResult
public class EmployeesControllerGetByIdTests
{
    private readonly Mock<IEmployeeService> _mockService;
    private readonly EmployeesController _controller;

    public EmployeesControllerGetByIdTests()
    {
        _mockService = new Mock<IEmployeeService>();
        _controller = new EmployeesController(_mockService.Object);
    }

    [Fact]
    public async Task GetById_WhenServiceReturnsNull_ReturnsNotFoundResult()
    {
        // Arrange
        _mockService
            .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeDto?)null);

        // Act
        var result = await _controller.GetById(1, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetById_ForwardsIdAndCancellationTokenToService()
    {
        // Arrange
        var expectedId = 42;
        using var cts = new CancellationTokenSource();
        var expectedToken = cts.Token;

        var dto = new EmployeeDto(expectedId, "Alice", new DateTime(1990, 1, 1), true);
        _mockService
            .Setup(s => s.GetByIdAsync(expectedId, expectedToken))
            .ReturnsAsync(dto);

        // Act
        await _controller.GetById(expectedId, expectedToken);

        // Assert
        _mockService.Verify(s => s.GetByIdAsync(expectedId, expectedToken), Times.Once);
    }

    public static IEnumerable<object[]> EmployeeDtoCases =>
    [
        [new EmployeeDto(1, "Alice", new DateTime(1990, 5, 15), true)],
        [new EmployeeDto(99, "Bob", new DateTime(1985, 11, 3), false)],
        [new EmployeeDto(7, "Carol", new DateTime(2000, 7, 22), true)],
    ];

    // Feature: unit-tests, Property 12: GetById controller wraps found EmployeeDto in OkObjectResult
    [Theory]
    [MemberData(nameof(EmployeeDtoCases))]
    public async Task GetById_WhenServiceReturnsEmployeeDto_ReturnsOkObjectResultWithThatDto(EmployeeDto dto)
    {
        // Arrange
        _mockService
            .Setup(s => s.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _controller.GetById(dto.Id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(dto, okResult.Value);
    }
}
