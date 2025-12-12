# FastAPI Authentication with MongoDB

A role-based authentication API built with FastAPI, using JWT tokens, and MongoDB for user storage via the Beanie ODM (Motor + PyMongo).

## Features

- JWT-based authentication
- Role-based access control (admin/user roles)
- MongoDB user storage using Beanie ODM documents and Motor (Async MongoDB)
- CORS support
- Configurable via YAML
- API Documentation with Swagger

## Setup

### Prerequisites

- Python 3.12+
- MongoDB 3.6+ running on `localhost:27017`

> **MongoDB 3.6 compatibility**
>
> Dependencies are pinned to `beanie==1.26.0`, `motor==2.5.1`, and `pymongo==3.13.0` so the service continues to work with clusters that have not yet upgraded beyond MongoDB 3.6.

### Installation

1. Create a virtual environment:
   ```bash
   py -3.12 -m venv venv
   ```

2. Activate the virtual environment:
   ```bash
   .\venv\Scripts\activate  # Windows
   # or
   source venv/bin/activate  # Linux/Mac
   ```

3. Install dependencies:
   ```bash
   pip install -r requirements.txt
   ```

4. Seed the database with initial users:
   ```bash
   python seed.py
   ```

   The seed script drops the `users` collection before re-inserting the default admin/user accounts, so it is safe to re-run whenever you need a clean slate.

## Configuration

The application supports environment-specific configuration via YAML files:

- `dev.config.yaml` - Development configuration (default)
- `prod.config.yaml` - Production configuration

Configuration options include:

- **JWT Key**: Secret key for JWT token signing
- **JWT Timeout**: Token expiration time in minutes
- **MongoDB Connection**: Database connection string
- **Server URLs**: HTTP/HTTPS endpoints
- **CORS Origins**: Allowed origins for CORS

## Running the Application

Start the server with the default development configuration:
```bash
python app/main.py
```

Or specify the environment:
```bash
python app/main.py --env dev    # Development (default)
python app/main.py --env prod   # Production
```

The API will be available at `http://localhost:5000` (dev) or the configured production URL.

## API Documentation with Swagger

When running in development mode, the API provides interactive documentation via Swagger UI.

### Accessing Swagger UI

Start the application in development mode

Open your browser and navigate to:
- for swagger API http://127.0.0.1:5000/docs#/
- for fastAPI http://127.0.0.1:5000/redoc#/

The Swagger UI will display all available endpoints with their parameters and response schemas

You can test endpoints directly from the UI by clicking "Try it out"

OpenAPI Specification

The OpenAPI JSON specification is available at: http://localhost:5000/openapi.json or https://localhost:5001/openapi.json

## API Endpoints

### Public Endpoints

- `GET /info` - Service information

### Authentication

- `POST /login` - Login with username/password, returns JWT token

### Protected Endpoints

- `GET /users/me` - Get current user info (requires authentication)
- `GET /admin/dashboard` - Admin-only endpoint (requires admin role)

## Default Users

After seeding, the following users are available:

- **Admin**: `admin@example.com` / `Admin123!`
- **User**: `user@example.com` / `User123!`

Passwords are hashed with `passlib.pbkdf2_sha256`, so you can safely change the defaults inside `seed.py` if needed.

## Testing

Run tests with pytest:
```bash
pytest
```

Or use VS Code's testing tab.

you need to open *.py from tests folder for py tests discovery in vsc

> **Windows event loop note**
>
> The service programmatically switches to `asyncio.WindowsSelectorEventLoopPolicy()` so Motor 2.x can run reliably on Windows. No manual action is required, but this is why the tests now pass without the "event loop is closed" error.

## Project Structure

```
backend_python_service/
├── app/
│   ├── __init__.py
│   ├── auth.py       # FastAPI app + Beanie models + JWT logic
│   └── main.py       # Server entry point
├── tests/
│   ├── conftest.py
│   └── test_auth.py  # API tests (pytest/TestClient)
├── dev.config.yaml   # Development configuration
├── prod.config.yaml  # Production configuration
├── config.yaml       # Legacy configuration (fallback)
├── seed.py           # Database seeding script (Beanie-based)
├── requirements.txt  # Python dependencies pinned for MongoDB 3.6
└── readme.md         # This file
```

## Helpful Commands

Common tasks from the repository root (after creating/activating the virtual environment):

```powershell
cd backend_python_service
python seed.py
python -m pytest
python app/main.py                    # Run with dev config
python app/main.py --env prod         # Run with prod config
```
