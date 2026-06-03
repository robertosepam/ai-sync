---
applyTo: "Backend/**"
---

# Project Constitution — Quality & Performance Principles

## Code Quality

1. **Single Responsibility** — Every class and method must have one clearly defined purpose.
2. **Immutability by Default** — Prefer `record` types for DTOs and read-only properties for entities. Use mutable state only when explicitly required.
3. **No Swallowed Exceptions** — Every `catch` block must log, rethrow, or return a meaningful result. Empty catch blocks are prohibited.
4. **Explicit Over Implicit** — Avoid magic strings, hidden conventions, or implicit type conversions. Use strongly-typed constants, enums, and named parameters.
5. **Consistent Naming** — PascalCase for public members and types, camelCase for locals and parameters, `_camelCase` for private fields.
6. **Blank Line Before Return** — Every `return` statement must be preceded by an empty line to improve readability.

## Testing Standards

7. **Minimum Coverage** — All Application-layer services must have unit tests covering happy path, error path, and edge cases.
8. **Test Naming** — Use the pattern `MethodName_Scenario_ExpectedResult` (e.g., `GetByIdAsync_NonExistentId_ReturnsNull`).
9. **Arrange-Act-Assert** — Every test must follow the AAA pattern with clear visual separation.
10. **No Test Interdependence** — Tests must not depend on execution order or shared mutable state.
11. **Mock at Boundaries** — Mock only external dependencies (repositories, HTTP clients). Do not mock the class under test.

## User Experience Consistency

12. **Uniform Error Responses** — All error responses must use the RFC 7807 `ProblemDetails` format with `type`, `title`, `status`, and `detail`.
13. **Consistent Pagination** — Collection endpoints must support `pageNumber` and `pageSize` query parameters with sensible defaults (page 1, size 20).
14. **Predictable Status Codes** — `200` for success, `201` for creation, `204` for deletion, `400` for validation errors, `404` for not found, `500` for unexpected failures.
15. **Input Validation at the Edge** — All DTOs must be validated in the API layer before reaching the Application layer. Return `400` with field-level errors.

## Performance Requirements

16. **Async All the Way** — No blocking calls (`Task.Result`, `.Wait()`, `.GetAwaiter().GetResult()`) in async paths.
17. **CancellationToken Propagation** — Every async method must accept and forward a `CancellationToken`.
18. **No N+1 Queries** — Repository methods returning collections must eagerly load required relations or use projections.
19. **AsNoTracking for Reads** — Read-only queries must use `AsNoTracking()` to reduce EF Core overhead.
20. **Response Time Target** — API endpoints must respond within 200 ms (p95) under normal load. Any endpoint exceeding this must be profiled and optimized.
