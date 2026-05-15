# AI-Sync Backend — Copilot Instructions

## Project
**AI-Sync Backend** is a REST API built in **.NET** following **Clean Architecture**.
Stack: ASP.NET Core · Entity Framework Core · SQL Server.

## How to use Skills
Skills define domain-specific conventions and rules.
Copilot applies them automatically based on the files you are editing (controlled by `applyTo` in each skill).

## Available Skills

| Skill | File | Applies to |
|---|---|---|
| Architecture | [skills/architecture.instructions.md](skills/architecture.instructions.md) | Entire backend |
| API Style | [skills/api-style.instructions.md](skills/api-style.instructions.md) | Controllers |
| Domain Rules | [skills/entity-rules.instructions.md](skills/entity-rules.instructions.md) | Entities, DTOs, Repositories |

## How to add a new skill
1. Create a file at `.github/skills/<name>.instructions.md`
2. Add a frontmatter with `applyTo` pointing to the corresponding file pattern
3. Register it in the table above

---
> For general technical documentation, see [`README.md`](../README.md)
