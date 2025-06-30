# HRKošarka - Basketball Management System

HRKošarka is a web-based basketball management system developed as part of a master thesis project. The application uses modern **.NET 8** best practices, focusing on maintainability, scalability, and clear separation of concerns.

## Key Technologies & Architecture

- **Clean Architecture**  
  Solution is organized into layers: Domain, Application, Infrastructure, Persistence, API, and UI.

- **CQRS (Command Query Responsibility Segregation)**  
  Commands for writes, queries for reads.

- **MediatR**  
  Decoupled request/response and notification patterns.

- **FluentValidation**  
  Robust and maintainable input validation.

- **Entity Framework Core**  
  Database access and repository pattern.

- **NSwag & OpenAPI**  
  Strongly-typed API clients and DTOs for the frontend.

- **Blazor Server:**  
  Modern server-side UI with [MudBlazor](https://mudblazor.com/) components.



