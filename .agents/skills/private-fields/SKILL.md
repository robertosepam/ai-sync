---
name: private-fields
description: Enforces naming conventions for private fields without using underscore prefixes.
---

# Private Fields Convention

Private fields inside classes must:
- Use the `private` modifier
- Use `camelCase` naming convention
- NOT use `_` prefix
- Be declared at the top of the class
- Prefer `readonly` when the value should not change

## Good Examples

```csharp
private readonly ILogger<UserService> logger;
private readonly IUserRepository repository;
private string connectionString;
private int retryCount;