# AI-Sync MCP Server

This is a **Model Context Protocol (MCP)** server written in **.NET 9** that exposes the AI-Sync Backend REST API as MCP tools consumable by AI agents (Copilot, Claude, etc.).

## Tools exposed

| Tool | Method | Endpoint | Description |
|---|---|---|---|
| `get_all_employees` | `GET` | `/api/employees` | Returns the full list of employees (Id, Name, DateOfBirth, IsActive). |
| `get_employee_by_id` | `GET` | `/api/employees/{id}` | Fetches a single employee by Id. Returns 404 message if not found. |
| `create_employee` | `POST` | `/api/employees` | Creates a new employee. Requires `name`, `dateOfBirth`; optional `isActive` (default: true). |
| `update_employee` | `PUT` | `/api/employees/{id}` | Fully replaces an employee's data. All fields required. |
| `delete_employee` | `DELETE` | `/api/employees/{id}` | Permanently deletes an employee record by Id. |

## Configuration

Set the backend base URL in `appsettings.json`:

```json
{
  "AiSyncApi": {
    "BaseUrl": "http://localhost:5023"
  }
}
```

## Running the server

```bash
cd Backend/MCP/AiSync.MCP
dotnet run
```

The MCP server communicates over **stdio** (standard input/output), which is the standard transport for MCP hosts like VS Code Copilot Chat.

## Registering in VS Code

Add the following to your `.vscode/mcp.json` (or user settings):

```json
{
  "servers": {
    "aisync": {
      "type": "stdio",
      "command": "dotnet",
      "args": ["run", "--project", "Backend/MCP/AiSync.MCP/AiSync.MCP.csproj"]
    }
  }
}
```
