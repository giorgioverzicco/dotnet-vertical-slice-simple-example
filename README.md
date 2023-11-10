# .NET 7 - Vertical Slice Architecture - Simple Example

This is a simple example project demonstrating the usage of Vertical Slice Architecture in **.NET 7**, along with **MediatR**, **Carter**, **FluentValidation**, MediatR pipelines for validation and logging, middlewares for global exception handling and validation responses, the **CQRS** pattern, **Entity Framework** for commands, and **Dapper** for queries, and **SQLite** as database provider.

## Introduction

**Vertical Slice Architecture** is an architectural pattern that emphasizes organizing your code around features or slices of your application, rather than traditional layered architectures. It helps maintain a clean and focused structure for each feature while keeping your codebase more maintainable and scalable.

In this example, we will build a simple "run tracker" for managing activities and workouts, using the **CQRS** pattern for separation of concerns. **Entity Framework** will be used for handling commands (write operations), **Dapper** will be used for queries (read operations) and **SQLite** will be used to store our data locally.

## Technologies Used

-   .NET 7
-   MediatR: For implementing the Mediator pattern.
-   Carter: For mapping our Minimal APIs.
-   FluentValidation: For request validation.
-   Entity Framework Core: For handling command operations (Create, Update, Delete).
-   Dapper: For handling query operations (Read).

## Getting Started

1.  Clone this repository:
    `https://github.com/giorgioverzicco/dotnet-vertical-slice-simple-example.git` 
    
2.  Navigate to the project folder:
    `cd dotnet-vertical-slice-simple-example` 
    
3.  Build and run the application:
	`dotnet run` 
    
4.  Access the API using your preferred HTTP client (e.g., Postman, cURL, or a web browser) at `http://localhost:5000` or `https://localhost:5001`.

## Project Structure

The project structure follows the Vertical Slice Architecture, where each feature or slice of the application is organized in a self-contained folder or file.

## Features

### Create an Activity

-   Endpoint: `/activities`
-   Description: Create a new activity.
-   Request: `POST` with a JSON body containing the entity data.
-   Validation: Request validation is handled using FluentValidation.
-   Command Handling: Uses Entity Framework Core to create the entity.
-   Logging: Log request and response using Microsoft Logger.
-   Request Example:
```json
{
  "userId": 1,
  "activityType": "run",
  "distanceInMeters": 1000.0,
  "duration": "00:45:00",
  "date": "2023-11-09",
  "location": "Central Park",
  "notes": "Morning jog in the park"
}
```

### Get an Activity

-   Endpoint: `/activities/{activityId:int}`
-   Description: Get an activity by ID.
-   Request: `GET` with the entity ID as a route parameter.
-   Validation: Request validation is handled using FluentValidation.
-   Query Handling: Uses Dapper to fetch the entity by ID.
-   Logging: Log request and response using Microsoft Logger.

### Create a Workout

-   Endpoint: `/workouts`
-   Description: Create a new workout.
-   Request: `POST` with a JSON body containing the entity data.
-   Validation: Request validation is handled using FluentValidation.
-   Command Handling: Uses Entity Framework Core to create the entity.
-   Logging: Log request and response using Microsoft Logger.
-   Request Example:
```json
{
  "userId": 1,
  "activityIds": [1],
  "startTime": "2023-11-15T07:00:00Z",
  "endTime": "2023-11-15T08:00:00Z",
  "notes": "Morning marathon training"
}
```

### Get a Workout

-   Endpoint: `/workouts/{workoutId:int}`
-   Description: Get a workout by ID.
-   Request: `GET` with the entity ID as a route parameter.
-   Validation: Request validation is handled using FluentValidation.
-   Query Handling: Uses Dapper to fetch the entity by ID.
-   Logging: Log request and response using Microsoft Logger.

## Middleware

### Validation Middleware

-   Handles request validation using FluentValidation.
-   Returns appropriate validation responses for invalid requests.

### Exception Handling Middleware

-   Catches unhandled exceptions and returns a structured error response.

## Logging

-   Uses Microsoft Logger for structured logging.
-   Logs request and response information for each API endpoint.

## Configuration

-   Configuration settings are stored in `appsettings.json`.
-   Update the database connection string and other settings as needed.

## Conclusion

This example demonstrates a simple application following the **Vertical Slice Architecture**, *do not assume that the code written in this repository is in any shape or form production ready*.

You can use this as a starting point to build more complex applications while maintaining a clean and organized codebase.

Feel free to explore and expand upon this project to suit your specific requirements.
