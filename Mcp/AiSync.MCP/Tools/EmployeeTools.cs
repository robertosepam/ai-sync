using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;

namespace AiSync.MCP.Tools;

// ---------------------------------------------------------------------------
// DTOs (mirrors AiSync.Application.DTOs — no project reference needed)
// ---------------------------------------------------------------------------

/// <summary>Represents a persisted Employee returned by the API.</summary>
public record EmployeeDto(int Id, string Name, DateTime DateOfBirth, bool IsActive);

/// <summary>Payload required to create a new Employee.</summary>
public record CreateEmployeeDto(string Name, DateTime DateOfBirth, bool IsActive = true);

/// <summary>Payload required to fully update an existing Employee.</summary>
public record UpdateEmployeeDto(string Name, DateTime DateOfBirth, bool IsActive);

// ---------------------------------------------------------------------------
// MCP Tool class
// ---------------------------------------------------------------------------

[McpServerToolType]
public sealed class EmployeeTools
{
    private readonly IHttpClientFactory _httpFactory;

    public EmployeeTools(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    // -----------------------------------------------------------------------
    // GET /api/employees
    // -----------------------------------------------------------------------

    [McpServerTool(Name = "get_all_employees")]
    [Description(
        "Returns the complete list of employees registered in the AI-Sync system. " +
        "Each employee includes: Id (int), Name (string), DateOfBirth (ISO-8601 datetime), " +
        "and IsActive (bool). Use this tool when you need an overview of the workforce " +
        "or when you want to search/filter employees on the client side.")]
    public async Task<string> GetAllEmployees()
    {
        try
        {
            var client = _httpFactory.CreateClient("AiSyncApi");
            var response = await client.GetAsync("api/employees");
            response.EnsureSuccessStatusCode();
            var employees = await response.Content.ReadFromJsonAsync<IEnumerable<EmployeeDto>>();
            return JsonSerializer.Serialize(employees, JsonOptions);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.GetType().Name} — {ex.Message}";
        }
    }

    // -----------------------------------------------------------------------
    // GET /api/employees/{id}
    // -----------------------------------------------------------------------

    [McpServerTool(Name = "get_employee_by_id")]
    [Description(
        "Retrieves a single employee by their numeric Id. " +
        "Returns employee data (Id, Name, DateOfBirth, IsActive) if found, " +
        "or a 404-not-found message if no employee with that Id exists. " +
        "Use this tool when you already know the employee's Id and want full details.")]
    public async Task<string> GetEmployeeById(
        [Description("The unique integer identifier of the employee to retrieve.")] int id)
    {
        var client = _httpFactory.CreateClient("AiSyncApi");
        var response = await client.GetAsync($"api/employees/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return $"Employee with id {id} was not found.";

        response.EnsureSuccessStatusCode();
        var employee = await response.Content.ReadFromJsonAsync<EmployeeDto>();
        return JsonSerializer.Serialize(employee, JsonOptions);
    }

    // -----------------------------------------------------------------------
    // POST /api/employees
    // -----------------------------------------------------------------------

    [McpServerTool(Name = "create_employee")]
    [Description(
        "Creates a new employee record in the AI-Sync system. " +
        "Required fields: Name (string, max 200 chars), DateOfBirth (ISO-8601 e.g. '1990-05-20'). " +
        "Optional field: IsActive (bool, defaults to true). " +
        "Returns the newly created employee including the system-assigned Id. " +
        "Use this tool when onboarding a new team member.")]
    public async Task<string> CreateEmployee(
        [Description("Full name of the employee (required, max 200 characters).")] string name,
        [Description("Date of birth in ISO-8601 format (e.g. '1990-05-20T00:00:00').")] DateTime dateOfBirth,
        [Description("Whether the employee is currently active. Defaults to true.")] bool isActive = true)
    {
        var dto = new CreateEmployeeDto(name, dateOfBirth, isActive);
        var client = _httpFactory.CreateClient("AiSyncApi");

        var response = await client.PostAsJsonAsync("api/employees", dto);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadAsStringAsync();
            return $"Validation error: {error}";
        }

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<EmployeeDto>();
        return JsonSerializer.Serialize(created, JsonOptions);
    }

    // -----------------------------------------------------------------------
    // PUT /api/employees/{id}
    // -----------------------------------------------------------------------

    [McpServerTool(Name = "update_employee")]
    [Description(
        "Fully updates an existing employee identified by their Id. " +
        "All fields must be supplied: Name (string, max 200 chars), " +
        "DateOfBirth (ISO-8601), IsActive (bool). " +
        "Returns the updated employee data, or a not-found message if the Id is invalid. " +
        "Use this tool to correct employee information or change their active status.")]
    public async Task<string> UpdateEmployee(
        [Description("The unique integer identifier of the employee to update.")] int id,
        [Description("New full name for the employee (required, max 200 characters).")] string name,
        [Description("New date of birth in ISO-8601 format (e.g. '1990-05-20T00:00:00').")] DateTime dateOfBirth,
        [Description("Whether the employee should be marked as active (true) or inactive (false).")] bool isActive)
    {
        var dto = new UpdateEmployeeDto(name, dateOfBirth, isActive);
        var client = _httpFactory.CreateClient("AiSyncApi");

        var response = await client.PutAsJsonAsync($"api/employees/{id}", dto);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return $"Employee with id {id} was not found.";

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadAsStringAsync();
            return $"Validation error: {error}";
        }

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<EmployeeDto>();
        return JsonSerializer.Serialize(updated, JsonOptions);
    }

    // -----------------------------------------------------------------------
    // DELETE /api/employees/{id}
    // -----------------------------------------------------------------------

    [McpServerTool(Name = "delete_employee")]
    [Description(
        "Permanently deletes the employee record with the given Id from the AI-Sync system. " +
        "Returns a success confirmation message on deletion (HTTP 204), " +
        "or a not-found message if no employee with that Id exists. " +
        "Use this tool only when an employee record must be permanently removed " +
        "(e.g., data-correction scenarios). Prefer setting IsActive=false for offboarding.")]
    public async Task<string> DeleteEmployee(
        [Description("The unique integer identifier of the employee to delete.")] int id)
    {
        var client = _httpFactory.CreateClient("AiSyncApi");
        var response = await client.DeleteAsync($"api/employees/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return $"Employee with id {id} was not found.";

        response.EnsureSuccessStatusCode();
        return $"Employee with id {id} was successfully deleted.";
    }

    // -----------------------------------------------------------------------
    // Shared serializer options
    // -----------------------------------------------------------------------

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
