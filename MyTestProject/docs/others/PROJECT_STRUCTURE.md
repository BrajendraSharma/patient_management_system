# Clinical Patient Management System - Project Structure

## Overview
This document outlines the layered architecture of the Clinical Patient Management System following Clean Architecture principles.

## Project Structure

```
ClinicalPatientManagement/
│
├── ClinicalPatientManagement.Api/
│   ├── Controllers/                    # API Endpoints (Interface Adapters)
│   │   └── HealthController.cs
│   │
│   ├── Services/                       # Business Logic Layer
│   │   ├── IService.cs                 # Service interface (generic)
│   │   └── [Service implementations added in Steps 6+]
│   │
│   ├── Repositories/                   # Data Access Layer
│   │   ├── IRepository.cs              # Generic repository interface
│   │   ├── IUnitOfWork.cs              # Unit of Work pattern
│   │   └── [Repository implementations added in Step 3]
│   │
│   ├── Models/                         # Domain Entities (Business Rules)
│   │   └── BaseEntity.cs               # Base entity with audit properties
│   │
│   ├── DTOs/                           # Data Transfer Objects (Interface Adapters)
│   │   ├── PatientDto.cs
│   │   └── AppointmentDto.cs
│   │
│   ├── Data/                           # Entity Framework Core Configuration
│   │   └── ClinicalDbContext.cs        # DbContext with entity mappings
│   │
│   ├── Mappings/                       # AutoMapper Profiles
│   │   └── MappingProfile.cs
│   │
│   ├── Extensions/                     # Dependency Injection & Extensions
│   │   └── DependencyInjectionExtensions.cs
│   │
│   ├── Program.cs                      # Application entry point
│   ├── appsettings.json               # Configuration
│   └── appsettings.Development.json
│
├── ClinicalPatientManagement.Client/
│   ├── Pages/                          # Blazor Pages
│   │   └── Index.razor
│   │
│   ├── Components/                     # Reusable Blazor Components
│   │   └── [Components added in Steps 6+]
│   │
│   ├── Services/                       # Client-side Services
│   │   ├── IApiClient.cs               # HTTP API communication
│   │   └── [Service implementations added in later steps]
│   │
│   ├── Models/                         # Client-side DTOs
│   │   └── BaseDto.cs
│   │
│   ├── App.razor                       # Root component
│   ├── Program.cs                      # Blazor initialization
│   └── appsettings.json
│
└── ClinicalPatientManagement.Api.Tests/
    ├── HealthControllerTests.cs
    ├── BaseEntityTests.cs
    └── [Additional tests added in Steps 14+]
```

## Layered Architecture Explanation

### Layer 1: Domain/Entities (Innermost)
- **Purpose**: Contains core business rules independent of any framework
- **Location**: `Models/` folder
- **Examples**: Patient, Appointment, Consultation, Prescription
- **Rule**: CANNOT depend on outer layers

### Layer 2: Use Cases/Application Services
- **Purpose**: Implements business logic and workflows
- **Location**: `Services/` folder
- **Interfaces**: `IService<T>` and specific service interfaces
- **Examples**: PatientService, AppointmentService, ConsultationService
- **Rule**: Depends only on entities and abstractions

### Layer 3: Interface Adapters
- **Purpose**: Converts between external formats and internal representations
- **Sub-layers**:
  - **Controllers**: HTTP endpoints (`Controllers/` folder)
  - **DTOs**: Data Transfer Objects (`DTOs/` folder)
  - **Repositories**: Data access abstractions (`Repositories/` folder)
- **Rule**: Depends on inner layers

### Layer 4: Frameworks & Drivers (Outermost)
- **Purpose**: External libraries and frameworks
- **Includes**: Entity Framework Core, Serilog, AutoMapper
- **Configuration**: `appsettings.json`, `Program.cs`

## Dependency Flow

```
Controllers
   ↓ (depends on)
Services
   ↓ (depends on)
Repositories
   ↓ (depends on)
Domain Entities
   ↑ (never depends on outer layers)
```

## Key Abstractions

### IRepository<T>
Generic repository interface for CRUD operations
- Added in Step 2
- Implemented in Step 3 with Entity Framework
- Used by services for data access

### IService<T>
Generic service interface for business logic
- Added in Step 2
- Implemented in Steps 6+ with specific business logic
- Called by controllers

### IUnitOfWork
Transaction management across repositories
- Added in Step 2
- Ensures ACID compliance (Step 11)
- Manages database transactions

### IApiClient
Client-side HTTP communication with API
- Added in Step 2
- Implemented in later steps
- Used by Blazor pages/components

## Testing Strategy

### Unit Tests
- Test services with mocked repositories
- Test business logic in isolation
- Location: `ClinicalPatientManagement.Api.Tests/`

### Integration Tests
- Test API controllers with real database (test containers)
- Test end-to-end flows
- Location: `ClinicalPatientManagement.Api.Tests/`

### UI Tests
- Test Blazor components and pages
- Location: (to be added in Step 16)

## Dependency Injection

All services are registered in `DependencyInjectionExtensions.cs`:
```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // DbContext registration
    // Repository registrations
    // Service registrations
    // AutoMapper configuration
}
```

Called from `Program.cs` to set up the DI container.

## Configuration Management

- **appsettings.json**: Production settings
  - Database connection string
  - JWT configuration
  - Logging levels
- **appsettings.Development.json**: Development overrides
  - Debug logging
  - Development database (LocalDB)

## Next Steps

### Step 3: Database Schema
- Create entity models (Patient, Appointment, etc.)
- Implement repository classes
- Create EF Core migrations
- Configure entity relationships in DbContext

### Step 4: Authentication
- Implement AuthController
- Set up JWT token generation
- Integrate with ASP.NET Identity

### Step 5: Logging
- Enhance Serilog configuration
- Add middleware for request/response logging
- Implement audit logging

### Step 6: Patient Management
- Create Patient entity
- Implement PatientService
- Create PatientRepository
- Implement PatientsController
- Create Blazor pages (Create, Edit, Index)

---

**Current Status**: Step 2 Complete - Project structure scaffolded
**Verification**: Run `dotnet restore && dotnet build` to verify setup
