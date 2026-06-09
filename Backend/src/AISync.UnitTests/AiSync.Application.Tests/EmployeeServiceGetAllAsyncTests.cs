using AiSync.Application.DTOs;
using AiSync.Application.Services;
using AiSync.Domain.Entities;
using AiSync.Domain.Interfaces;
using Moq;

namespace AiSync.Application.Tests;

public class EmployeeServiceGetAllAsyncTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly EmployeeService _sut;

    public EmployeeServiceGetAllAsyncTests()
    {
        _sut = new EmployeeService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsEmptyList_ReturnsEmptyEnumerable()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _sut.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PassesCancellationTokenToRepository()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _repositoryMock
            .Setup(r => r.GetAllAsync(token))
            .ReturnsAsync([])
            .Verifiable();

        await _sut.GetAllAsync(token);

        _repositoryMock.Verify(r => r.GetAllAsync(token), Times.Once);
    }

    // Feature: unit-tests, Property 1: GetAllAsync mapping preserves all Employee fields
    [Theory]
    [MemberData(nameof(EmployeeListCases))]
    public async Task GetAllAsync_MappingPreservesAllEmployeeFields(List<Employee> employees)
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        var result = (await _sut.GetAllAsync()).ToList();

        Assert.Equal(employees.Count, result.Count);
        for (int i = 0; i < employees.Count; i++)
        {
            Assert.Equal(employees[i].Id, result[i].Id);
            Assert.Equal(employees[i].Name, result[i].Name);
            Assert.Equal(employees[i].DateOfBirth, result[i].DateOfBirth);
            Assert.Equal(employees[i].IsActive, result[i].IsActive);
        }
    }

    public static TheoryData<List<Employee>> EmployeeListCases
    {
        get
        {
            var data = new TheoryData<List<Employee>>();

            // Case 1: single active employee
            data.Add([EmployeeFactory.Create(1, "Alice", new DateTime(1990, 6, 15), true)]);

            // Case 2: single inactive employee
            data.Add([EmployeeFactory.Create(2, "Bob", new DateTime(1985, 11, 3), false)]);

            // Case 3: multiple employees with mixed IsActive values
            data.Add(
            [
                EmployeeFactory.Create(10, "Carlos", new DateTime(1978, 1, 22), true),
                EmployeeFactory.Create(11, "Diana", new DateTime(1995, 7, 8), false),
                EmployeeFactory.Create(12, "Eve", new DateTime(2000, 12, 31), true),
            ]);

            return data;
        }
    }
}
