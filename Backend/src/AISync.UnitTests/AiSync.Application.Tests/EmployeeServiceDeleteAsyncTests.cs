using AiSync.Application.Services;
using AiSync.Domain.Interfaces;
using Moq;

namespace AiSync.Application.Tests;

public class EmployeeServiceDeleteAsyncTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly EmployeeService _sut;

    public EmployeeServiceDeleteAsyncTests()
    {
        _sut = new EmployeeService(_repositoryMock.Object);
    }

    // Feature: unit-tests, Property 5: DeleteAsync passes id through to repository unchanged
    [Theory]
    [MemberData(nameof(DeleteIdCases))]
    public async Task DeleteAsync_PassesIdToRepositoryUnchanged(int id)
    {
        _repositoryMock
            .Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await _sut.DeleteAsync(id);

        _repositoryMock.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    public static TheoryData<int> DeleteIdCases =>
        new()
        {
            1,       // small positive id
            42,      // typical mid-range id
            999999,  // large positive id
        };

    [Fact]
    public async Task DeleteAsync_ForwardsCancellationTokenToRepositoryDeleteAsync()
    {
        const int id = 7;
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _repositoryMock
            .Setup(r => r.DeleteAsync(id, token))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await _sut.DeleteAsync(id, token);

        _repositoryMock.Verify(r => r.DeleteAsync(id, token), Times.Once);
    }
}
