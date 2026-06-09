using AiSync.Application.Services;
using AiSync.Domain.Entities;
using AiSync.Domain.Interfaces;
using Moq;

namespace AiSync.Application.Tests;

public class EmployeeServiceGetByIdAsyncTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly EmployeeService _sut;

    public EmployeeServiceGetByIdAsyncTests()
    {
        _sut = new EmployeeService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRepositoryReturnsNull_ReturnsNull()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        var result = await _sut.GetByIdAsync(42);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ForwardsIdAndCancellationTokenToRepository()
    {
        const int id = 7;
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, token))
            .ReturnsAsync((Employee?)null)
            .Verifiable();

        await _sut.GetByIdAsync(id, token);

        _repositoryMock.Verify(r => r.GetByIdAsync(id, token), Times.Once);
    }

    // Feature: unit-tests, Property 2: GetByIdAsync maps found Employee to EmployeeDto
    [Theory]
    [MemberData(nameof(EmployeeCases))]
    public async Task GetByIdAsync_MappingPreservesAllEmployeeFields(Employee employee)
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(employee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        var result = await _sut.GetByIdAsync(employee.Id);

        Assert.NotNull(result);
        Assert.Equal(employee.Id, result.Id);
        Assert.Equal(employee.Name, result.Name);
        Assert.Equal(employee.DateOfBirth, result.DateOfBirth);
        Assert.Equal(employee.IsActive, result.IsActive);
    }

    public static TheoryData<Employee> EmployeeCases
    {
        get
        {
            var data = new TheoryData<Employee>();

            // Case 1: active employee
            data.Add(EmployeeFactory.Create(1, "Alice", new DateTime(1990, 6, 15), true));

            // Case 2: inactive employee
            data.Add(EmployeeFactory.Create(2, "Bob", new DateTime(1985, 11, 3), false));

            // Case 3: employee with a different date and name
            data.Add(EmployeeFactory.Create(99, "Carlos", new DateTime(2000, 1, 1), true));

            return data;
        }
    }
}
