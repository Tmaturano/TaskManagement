# Task Management API

A minimal RESTful task management API built with ASP.NET Core (.NET 10), following **Clean Architecture** principles and backed by an **EF Core InMemory** database.

---

## Architecture

The solution is split into four projects that enforce a strict inward dependency rule:

```
TaskManagement.Api  ──►  TaskManagement.Application  ──►  TaskManagement.Domain
         │                                                        ▲
         └──────────►  TaskManagement.Infrastructure  ──────────┘
```

| Project | Type | Responsibility |
|---|---|---|
| `TaskManagement.Domain` | Class Library | `TaskItem` entity + `ITaskRepository` interface |
| `TaskManagement.Application` | Class Library | DTOs, `ITaskService` interface, `TaskService` implementation |
| `TaskManagement.Infrastructure` | Class Library | EF Core `AppDbContext` + `TaskRepository` implementation |
| `TaskManagement.Api` | Web API | `TasksController`, `Program.cs`, DI wiring |

The **Domain** and **Application** layers have **zero infrastructure dependencies** — they only reference .NET BCL and `Microsoft.Extensions.DependencyInjection.Abstractions`.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

---

## Setup & Run

```bash
# 1 — Clone the repository
git clone <repo-url>
cd TaskManagement

# 2 — Restore packages
dotnet restore

# 3 — Build
dotnet build

# 4 — Run the API
dotnet run --project src/TaskManagement.Api
```

The API starts on `https://localhost:7xxx` / `http://localhost:5xxx` (ports printed at startup).  
OpenAPI schema is available at `/openapi/v1.json` in the **Development** environment.

---

## Endpoints

| Method | Route | Description | Success |
|--------|-------|-------------|---------|
| `GET` | `/api/tasks` | Returns all tasks ordered by creation date (newest first) | `200 OK` |
| `POST` | `/api/tasks` | Creates a new task | `201 Created` |
| `PATCH` | `/api/tasks/{id}/toggle` | Toggles the `isCompleted` status | `200 OK` / `404 Not Found` |

### Sample requests

**Create a task**
```http
POST /api/tasks
Content-Type: application/json

{
  "title": "Write unit tests",
  "description": "Cover the service and repository layers"
}
```

**Toggle a task**
```http
PATCH /api/tasks/3fa85f64-5717-4562-b3fc-2c963f66afa6/toggle
```

---

## Design Notes

- **`TaskItem.Id`** is a server-generated `Guid` (`init`-only), preventing accidental mutation after creation.
- **`TaskItem.CreatedAt`** is set to `DateTime.UtcNow` at construction and is also `init`-only.
- **`ITaskRepository`** lives in the Domain so the Application layer can depend on the abstraction without knowing about EF Core.
- **`TaskService`** is the single orchestration point — it maps entities to `TaskResponse` records and delegates all persistence work to the repository.
- **`DependencyInjection`** extension classes in Application and Infrastructure keep `Program.cs` clean and let each layer own its own registrations.
- All EF Core package versions are managed centrally in `Directory.Packages.props` (Central Package Management).
- The InMemory database is scoped to the process lifetime; data resets on every restart (by design for this exercise).
