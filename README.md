# Project Overview

**E-InsuranceApp-Monocept** is a full-featured, enterprise-grade insurance management system built with ASP.NET Core MVC 8.0. It implements a clean architecture pattern with robust authentication, policy management, payment processing (Stripe), commission calculations, and comprehensive admin capabilities.

---

## Key Features

### Multi-Role User Management
- **Customers**: Register, purchase policies, view policies, make payments, download receipts
- **Insurance Agents**: Manage assigned customers, earn commissions on policies sold
- **Employees**: Process policies, manage customer relationships
- **Administrators**: Full system control, user management, policy oversight, commission management

### Core Functionality
- **Policy Management**: Create, view, and manage insurance policies with premium calculations
- **Payment Processing**: Integrated Stripe payment gateway with secure checkout
- **Commission System**: Automated commission calculations for agents with batch processing and reporting
- **PDF Generation**: Professional payment receipts using QuestPDF
- **Session Management**: Secure session handling with validation middleware
- **Rate Limiting**: Protection against brute force attacks on authentication endpoints
- **Data Seeding**: Development data seeder for testing and demo environments

### Security Features
- JWT-based authentication with cookie storage
- Role-based authorization (Admin, Employee, InsuranceAgent, Customer)
- Password hashing with ASP.NET Core Identity
- Input validation and sanitization
- Session validation middleware

---

## Technology Stack

| Layer | Technology |
|-------|------------|
| **Framework** | .NET 8.0 (ASP.NET Core MVC) |
| **Database** | Microsoft SQL Server |
| **ORM** | Entity Framework Core 8.0 |
| **Authentication** | JWT Bearer Tokens |
| **Payment Gateway** | Stripe.net (v51.0.1) |
| **PDF Generation** | QuestPDF 2024.10.2 |
| **Logging** | Serilog (Console + File) |
| **Frontend** | Bootstrap 5, Bootstrap Icons |
| **Architecture** | Clean Architecture / Layered |
| **Design Patterns** | Repository, Dependency Injection, Service Layer |

---

## Project Structure

```
EInsurance/
├── Controllers/           # MVC Controllers (API & View)
├── Services/              # Business logic layer
│   ├── Authentication/
│   ├── Registration/
│   ├── Policies/
│   ├── Premium/
│   ├── Commission/
│   └── Session/
├── Repository/            # Data access layer
├── Interfaces/            # Service contracts
├── Domain/                # Entity models
│   ├── Entities/          # Database tables
│   └── Common/            # Base classes (AuditableEntity, AccountEntity, PersonEntity)
├── Models/                # ViewModels & DTOs
│   ├── Auth/
│   ├── Policies/
│   ├── Admin/
│   ├── Commission/
│   └── Payment/
├── Data/                  # EF Core context & migrations
│   └── Seed/              # Development data seeder
├── Extensions/            # Service collection extensions
├── Middleware/            # Custom HTTP middleware
├── Security/              # Validation attributes & role constants
├── Configuration/         # App settings classes
├── Views/                 # Razor views
└── wwwroot/              # Static files
```

---

## Architecture

### Clean Architecture Layers

```
Presentation Layer (Controllers + Views)
         ↓
Application Layer (Services + Interfaces)
         ↓
Domain Layer (Entities + Common)
         ↓
Infrastructure Layer (Repository + EF Core)
```

### Key Design Patterns
- **Repository Pattern**: Abstraction over data access (`IPolicyRepository`, `IRegistrationRepository`)
- **Service Layer**: Business logic separation (`IPolicyService`, `ICommissionCalculationService`)
- **Dependency Injection**: All services registered via extension methods
- **DRY Principle**: Shared base entities and validation services
- **Middleware Pipeline**: Custom session validation, rate limiting

---

## Database Schema

### Core Entities

| Entity | Description |
|--------|-------------|
| `Customer` | Registered users who purchase policies |
| `Admin` | System administrators |
| `Employee` | Company employees with defined roles |
| `InsuranceAgent` | Agents who sell policies and earn commissions |
| `InsurancePlan` | Master plan definitions (e.g., Life, Health) |
| `Scheme` | Specific plan variants with pricing details |
| `Policy` | Insurance policy purchased by customer |
| `Payment` | Transaction records with Stripe integration |
| `Commission` | Agent commission tracking |
| `UserSession` | Active user sessions for validation |

