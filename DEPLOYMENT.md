# Fly.io Deployment Summary

**Project:** Vehicle Expense Tracker
**Deployment Date:** January 5, 2026
**Status:** ✅ Successfully Deployed

## Live URLs

- **Frontend (React/Vite):** https://vehicle-expense-frontend.fly.dev
- **Backend (ASP.NET Core 8):** https://vehicle-expense-backend.fly.dev
- **Database:** Fly Postgres (vehicle-expense-db)

## Demo Credentials

- **Email:** demo@vehicletracker.com
- **Password:** Test123

## Architecture

### Frontend
- **Framework:** React 19 + TypeScript + Vite
- **Styling:** TailwindCSS
- **Server:** Nginx (Alpine Linux)
- **Region:** Chicago (ord)
- **Resources:** 1 shared CPU, 256MB RAM

### Backend
- **Framework:** ASP.NET Core 8 Web API
- **Authentication:** JWT Bearer tokens with ASP.NET Identity
- **Region:** Chicago (ord)
- **Resources:** 1 shared CPU, 512MB RAM

### Database
- **Type:** Fly Postgres (unmanaged)
- **Region:** Chicago (ord)
- **Resources:** 1 shared CPU, 256MB RAM
- **Storage:** 1GB

## Deployment Configuration

### Backend Configuration (`backend/fly.toml`)

```toml
app = "vehicle-expense-backend"
primary_region = "ord"

[build]
  dockerfile = "Dockerfile"

[env]
  ASPNETCORE_ENVIRONMENT = "Production"
  ASPNETCORE_URLS = "http://+:8080"
  Jwt__Issuer = "VehicleExpenseAPI"
  Jwt__Audience = "VehicleExpenseClient"
  Jwt__ExpirationMinutes = "60"

[http_service]
  internal_port = 8080
  force_https = true
  auto_stop_machines = false
  auto_start_machines = true
  min_machines_running = 1

[[vm]]
  cpu_kind = "shared"
  cpus = 1
  memory_mb = 512
```

**Secrets Set:**
- `Jwt__SecretKey` - JWT signing secret
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string

### Frontend Configuration (`frontend/fly.toml`)

```toml
app = "vehicle-expense-frontend"
primary_region = "ord"

[build]
  dockerfile = "Dockerfile.prod"
  [build.args]
    VITE_API_URL = "https://vehicle-expense-backend.fly.dev/api"

[env]
  NODE_ENV = "production"

[http_service]
  internal_port = 80
  force_https = true
  auto_stop_machines = false
  auto_start_machines = true
  min_machines_running = 1

[[vm]]
  cpu_kind = "shared"
  cpus = 1
  memory_mb = 256
```

## Key Issues Resolved

### 1. Frontend Build-Time Environment Variables

**Issue:** Vite requires environment variables at build time, not runtime.

**Solution:** Updated `frontend/Dockerfile.prod` to accept build arguments:

```dockerfile
# Accept build argument for API URL
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL
```

And configured `frontend/fly.toml` to pass build arguments:

```toml
[build]
  dockerfile = "Dockerfile.prod"
  [build.args]
    VITE_API_URL = "https://vehicle-expense-backend.fly.dev/api"
```

### 2. JWT Audience Mismatch

**Issue:** Backend was generating JWT tokens with `audience: "VehicleExpenseApp"` but validating with `audience: "VehicleExpenseClient"`, causing all authenticated API requests to return 401 Unauthorized.

**Root Cause:**
- `Program.cs` read JWT config from environment variables (`Jwt__Audience`)
- `AuthService.cs` read JWT config from appsettings.json (`JwtSettings:Audience`)
- This caused a mismatch between token generation and validation

**Solution:** Updated `AuthService.cs` to prioritize environment variables (same pattern as Program.cs):

```csharp
// Support both JwtSettings (local) and Jwt__ (Fly.io env vars)
var secretKey = Environment.GetEnvironmentVariable("Jwt__SecretKey")
    ?? jwtSettings["Secret"]
    ?? throw new InvalidOperationException("JWT Secret not configured");

var issuer = Environment.GetEnvironmentVariable("Jwt__Issuer")
    ?? jwtSettings["Issuer"]
    ?? "VehicleExpenseAPI";

var audience = Environment.GetEnvironmentVariable("Jwt__Audience")
    ?? jwtSettings["Audience"]
    ?? "VehicleExpenseApp";
```

### 3. Login Navigation Race Condition

**Issue:** After successful login, users were redirected back to the login page.

**Solution:** Added a small delay and used `window.location.href` instead of React Router's `navigate()` to ensure localStorage write completes before navigation:

