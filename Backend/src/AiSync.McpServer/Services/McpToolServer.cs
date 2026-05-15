using AiSync.Application.DTOs;
using AiSync.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AiSync.McpServer.Services;

/// <summary>
/// MCP Tool Server that exposes employee management operations as MCP tools.
/// Communicates via stdio using JSON-RPC protocol.
/// </summary>
public sealed class McpToolServer : BackgroundService
{
    private readonly IEmployeeService _service;
    private readonly ILogger<McpToolServer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public McpToolServer(IEmployeeService service, ILogger<McpToolServer> logger)
    {
        _service = service;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MCP Tool Server starting");

        using var reader = new StreamReader(Console.OpenStandardInput());
        using var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };

        try
        {
            // Initialize MCP server
            var initRequest = await reader.ReadLineAsync(stoppingToken);
            if (initRequest != null)
            {
                var initMessage = JsonSerializer.Deserialize<JsonElement>(initRequest);
                var method = initMessage.GetProperty("method").GetString();

                if (method == "initialize")
                {
                    var response = new
                    {
                        jsonrpc = "2.0",
                        id = initMessage.GetProperty("id").GetInt32(),
                        result = new
                        {
                            protocolVersion = "2024-11-05",
                            capabilities = new
                            {
                                tools = new { }
                            },
                            serverInfo = new
                            {
                                name = "AI-Sync MCP Server",
                                version = "1.0.0"
                            }
                        }
                    };

                    await writer.WriteLineAsync(JsonSerializer.Serialize(response, _jsonOptions));
                }
            }

            // Process tool calls
            string? line;
            while ((line = await reader.ReadLineAsync(stoppingToken)) != null)
            {
                try
                {
                    var message = JsonSerializer.Deserialize<JsonElement>(line);
                    var method = message.GetProperty("method").GetString();
                    var requestId = message.GetProperty("id").GetInt32();
                    var @params = message.GetProperty("params");

                    var response = await HandleToolCall(method, @params, requestId, stoppingToken);
                    await writer.WriteLineAsync(JsonSerializer.Serialize(response, _jsonOptions));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("MCP Tool Server shutting down");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MCP Tool Server error");
        }
    }

    private async Task<object> HandleToolCall(string? method, JsonElement @params, int requestId, CancellationToken cancellationToken)
    {
        try
        {
            return method switch
            {
                "tools/list" => new
                {
                    jsonrpc = "2.0",
                    id = requestId,
                    result = new
                    {
                        tools = new[]
                        {
                            new { name = "get_all_employees", description = "Get all employees" },
                            new { name = "get_employee_by_id", description = "Get an employee by ID" },
                            new { name = "create_employee", description = "Create a new employee" },
                            new { name = "update_employee", description = "Update an existing employee" },
                            new { name = "delete_employee", description = "Delete an employee by ID" }
                        }
                    }
                },

                "tools/call" => await HandleToolExecution(@params, requestId, cancellationToken),

                _ => new
                {
                    jsonrpc = "2.0",
                    id = requestId,
                    error = new { code = -32601, message = "Method not found" }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling tool call: {Method}", method);
            return new
            {
                jsonrpc = "2.0",
                id = requestId,
                error = new { code = -32603, message = ex.Message }
            };
        }
    }

    private async Task<object> HandleToolExecution(JsonElement @params, int requestId, CancellationToken cancellationToken)
    {
        var toolName = @params.GetProperty("name").GetString();
        var toolParams = @params.GetProperty("arguments");

        try
        {
            object result = toolName switch
            {
                "get_all_employees" => (object)await _service.GetAllAsync(cancellationToken),

                "get_employee_by_id" => await _service.GetByIdAsync(
                    toolParams.GetProperty("id").GetInt32(), cancellationToken),

                "create_employee" => await _service.CreateAsync(
                    new CreateEmployeeDto(
                        toolParams.GetProperty("name").GetString() ?? string.Empty,
                        toolParams.GetProperty("dateOfBirth").GetDateTime(),
                        toolParams.GetProperty("isActive").GetBoolean()
                    ), cancellationToken),

                "update_employee" => await _service.UpdateAsync(
                    toolParams.GetProperty("id").GetInt32(),
                    new UpdateEmployeeDto(
                        toolParams.GetProperty("name").GetString() ?? string.Empty,
                        toolParams.GetProperty("dateOfBirth").GetDateTime(),
                        toolParams.GetProperty("isActive").GetBoolean()
                    ), cancellationToken),

                "delete_employee" => new { success = await DeleteEmployeeAsync(
                    toolParams.GetProperty("id").GetInt32(), cancellationToken) },

                _ => throw new InvalidOperationException($"Unknown tool: {toolName}")
            };

            return new
            {
                jsonrpc = "2.0",
                id = requestId,
                result = new { content = new[] { new { type = "text", text = JsonSerializer.Serialize(result, _jsonOptions) } } }
            };
        }
        catch (Exception ex)
        {
            return new
            {
                jsonrpc = "2.0",
                id = requestId,
                error = new { code = -32603, message = ex.Message }
            };
        }
    }

    private async Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _service.DeleteAsync(id, cancellationToken);
            return true;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }
}
