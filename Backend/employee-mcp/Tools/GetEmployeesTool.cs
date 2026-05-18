using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;


/// <summary>
/// Tool to call Employees API and return the list of employees. This tool can be invoked by MCP clients to retrieve employee data from the API.
/// </summary>
internal class GetEmployeesTool
{
    [McpServerTool]
    [Description("Calls the Employees API and returns the list of employees.")]
    [RequiresDynamicCode("Calls System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync<T>(CancellationToken)")]
    public async Task<string> GetEmployees()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7132/api/")
        };

        var response = await client.GetAsync("employees");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}

