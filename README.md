# Task Service - .NET 8+ Case Study

A REST API for managing tasks using .NET 8+, C#, Azure Cosmos DB and xUnit.

## Architecture

TaskService follows a layered architecture inspired by Clean Architecture principles, providing separation of concerns, testability, and maintainability. 

The architecture diagram is available in 'CaseStudy-Architecture-README.docx'.
    
 
📁 Project Structure

TaskService
│
├── Controllers
│   └── TasksController.cs
│
├── Application
│   ├── DTOs
│   │   ├── CreateTaskRequest.cs
│   │   ├── UpdateTaskRequest.cs
│   │   └── TaskResponse.cs
│   │
│   ├── Interfaces
│   │   └── ITaskRepository.cs
│   │
│   └── Services
│       ├── ITaskService.cs
│       └── TaskService.cs
│
├── Domain
│   ├── Entities
│   │   └── TaskItem.cs
│   │
│   └── Enums
│       └── TaskItemStatus.cs
│
├── Infrastructure
│   └── CosmosDb
│       └── CosmosTaskRepository.cs
│
├── Middleware
│   └── ExceptionHandlingMiddleware.cs
│
├── Exceptions
│   └── ...
│
├── Options
│   └── CosmosDbOptions.cs
│
└── Tests
    └── Unit
        ├── Services
        │   └── TaskServiceTests.cs
        │
        └── Controllers
            └── TasksControllerTests.cs

## Request flow

 
                     HTTP Request
                         │
                         ▼
              ┌─────────────────────┐
              │   TasksController   │
              │   REST / HTTP Layer │
              └──────────┬──────────┘
                         │
                         ▼
              ┌─────────────────────┐
              │     TaskService     │
              │ Business/Application│
              │      Logic          │
              └──────────┬──────────┘
                         │
                         ▼
              ┌─────────────────────┐
              │   ITaskRepository   │
              │    Abstraction      │
              └──────────┬──────────┘
                         │
                         ▼
              ┌─────────────────────┐
              │ CosmosTaskRepository│
              │   Infrastructure    │
              └──────────┬──────────┘
                         │
                         ▼
                  Azure Cosmos DB

Cross-cutting flow

The exception middleware sits around the request pipeline:

HTTP Request
     │
     ▼
ExceptionHandlingMiddleware
     │
     ▼
TasksController
     │
     ▼
TaskService
     │
     ▼
Repository
     │
     ▼
Cosmos DB
     │
     ▼
HTTP Response

 
### Layers

- **API/Controller**: HTTP and REST concerns.
- **Application**: DTOs, service contract and repository abstraction.
- **Domain**: Task entity and status enum.
- **Infrastructure**: Cosmos DB implementation.
- **Middleware**: centralized exception handling.
- **Tests**: service unit tests and controller tests.

The solution deliberately avoids CQRS, MediatR and other additional patterns because the exercise is timeboxed .

## Why this design

- Controllers contain HTTP concerns only.
- Application services contain application/business orchestration.
- Domain contains the core task model and status enum.
- Application depends on `ITaskRepository`, not on Cosmos SDK.
- Infrastructure implements the repository abstraction.
- DTOs keep the API contract separate from the persistence entity.
- Middleware centralizes exception-to-HTTP mapping.
- xUnit + Moq provide fast isolated unit tests.
- Cosmos DB Emulator integration tests are separated from unit tests.

## API

 
| Method | Endpoint | Description |
|---|---|---|
| POST | /api/tasks | Create a task |
| GET | /api/tasks/{id} | Get a task |
| GET | /api/tasks | List all tasks |
| PUT | /api/tasks/{id} | Update a task |
| DELETE | /api/tasks/{id} | Delete a task |



## Cosmos DB

Database: 'TaskDb'  
Container: 'Tasks'
Partition key: '/id'

The '/id' partition key is a simple case-study choice because the main point-read operation is by task ID. A production design would validate the partition key against query patterns, cardinality and scale.

Cosmos automatically adds system properties such as `_rid`, `_self`, `_etag`, `_attachments` and `_ts`. The application does not model those internal fields as normal business properties.

## Validation

- Title is required and limited to 200 characters.
- Description is limited to 2000 characters.
- Work values cannot be negative(must be >= 0).
- ASP.NET Core '[ApiController]' validation returns HTTP 400 for invalid requests.

## Error handling

- Missing task -> '404 Not Found'.
- Unexpected exception -> '500 Internal Server Error'.
- Request cancellation -> '499' in the middleware.
- Error responses contain a trace ID for troubleshooting.

## Unit testing strategy

### Unit tests - xUnit + Moq

The service is tested independently of Cosmos DB by mocking 'ITaskRepository'.

Covered scenarios include:
- Create and field mapping.
- Created/updated timestamps.
- Get existing task.
- Get missing task.
- Get all tasks.
- Empty collection.
- Update fields.
- Update missing task.
- Update timestamps.
- Delete existing task.
- Delete missing task.
- Validate status code.

Controller tests verify HTTP result contracts:
- POST -> 201 Created.
- GET -> 200 OK.
- GET all -> 200 OK.
- PUT -> 200 OK.
- DELETE -> 204 No Content.

 
## Assumptions

1. Work is represented as decimal because no unit is specified.
2. GUID is used for task ID.
3. Timestamps use UTC.
4. PUT is a full update.
5. No rule enforces 'OriginalEstimatedWork = RemainingWork + CompletedWork', because the case study permits remaining/completed work to be more or less than the original estimate.
6. '/id' is used as the Cosmos partition key for this timeboxed implementation.

## Deliberate trade-offs

Not included deliberately:

- Authentication/authorization – Secures APIs using authentication and role/permission-based authorization.
- API versioning – Supports versioned APIs to maintain backward compatibility and enable future enhancements.
- Pagination/filtering – Provides efficient retrieval of task collections with pagination and filtering capabilities.
- ETag optimistic concurrency – Uses Cosmos DB ETags to prevent lost updates during concurrent modifications.
- Retry/resilience policies – Handles transient failures using retry and resilience mechanisms.
- Full Cosmos integration suite – Includes integration tests covering Cosmos DB CRUD operations and persistence scenarios.
- Distributed caching – Improves performance and reduces database load through distributed caching.
- CI/CD – Supports automated build, test, and deployment pipelines for continuous integration and delivery.

## Run

1. Start Azure Cosmos DB Emulator.
2. Create database 'TaskDb'.
3. Create container 'Tasks' with partition key '/id'.
4. Check endpoint/key in 'src/TaskService.Api/appsettings.json'..
5. Run:

'''bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/TaskService.Api

Open '/swagger' using the HTTPS port printed by ASP.NET Core.

## Production enhancements

For production I would consider Managed Identity/Key Vault, authentication/authorization, pagination with Cosmos continuation tokens, resilience policies for transient failures, ETag optimistic concurrency, OpenTelemetry/Application Insights, health checks, rate limiting, API versioning, CI/CD quality gates and automated security/dependency scanning.
