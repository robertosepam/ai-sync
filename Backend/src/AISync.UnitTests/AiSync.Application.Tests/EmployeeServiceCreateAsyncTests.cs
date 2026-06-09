using AiSync.Application.DTOs;
using AiSync.Application.Services;
using AiSync.Domain.Entities;
using AiSync.Domain.Interfaces;
using Moq;

namespace AiSync.Application.Tests;

public class EmployeeServiceCreateAsyncTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly EmployeeService _sut;

    public EmployeeServiceCreateAsyncTests()
    {
        _sut = new EmployeeService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenIsActiveNotSpecified_EntityPassedToAddAsyncHasIsActiveTrue()
    {
        // CreateEmployeeDto default for IsActive is true
        var dto = new CreateEmployeeDto("Diana", new DateTime(1992, 4, 10));

        var returnedEmployee = EmployeeFactory.Create(1, dto.Name, dto.DateOfBirth, true);

        Employee? capturedEntity = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Callback<Employee, CancellationToken>((e, _) => capturedEntity = e)
            .ReturnsAsync(returnedEmployee);

        await _sut.CreateAsync(dto);

        Assert.NotNull(capturedEntity);
        Assert.True(capturedEntity.IsActive);
    }

    [Fact]
    public async Task CreateAsync_ForwardsCancellationTokenToAddAsync()
    {
        var dto = new CreateEmployeeDto("Eve", new DateTime(1988, 7, 22), true);
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var returnedEmployee = EmployeeFactory.Create(2, dto.Name, dto.DateOfBirth, dto.IsActive);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Employee>(), token))
            .ReturnsAsync(returnedEmployee)
            .Verifiable();

        await _sut.CreateAsync(dto, token);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>(), token), Times.Once);
    }

    // Feature: unit-tests, Property 3: CreateAsync maps DTO to entity and result DTO correctly
    [Theory]
    [MemberData(nameof(CreateDtoCases))]
    public async Task CreateAsync_MapsEntityFieldsFromDtoAndReturnsCorrectlyMappedDto(
        CreateEmployeeDto dto,
        Employee repositoryReturnValue)
    {
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryReturnValue);

        Employee? capturedEntity = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Callback<Employee, CancellationToken>((e, _) => capturedEntity = e)
            .ReturnsAsync(repositoryReturnValue);

        var result = await _sut.CreateAsync(dto);

        // (a) AddAsync received an entity whose fields match the DTO
        Assert.NotNull(capturedEntity);
        Assert.Equal(dto.Name, capturedEntity.Name);
        Assert.Equal(dto.DateOfBirth, capturedEntity.DateOfBirth);
        Assert.Equal(dto.IsActive, capturedEntity.IsActive);

        // (b) Returned EmployeeDto fields match the entity returned by the repository
        Assert.Equal(repositoryReturnValue.Id, result.Id);
        Assert.Equal(repositoryReturnValue.Name, result.Name);
        Assert.Equal(repositoryReturnValue.DateOfBirth, result.DateOfBirth);
        Assert.Equal(repositoryReturnValue.IsActive, result.IsActive);
    }

    public static TheoryData<CreateEmployeeDto, Employee> CreateDtoCases
    {
        get
        {
            var data = new TheoryData<CreateEmployeeDto, Employee>();

            // Case 1: active employee with IsActive explicitly true
            var dto1 = new CreateEmployeeDto("Alice", new DateTime(1990, 6, 15), true);
            data.Add(dto1, EmployeeFactory.Create(1, dto1.Name, dto1.DateOfBirth, dto1.IsActive));

            // Case 2: inactive employee (IsActive explicitly false)
            var dto2 = new CreateEmployeeDto("Bob", new DateTime(1985, 11, 3), false);
            data.Add(dto2, EmployeeFactory.Create(2, dto2.Name, dto2.DateOfBirth, dto2.IsActive));

            // Case 3: employee using default IsActive (true) — not specified in constructor
            var dto3 = new CreateEmployeeDto("Carlos", new DateTime(2000, 1, 1));
            data.Add(dto3, EmployeeFactory.Create(3, dto3.Name, dto3.DateOfBirth, dto3.IsActive));

            return data;
        }
    }
}
