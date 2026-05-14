# AI-Sync User API - Clean Architecture Implementation

A simple ASP.NET Core 8 REST API built with Clean Architecture principles for managing users with full CRUD operations.

## Project Structure

```
AiSync.Domain/                 # Core business entities and interfaces
├── Entities/
│   └── User.cs               # User entity with UserId, Name, DateOfBirth, IsActive
├── Dtos/
│   ├── UserDto.cs            # Read DTO
│   ├── CreateUserDto.cs      # Create DTO
│   └── UpdateUserDto.cs      # Update DTO
└── Interfaces/
    └── IUserRepository.cs    # Repository contract

AiSync.Application/            # Business logic and services
└── Services/
    └── UserService.cs        # User service with CRUD operations

AiSync.Infrastructure/         # Data access and external services
└── Repositories/
    └── UserRepository.cs     # In-memory user repository implementation

AiSync.API/                    # REST API layer
├── Controllers/
│   └── UsersController.cs    # User endpoints
└── Program.cs               # Dependency injection configuration

AiSync.Tests/                  # Unit tests
└── UserServiceTests.cs       # User service tests with xUnit & Moq
```

## API Endpoints

All endpoints are prefixed with `/api/users`

### GET /api/users
**Description:** Retrieve all users  
**Response:** 200 OK with list of UserDto

### GET /api/users/{userId}
**Description:** Retrieve a specific user by ID  
**Parameters:** `userId` (int) - The user ID  
**Response:** 
- 200 OK with UserDto
- 404 Not Found if user doesn't exist

### POST /api/users
**Description:** Create a new user  
**Request Body:** CreateUserDto
```json
{
  "name": "John Doe",
  "dateOfBirth": "1990-01-15T00:00:00",
  "isActive": true
}
```
**Response:** 
- 201 Created with created UserDto and Location header
- 400 Bad Request if validation fails

### PUT /api/users/{userId}
**Description:** Update an existing user  
**Parameters:** `userId` (int) - The user ID  
**Request Body:** UpdateUserDto
```json
{
  "name": "Jane Doe",
  "dateOfBirth": "1985-05-20T00:00:00",
  "isActive": true
}
```
**Response:**
- 200 OK with updated UserDto
- 404 Not Found if user doesn't exist
- 400 Bad Request if validation fails

### DELETE /api/users/{userId}
**Description:** Delete a user  
**Parameters:** `userId` (int) - The user ID  
**Response:**
- 204 No Content on success
- 404 Not Found if user doesn't exist

## User Entity

| Column | Type | Constraints |
|--------|------|-------------|
| UserId | int | Primary Key, Auto-increment |
| Name | string(50) | Required, Max 50 characters |
| DateOfBirth | datetime | Required |
| IsActive | bool | Required |

## Running the Application

### Build Solution
```bash
dotnet build AI-Sync-Backend.sln
```

### Run Tests
```bash
dotnet test AiSync.Tests/AiSync.Tests.csproj
```

### Start API Server
```bash
dotnet run --project AiSync.API/AiSync.API.csproj
```

The API will be available at `https://localhost:5001` and Swagger UI at `/swagger/index.html`

## Unit Tests

8 comprehensive unit tests covering:
- Get user by ID (valid and invalid cases)
- Get all users
- Create user
- Update user
- Delete user (success and failure cases)

Tests use xUnit and Moq for mocking the repository layer.

Run tests with:
```bash
dotnet test
```

## Clean Architecture Layers

1. **Domain Layer** - Contains core business logic, entities, and interfaces
2. **Application Layer** - Contains business rules and orchestration logic
3. **Infrastructure Layer** - Contains implementation of repositories and external services
4. **API Layer** - REST API endpoints and dependency injection configuration

## Dependencies

- .NET 8.0
- xUnit 2.5.3 (Testing)
- Moq 4.20.70 (Mocking)
- Swagger/OpenAPI (API documentation)
