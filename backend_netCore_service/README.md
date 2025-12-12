# Auth API Sample

Minimal ASP.NET Core Web API demonstrating JWT-based authentication with role-protected endpoints (Admin/User), YAML configuration, database persistence (MySQL or MongoDB), CORS configuration, and integration tests using xUnit + WebApplicationFactory.

## Prerequisites

- .NET SDK 9.0 (or later)
- MySQL Server 8.0 or later (for MySQL provider) or MongoDB (for MongoDB provider)
- PowerShell (or any shell supported by the .NET CLI)

## Restore dependencies

```pwsh
cd d:\Temp\Time-Management-App\backend_netCore_service
dotnet restore
```

## Setup the database

Ensure your chosen database is running and create the database, then apply migrations (for MySQL):

```pwsh
cd d:\Temp\Time-Management-App\backend_netCore_service\AuthApi
dotnet ef database update
```

This will create the `netcore_auth_xunit` database, the `Users` table, and seed the default users.

For MongoDB, no migrations are needed as collections are created dynamically.

## Run the API locally

```pwsh
cd d:\Temp\Time-Management-App\backend_netCore_service\AuthApi
// default is dev
dotnet run
// OR same by sepecify it
dotnet run -- --env dev
```

For production mode:

```pwsh
dotnet run -- --env prod
```

By default, the application listens on URLs configured in `dev.appsettings.yaml` (for dev) or `prod.appsettings.yaml` (for prod):
- Dev: `http://localhost:5000`, `https://localhost:5001`
- Prod: `https://localhost:5001`

### Configure the server URLs

Update `AuthApi/dev.appsettings.yaml` or `AuthApi/prod.appsettings.yaml` under the `Server:Urls` section. The URLs listed here are used when running via `dotnet run` or Visual Studio launch profiles.

Example for dev:
```yaml
Server:
  Urls: "http://localhost:5000;https://localhost:5001"
```

Example for prod:
```yaml
Server:
  Urls: "https://localhost:5001"
```

### Configure CORS allowed origins

Update `AuthApi/dev.appsettings.yaml` or `AuthApi/prod.appsettings.yaml` under the `Cors:AllowedOrigins` section to specify allowed frontend origins.

Example for dev:
```yaml
Cors:
  AllowedOrigins: "http://localhost:3000,http://localhost:3001"
```

Example for prod:
```yaml
Cors:
  AllowedOrigins: "https://prod-frontend.com"
```

### Configure users, passwords, and roles

For this sample the credentials are seeded in the database via Entity Framework Core migrations (for MySQL) or directly in code (for MongoDB) in `AuthApi/AuthDbContext.cs`.

### Configure users, passwords, and roles

For this sample the credentials are seeded in the MySQL database via Entity Framework Core migrations in `AuthApi/AuthDbContext.cs`.

The default seeded users are:
- `admin@example.com` / `Admin123!` (Admin role)
- `user@example.com` / `User123!` (User role)

To add, remove, or change users, modify the `OnModelCreating` method in `AuthDbContext.cs`. Password hashes are automatically generated using ASP.NET Core Identity's password hasher.

### Configure JWT settings

Edit `AuthApi/dev.appsettings.yaml` or `AuthApi/prod.appsettings.yaml` under the `Jwt` section:
- `Key`: symmetric signing key (keep at least 32 chars; store securely in production)
- `Issuer` / `Audience`: expected token issuer and audience
- `ExpirationMinutes`: token expiration time in minutes (default: 60)

### Configure database provider

Edit `AuthApi/dev.appsettings.yaml` or `AuthApi/prod.appsettings.yaml` under the `Database:Provider` section:
- Set to `"MySQL"` for MySQL database
- Set to `"MongoDB"` for MongoDB database

### Configure connection strings

Edit `AuthApi/dev.appsettings.yaml` or `AuthApi/prod.appsettings.yaml` under the `ConnectionStrings` section to configure the database connection.

For MySQL (default):
```yaml
ConnectionStrings:
  DefaultConnection: "server=localhost;port=3306;user=root;password=yourpassword;database=netcore_auth_xunit;"
```

For MongoDB:
```yaml
ConnectionStrings:
  MongoConnection: "mongodb://localhost:27017/netcore_auth_xunit"
```

## API Documentation with Swagger

When running in development mode, the API provides interactive documentation via Swagger UI.

### Accessing Swagger UI

1. Start the application in development mode
2. Open your browser and navigate to: `http://localhost:5000/swagger` or `https://localhost:5001/swagger`
3. The Swagger UI will display all available endpoints with their parameters and response schemas
4. You can test endpoints directly from the UI by clicking "Try it out"

### OpenAPI Specification

The OpenAPI JSON specification is available at: `http://localhost:5000/openapi/v1.json` or `https://localhost:5001/openapi/v1.json`

## Run the integration tests

The tests build an in-memory server and exercise the authentication flow end-to-end.

```pwsh
cd d:\Temp\Time-Management-App\backend_netCore_service
dotnet test
```

## Calling the API

1. POST `/api/auth/login` with JSON body `{"username":"admin@example.com","password":"Admin123!"}`.
2. Use the returned `accessToken` as a bearer token when calling protected endpoints, e.g. `Authorization: Bearer <token>`.
3. `GET /api/admin/reports` requires the `Admin` role; `GET /api/users/profile` and `GET /api/reports/daily` allow `User` or `Admin`.
4. `GET /api/health` is a public health check endpoint.

## Notes

- Users are stored in the configured database (MySQL or MongoDB) with proper password hashing using ASP.NET Core Identity (for MySQL) or custom hashing (for MongoDB).
- Store secrets (such as JWT keys and database passwords) outside of source control for real deployments (user-secrets, environment variables, vaults, etc.).
- Configuration is read from `dev.appsettings.yaml` or `prod.appsettings.yaml` for flexibility and readability, including server URLs, CORS origins, and database settings.
- Database migrations are applied automatically on startup for MySQL. MongoDB collections are created dynamically.
- In development environment, HTTPS redirection is disabled to allow HTTP requests for easier testing with CORS.