### Relationships
- **Customer → Agent** (Many-to-One): Customers can be assigned to agents
- **Plan → Scheme** (One-to-Many): Plans have multiple schemes
- **Scheme → Policy** (One-to-Many): Schemes can be purchased multiple times
- **Customer → Policy** (One-to-Many): Customers can have multiple policies
- **Policy → Payment** (One-to-Many): Policies have multiple payments
- **Agent → Commission** (One-to-Many): Agents earn commissions per policy
- **Policy → Commission** (One-to-Many): Policies generate commissions

### Auditing
All auditable entities inherit from `AuditableEntity` with automatic `CreatedAt` timestamp (UTC).

---

## Security Implementation

### Authentication Flow
1. User submits credentials via login form
2. `UserAuthenticationService` validates against database
3. `JwtTokenService` generates JWT with claims (userId, role, sessionId)
4. Token stored in **HttpOnly, Secure cookie** (`EInsurance.Token`)
5. `SessionValidationMiddleware` validates session on each request
6. JWT Bearer authentication extracts token from cookie

### Authorization
- Role-based access using `[Authorize(Roles = "...")]` attribute
- Defined roles: `Admin`, `Employee`, `InsuranceAgent`, `Customer`
- Composite role: `AdminOrEmployee` for shared admin/employee features
- Policy-level authorization (e.g., customers can only see their own policies)

### Additional Security
- **Rate Limiting**: 5 attempts per 15 minutes on /account/login and /account/register
- **Password Hashing**: `PasswordHasher<object>` from Microsoft.AspNetCore.Identity
- **Custom Validators**: `StrongPasswordAttribute`, `MinimumAgeAttribute`, `ValidRoleAttribute`
- **Anti-Forgery Tokens**: All POST actions protected with `[ValidateAntiForgeryToken]`
- **SQL Injection Protection**: Parameterized queries via Entity Framework
- **XSS Protection**: Razor views auto-encode output

---

## API Reference

### Public Endpoints

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/Account/Login` | Login page |
| POST | `/Account/Login` | Authenticate user |
| GET | `/Account/Register` | Registration page |
| POST | `/Account/Register` | Create customer account |
| POST | `/Account/Logout` | Invalidate session & clear cookie |

### Customer Endpoints

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/Policies/MyPolicies` | View purchased policies |
| GET | `/PolicyPurchase/AvailableSchemes` | Browse available insurance schemes |
| POST | `/PolicyPurchase/Purchase` | Purchase a policy |
| GET | `/Payment/PaymentHistory` | View payment history |
| GET | `/Payment/DownloadReceipt/{id}` | Download PDF receipt |

### Admin/Employee Endpoints

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/Admin/AllCustomers` | List all customers (paginated) |
| GET | `/Admin/AllPayments` | View all payments |
| GET | `/Admin/ManageUsers` | User management interface |
| POST | `/Admin/DeleteUser/{id}` | Delete user account |
| GET/POST | `/Admin/EditUser/{id}` | Edit user details |
| GET | `/Admin/AssignAgent` | Assign agent to customer |
| POST | `/Admin/AssignAgent` | Save agent assignment |

### Commission Endpoints (Admin only)

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/Admin/Commission/Dashboard` | Commission overview |
| GET | `/Admin/Commission/Calculator` | Commission calculation form |
| POST | `/Admin/Commission/Calculate` | Calculate agent commission |
| POST | `/Admin/Commission/Finalize` | Finalize commission payment |
| POST | `/Admin/Commission/BatchCalculate` | Batch calculate all agents |
| POST | `/Admin/Commission/MarkAsPaid/{id}` | Mark commission as paid |
| GET | `/Admin/Commission/Ledger/{agentId}` | Agent commission ledger |

### Stripe Integration (AJAX)

| Method | Route | Purpose |
|--------|-------|---------|
| POST | `/stripe/create-payment-intent` | Create Stripe PaymentIntent |
| GET | `/stripe/get-payment-status/{id}` | Check payment status |
| GET | `/stripe/publishable-key` | Get Stripe publishable key |

