using AiSync.Domain.Entities;

namespace AiSync.Domain.Tests;

public class EmployeeEntityTests
{
    [Fact]
    public void Id_CanBeSetAndReadBack()
    {
        var employee = new Employee { Id = 42 };

        Assert.Equal(42, employee.Id);
    }

    [Fact]
    public void Name_CanBeSetAndReadBack()
    {
        var employee = new Employee { Name = "Jane Doe" };

        Assert.Equal("Jane Doe", employee.Name);
    }

    [Fact]
    public void DateOfBirth_CanBeSetAndReadBack()
    {
        var dob = new DateTime(1990, 6, 15);
        var employee = new Employee { DateOfBirth = dob };

        Assert.Equal(dob, employee.DateOfBirth);
    }

    [Fact]
    public void IsActive_CanBeSetAndReadBack()
    {
        var employee = new Employee { IsActive = true };

        Assert.True(employee.IsActive);
    }

    [Fact]
    public void AllProperties_CanBeSetAndReadBack()
    {
        var dob = new DateTime(1985, 3, 22);
        var employee = new Employee
        {
            Id = 7,
            Name = "John Smith",
            DateOfBirth = dob,
            IsActive = true
        };

        Assert.Equal(7, employee.Id);
        Assert.Equal("John Smith", employee.Name);
        Assert.Equal(dob, employee.DateOfBirth);
        Assert.True(employee.IsActive);
    }

    [Fact]
    public void Name_DefaultValue_IsEmptyString()
    {
        var employee = new Employee();

        Assert.Equal(string.Empty, employee.Name);
    }

    [Fact]
    public void IsActive_CanBeSetToTrue()
    {
        var employee = new Employee { IsActive = true };

        Assert.True(employee.IsActive);
    }

    [Fact]
    public void IsActive_CanBeSetToFalse()
    {
        var employee = new Employee { IsActive = false };

        Assert.False(employee.IsActive);
    }
}