```typescript
await login({ email: formData.email, password: formData.password });

// Give a small delay to ensure localStorage write completes
await new Promise(resolve => setTimeout(resolve, 100));

// Use window.location.href instead of navigate to force a full page reload
window.location.href = '/';
```

## Database Setup

### Connection String Format

Fly.io Postgres connection string format:
```
Host=vehicle-expense-db.flycast;Port=5432;Database=vehicle_expense_backend;Username=vehicle_expense_backend;Password=<password>;SSL Mode=Disable
```

**Note:** Use `.flycast` domain for internal Fly.io network communication, not `.internal`.

### Running Migrations

To run Entity Framework migrations on Fly Postgres:

1. Create a proxy to the database:
```bash
flyctl proxy 15432:5432 -a vehicle-expense-db
```

2. Run migrations from local machine:
```bash
cd backend/src/VehicleExpenseAPI
dotnet ef database update --connection "Host=localhost;Port=15432;Database=vehicle_expense_backend;Username=vehicle_expense_backend;Password=<password>;SSL Mode=Disable"
```

## CORS Configuration

Backend CORS is configured to allow requests from:
- `http://localhost:5173` (Vite dev server)
- `http://localhost:3000` (alternative dev port)
- `https://vehicle-expense-frontend.fly.dev` (production frontend)

## Deployment Commands

### Deploy Backend
```bash
cd backend
flyctl deploy
```

### Deploy Frontend
```bash
cd frontend
flyctl deploy
```

### View Logs
```bash
# Backend logs
flyctl logs -a vehicle-expense-backend

# Frontend logs
flyctl logs -a vehicle-expense-frontend

# Database logs
flyctl logs -a vehicle-expense-db
```

### Check Status
```bash
# Backend status
flyctl status -a vehicle-expense-backend

# Frontend status
flyctl status -a vehicle-expense-frontend

# Database status
flyctl status -a vehicle-expense-db
```

## Cost Optimization

Current configuration uses Fly.io's free tier:
- 3 shared-cpu-1x VMs with 256-512MB RAM
- 1GB total persistent storage
- Auto-stop/start machines enabled for frontend (reduces costs when not in use)

To reduce costs further:
- Consider enabling auto-stop for backend (note: first request after sleep will be slower)
- Use `fly scale count 1` to reduce from 2 machines to 1 per app

## Security Considerations

1. **JWT Secret:** Strong secret key stored in Fly.io secrets (not in repository)
2. **HTTPS:** All traffic forced to HTTPS via `force_https = true`
3. **CORS:** Restricted to specific frontend origins
4. **Database:** Internal network access only via `.flycast` domain
5. **Secrets Management:** All sensitive values stored in Fly.io secrets, not in code

## Troubleshooting

### Backend Health Check Failing

Check health endpoint:
```bash
curl https://vehicle-expense-backend.fly.dev/health
```

View backend logs:
```bash
flyctl logs -a vehicle-expense-backend
```

### Frontend 401 Errors

1. Verify JWT configuration matches between token generation and validation
2. Check browser console for CORS errors
3. Verify token is being stored in localStorage
4. Check Network tab for Authorization header in requests

### Database Connection Issues

1. Verify connection string format uses `.flycast` domain
2. Check if database is running: `flyctl status -a vehicle-expense-db`
3. Test connection via proxy: `flyctl proxy 15432:5432 -a vehicle-expense-db`

## Maintenance

### Updating the Application

1. Make code changes locally
2. Test changes with Docker: `docker-compose up`
3. Commit changes to git
4. Deploy backend: `cd backend && flyctl deploy`
5. Deploy frontend: `cd frontend && flyctl deploy`

### Backing Up the Database

Fly.io does not provide automatic backups for Postgres. To backup:

```bash
# Create proxy connection
flyctl proxy 15432:5432 -a vehicle-expense-db

# Backup using pg_dump
pg_dump -h localhost -p 15432 -U vehicle_expense_backend vehicle_expense_backend > backup.sql
```

### Scaling

To scale resources:

```bash
# Scale VM size
flyctl scale vm shared-cpu-2x --memory 1024 -a vehicle-expense-backend

# Scale machine count
flyctl scale count 2 -a vehicle-expense-backend
```

## Support

For issues or questions:
- Fly.io Documentation: https://fly.io/docs/
- Fly.io Community: https://community.fly.io/
- Project Repository: [Add your GitHub URL]

---

**Last Updated:** January 5, 2026
**Deployed By:** Claude Sonnet 4.5
