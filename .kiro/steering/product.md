# AI-Sync — Product Overview

AI-Sync is a backend REST API for managing employee data. It exposes CRUD operations over HTTP and serves as the data layer for downstream consumers (e.g. a frontend or integration clients).

## Current domain
- **Employee** — the only entity at this stage. Supports create, read, update, and soft-delete via `IsActive`.

## Goals
- Provide a clean, versioned REST API following standard HTTP semantics.
- Keep the backend maintainable and extensible as new domain entities are added.
