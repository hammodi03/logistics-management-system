# Logistics Management System

A backend-focused logistics management system built with **ASP.NET Core 10, C#, PostgreSQL, Entity Framework Core, and Docker**.

The project demonstrates relational database design, SQL querying, REST API development, business logic validation, automated testing, and containerization.

## Features

- Manage packages with CRUD operations.
- Assign packages to delivery drivers.
- Track deliveries through `Assigned`, `In transit`, and `Delivered` statuses.
- Validate delivery status transitions.
- Automatically update package status when delivery status changes.
- Record delivery events and retrieve delivery history.
- Explore and test API endpoints using Swagger UI.
- Run the API and PostgreSQL together using Docker Compose.

## Tech Stack

| Technology | Purpose |
|---|---|
| C# / ASP.NET Core 10 | REST API and business logic |
| PostgreSQL | Relational database |
| Entity Framework Core | Database access |
| SQL | Queries, joins, aggregations, CTEs and window functions |
| xUnit | Automated testing |
| Swagger / OpenAPI | API documentation |
| Docker / Docker Compose | Containerized application environment |

## Project Structure

```text
logistics-management-system/
├── database/
│   ├── 01_schema.sql
│   ├── 02_seed_data.sql
│   ├── 03_queries.sql
│   └── 04_performance.sql
├── backend/
│   ├── Logistics.Api/
│   │   ├── Controllers/
│   │   ├── Data/
│   │   ├── DTOs/
│   │   ├── Models/
│   │   ├── Services/
│   │   └── Dockerfile
│   ├── Logistics.Api.Tests/
│   └── compose.yaml
├── .gitignore
└── README.md
```

## Database

The PostgreSQL database contains five related tables:

- `customers` — customer information.
- `drivers` — delivery driver information.
- `packages` — customer packages, destinations and statuses.
- `deliveries` — delivery assignments and statuses.
- `delivery_events` — delivery status history.

The SQL files include schema creation, sample data, analytical queries and index performance examples.

The `database/` directory contains four SQL scripts:

- `01_schema.sql` – Creates the relational database schema, including tables, primary keys, foreign keys, and constraints.
- `02_seed_data.sql` – Inserts sample customers, drivers, packages, deliveries, and delivery events.
- `03_queries.sql` – Demonstrates analytical SQL queries using JOINs, aggregations, CTEs, and window functions.
- `04_performance.sql` – Demonstrates indexing and query performance analysis with EXPLAIN ANALYZE.

The first two scripts are executed automatically when the Docker PostgreSQL database is initialized. The analytical and performance scripts can be executed manually for learning, exploration, and demonstration purposes.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/packages` | Retrieve all packages |
| GET | `/api/packages/{id}` | Retrieve a package by ID |
| POST | `/api/packages` | Create a package |
| PUT | `/api/packages/{id}` | Update package destination |
| DELETE | `/api/packages/{id}` | Delete a package |
| POST | `/api/deliveries` | Assign a package to a driver |
| PUT | `/api/deliveries/{id}/status` | Update delivery status |
| GET | `/api/deliveries/{id}/events` | Retrieve delivery history |

## Getting Started with Docker

**Requirements:** Docker Desktop with Docker Compose.

1. Clone the repository.
2. Navigate to the `backend` directory.
3. Create a `.env` file in `backend` containing:

   ```env
   POSTGRES_PASSWORD=choose-your-own-password
   ```

4. Start the application:

   ```bash
   docker compose up -d --build
   ```

5. Open Swagger UI:

   http://localhost:5110/swagger

The PostgreSQL database is initialized automatically using `01_schema.sql` and `02_seed_data.sql` when the database volume is created for the first time.

To stop the containers:

```bash
docker compose down
```

The PostgreSQL data is stored in a Docker volume and is preserved when the containers are stopped normally.

## Running Automated Tests

The project includes unit tests and database-backed integration tests.

The integration tests use a separate PostgreSQL database named `logistics_integration_test_db`.

To run the tests, create and configure the test database, set the required connection strings using .NET User Secrets, and run:

```bash
cd backend
dotnet test Logistics.Api.Tests/Logistics.Api.Tests.csproj
```

**Note:** The test database is separate from the Docker database. Running the tests requires the corresponding local test database and configuration.

## Future Improvements

- React and TypeScript frontend.
- Improved deployment configuration.
- Additional API documentation and error handling.

## Project Purpose

This project was developed to practice and demonstrate backend development, database design, SQL, REST API architecture, testing and Docker in a realistic logistics scenario.