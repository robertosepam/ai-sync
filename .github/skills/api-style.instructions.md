---
applyTo: "Backend/src/AiSync.API/Controllers/**"
---

# API Style — Controllers

## General conventions
- All controllers inherit from `ControllerBase` (not `Controller`).
- Always decorate with `[ApiController]` and `[Route("api/[controller]")]`.
- Methods are `async` and receive `CancellationToken` as the last parameter.
- Return `IActionResult`, not concrete types.

## Expected HTTP responses per operation

| Operation | Method | Possible responses |
|---|---|---|
| List all | `GET /api/resource` | `200 OK` |
| Get by ID | `GET /api/resource/{id}` | `200 OK`, `404 Not Found` |
| Create | `POST /api/resource` | `201 Created`, `400 Bad Request` |
| Update | `PUT /api/resource/{id}` | `200 OK`, `404 Not Found`, `400 Bad Request` |
| Delete | `DELETE /api/resource/{id}` | `204 No Content`, `404 Not Found` |

## Structure example
```csharp
[HttpGet("{id:int}")]
[ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
{
    var result = await _service.GetByIdAsync(id, cancellationToken);
    return result is null ? NotFound() : Ok(result);
}
```
