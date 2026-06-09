using AiSync.Application.DTOs;
using AiSync.Application.Services;
using AiSync.Domain.Entities;
using AiSync.Domain.Interfaces;
using Moq;

namespace AiSync.Application.Tests;

public class EmployeeServiceUpdateAsyncTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly EmployeeService _sut;

    public EmployeeServiceUpdateAsyncTests()
    {
        _sut = new EmployeeService(_repositoryMock.Object);
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryReturnsNullForGetById_ThrowsKeyNotFoundException()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        var dto = new UpdateEmployeeDto("Ghost", new DateTime(1990, 1, 1), true);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateAsync(99, dto));
    }

    [Fact]
    public async Task UpdateAsync_ForwardsCancellationTokenToBothGetByIdAsyncAndUpdateAsync()
    {
        const int id = 5;
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var existing = EmployeeFactory.Create(id, "Original", new DateTime(1980, 3, 12), true);
        var dto = new UpdateEmployeeDto("Updated", new DateTime(1980, 3, 12), false);
        var updatedEmployee = EmployeeFactory.Create(id, dto.Name, dto.DateOfBirth, dto.IsActive);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, token))
            .ReturnsAsync(existing)
            .Verifiable();

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Employee>(), token))
            .ReturnsAsync(updatedEmployee)
            .Verifiable();

        await _sut.UpdateAsync(id, dto, token);

        _repositoryMock.Verify(r => r.GetByIdAsync(id, token), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>(), token), Times.Once);
    }

    // Feature: unit-tests, Property 4: UpdateAsync mutates entity fields and returns correct DTO
    [Theory]
    [MemberData(nameof(UpdateCases))]
    public async Task UpdateAsync_MutatesEntityFieldsAndReturnsCorrectlyMappedDto(
        Employee existingEmployee,
        UpdateEmployeeDto dto)
    {
        Employee? capturedEntity = null;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(existingEmployee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEmployee);

        // Build the expected updated employee (mirrors what the service produces after mutation)
        var updatedEmployee = EmployeeFactory.Create(existingEmployee.Id, dto.Name, dto.DateOfBirth, dto.IsActive);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Callback<Employee, CancellationToken>((e, _) => capturedEntity = e)
            .ReturnsAsync(updatedEmployee);

        var result = await _sut.UpdateAsync(existingEmployee.Id, dto);

        // (a) UpdateAsync received the entity mutated to match the DTO
        Assert.NotNull(capturedEntity);
        Assert.Equal(dto.Name, capturedEntity.Name);
        Assert.Equal(dto.DateOfBirth, capturedEntity.DateOfBirth);
        Assert.Equal(dto.IsActive, capturedEntity.IsActive);

        // (b) Returned EmployeeDto fields match the updated entity returned by the repository
        Assert.Equal(updatedEmployee.Id, result.Id);
        Assert.Equal(updatedEmployee.Name, result.Name);
        Assert.Equal(updatedEmployee.DateOfBirth, result.DateOfBirth);
        Assert.Equal(updatedEmployee.IsActive, result.IsActive);
    }

    public static TheoryData<Employee, UpdateEmployeeDto> UpdateCases
    {
        get
        {
            var data = new TheoryData<Employee, UpdateEmployeeDto>();

            // Case 1: rename active employee and deactivate them (soft-delete via flag)
            data.Add(
                EmployeeFactory.Create(1, "Alice", new DateTime(1990, 6, 15), true),
                new UpdateEmployeeDto("Alice Smith", new DateTime(1990, 6, 15), false));

            // Case 2: change all fields of an inactive employee
            data.Add(
                EmployeeFactory.Create(2, "Bob", new DateTime(1985, 11, 3), false),
                new UpdateEmployeeDto("Robert", new DateTime(1985, 11, 3), true));

            // Case 3: update date of birth only, keep other fields the same
            data.Add(
                EmployeeFactory.Create(7, "Carlos", new DateTime(2000, 1, 1), true),
                new UpdateEmployeeDto("Carlos", new DateTime(1999, 12, 31), true));

            return data;
        }
    }
}