---

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "EInsuranceConnection": "Server=.;Database=EInsuranceDb;Trusted_Connection=True;"
  },
  "Jwt": {
    "Issuer": "EInsurance",
    "Audience": "EInsuranceUsers",
    "Key": "YOUR_SECURE_KEY_HERE",
    "ExpiryMinutes": 120
  },
  "Stripe": {
    "PublishableKey": "pk_test_...",
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
  }
}
```

### Environment Variables
- `STRIPE_PUBLISHABLE_KEY`: Alternative Stripe key source
- `ASPNETCORE_ENVIRONMENT`: `Development` or `Production`

### Required Packages
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.0)
- `Microsoft.EntityFrameworkCore.SqlServer` (8.0.0)
- `Stripe.net` (51.0.1)
- `Serilog.AspNetCore` (8.0.0)
- `QuestPDF` (2024.10.2)

---

## Installation & Setup

### Prerequisites
- .NET 8.0 SDK
- SQL Server (Express or higher)
- Stripe account (for payment processing)

### Steps

1. **Clone repository**
   ```bash
   git clone <repository-url>
   cd E-InsuranceApp-Monocept/EInsurance
   ```

2. **Configure connection string**
   - Edit `appsettings.json`
   - Update `EInsuranceConnection` to point to your SQL Server instance

3. **Update JWT & Stripe keys**
   - Generate a secure JWT key (min 32 chars)
   - Replace values in `Jwt` and `Stripe` sections

4. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Seed development data** (optional)
   - In `Program.cs`, the `SeedDevelopmentData()` extension runs automatically in Development environment
   - Creates sample admins, agents, schemes, and plans

6. **Run the application**
   ```bash
   dotnet run
   ```

7. **Access the app**
   - Navigate to `https://localhost:5001` (or port shown in console)
   - Default admin credentials (if seeded): Check `DevelopmentDataSeeder.cs`

---

## Development Data Seeding

The `DevelopmentDataSeeder` automatically creates:
- **Admin user**: admin@einsurance.com
- **Insurance Agents**: Sample agents with commission rates
- **Insurance Plans**: Life, Health, Auto categories
- **Schemes**: Specific plan variants with JSON-based calculation parameters
- **Sample Customer**: For testing registration flow

To disable seeding, set environment to `Production` or comment out `app.SeedDevelopmentData()` in `Program.cs:39`.

---

## Testing

No formal test suite is included, but manual testing can be performed:

1. **Authentication Flow**
   - Register as customer
   - Login and verify session cookie
   - Logout and verify session invalidation

2. **Policy Purchase**
   - Browse available schemes as customer
   - Calculate premium for different parameters
   - Complete Stripe test payment (use test cards: 4242 4242 4242 4242)

3. **Admin Functions**
   - Create/edit/delete users
   - Assign agents to customers
   - View all policies and payments
   - Calculate and finalize commissions

4. **Stripe Webhooks**
   - Endpoint: `/StripeWebhook/Handle` (requires webhook secret)
   - Configure in Stripe Dashboard to point to your deployed URL

---

## Deployment Considerations

### Production Checklist
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Use strong JWT key (store in Azure Key Vault or environment variable)
- [ ] Configure real SQL Server (not localhost)
- [ ] Update Stripe keys to live mode
- [ ] Enable HTTPS-only (already configured via `UseHttpsRedirection`)
- [ ] Configure Serilog to write to centralized logging (Application Insights, Seq)
- [ ] Set up database backups
- [ ] Configure CORS if serving frontend separately
- [ ] Review rate limiting thresholds
- [ ] Enable HSTS (`app.UseHsts()`)

### Docker Deployment (Optional)
Create a `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EInsurance/EInsurance.csproj", "EInsurance/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/EInsurance"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EInsurance.dll"]
```

---

## Contributing

Contributions are welcome. Please follow these guidelines:

1. **Branch naming**: `feature/`, `bugfix/`, `hotfix/` prefixes
2. **Code style**: Follow existing patterns (PascalCase for methods, camelCase for locals)
3. **Dependency injection**: Register new services in `ServiceCollectionExtensions.cs`
4. **Database changes**: Create a new migration via `dotnet ef migrations add <Name>`
5. **Validation**: Use `IDataValidationService` for all input validation
6. **Testing**: Manually test role-based access and edge cases
7. **Security**: Never commit secrets; use environment variables or user-secrets

---

## Support

For issues, questions, or feature requests, please open an issue on the repository or contact the development team.

---

**E-InsuranceApp-Monocept** — Enterprise Insurance Management Solution