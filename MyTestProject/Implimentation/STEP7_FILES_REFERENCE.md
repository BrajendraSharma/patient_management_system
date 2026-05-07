# Step 7: Appointment Scheduling - Files & Structure

## Quick Reference: All Files Created or Modified

### 📦 BACKEND API FILES

#### Data Access Layer - Repositories
```
📄 IAppointmentRepository.cs
   Location: ClinicalPatientManagement.Api/Repositories/
   Type: Interface
   Purpose: Defines contract for appointment data operations
   Methods: GetAll, GetById, Add, Update, Delete, GetByPatientId, GetByDateRange, 
            GetByDate, GetByDateAndStatus, UpdateStatus, GetUpcomingAppointments, HasConflict

📄 AppointmentRepository.cs
   Location: ClinicalPatientManagement.Api/Repositories/
   Type: Implementation
   Purpose: Implements IAppointmentRepository with EF Core operations
   Key Features: Conflict detection (30-min buffer), patient validation, proper indexes
```

#### Business Logic Layer - Services
```
📄 IAppointmentService.cs
   Location: ClinicalPatientManagement.Api/Services/
   Type: Interface
   Purpose: Defines contract for appointment business logic
   Methods: GetAll, GetById, Create, Update, Delete, GetByPatientId, GetByPatientIdAndDateRange,
            GetByDate, GetByDateAndStatus, UpdateStatus, GetUpcomingAppointments, 
            HasConflict, Exists, ValidateAppointmentData

📄 AppointmentService.cs
   Location: ClinicalPatientManagement.Api/Services/
   Type: Implementation
   Purpose: Implements IAppointmentService with validation and business rules
   Key Features: Validation, conflict detection, status management, structured logging
```

#### Interface Adapter Layer - Controllers
```
📄 AppointmentsController.cs
   Location: ClinicalPatientManagement.Api/Controllers/
   Type: REST API Controller
   Purpose: Provides HTTP endpoints for appointment management
   Endpoints: 11 endpoints covering CRUD, filtering, conflict checking
   Auth: All endpoints require JWT authentication
```

#### DTOs - Data Transfer Objects
```
📄 AppointmentDto.cs (MODIFIED)
   Location: ClinicalPatientManagement.Api/DTOs/
   Changes: Added PatientId to UpdateAppointmentDto
   Classes: AppointmentDto, CreateAppointmentDto, UpdateAppointmentDto
```

#### Configuration & Mapping
```
📄 DependencyInjectionExtensions.cs (MODIFIED)
   Location: ClinicalPatientManagement.Api/Extensions/
   Changes: Added AppointmentRepository and IAppointmentService registration

📄 MappingProfile.cs (MODIFIED)
   Location: ClinicalPatientManagement.Api/Mappings/
   Changes: Added AutoMapper mappings for Appointment entities and DTOs
```

---

### 🎨 BLAZOR CLIENT FILES

#### API Communication Layer
```
📄 IAppointmentApiClient.cs
   Location: ClinicalPatientManagement.Client/Services/
   Type: Interface
   Purpose: Contract for HTTP API communication
   Methods: All appointment operations mirroring backend API

📄 AppointmentApiClient.cs
   Location: ClinicalPatientManagement.Client/Services/
   Type: Implementation
   Purpose: HTTP client implementation using HttpClient
   Key Features: Error handling, console logging, async operations
```

#### Client Models
```
📄 AppointmentModel.cs
   Location: ClinicalPatientManagement.Client/Models/
   Classes:
   - AppointmentModel: Full data model with helper properties
   - CreateAppointmentModel: DTO for creation
   - UpdateAppointmentModel: DTO for updates
   Key Features: Formatted date/time properties, status badge styling
```

#### UI Components - Razor Pages
```
📄 Index.razor
   Location: ClinicalPatientManagement.Client/Pages/Appointments/
   Route: /appointments
   Purpose: List all appointments with filtering
   Features: 
   - Responsive table layout
   - Search/filter by patient name, notes, status
   - Quick action buttons (View, Edit, Delete)
   - Patient name resolution
   - Confirmation dialogs

📄 Create.razor
   Location: ClinicalPatientManagement.Client/Pages/Appointments/
   Routes: /appointments/create, /appointments/edit/{id}
   Purpose: Create or edit appointments
   Features:
   - Patient dropdown selection
   - Date/time picker with conflict checking
   - Status selection
   - Notes field (max 500 chars)
   - Success/error messaging
   - Form validation
```

#### Client Configuration
```
📄 Program.cs (MODIFIED)
   Location: ClinicalPatientManagement.Client/
   Changes: Added AppointmentApiClient service registration
```

---

### ✅ TEST FILES

#### Unit Tests
```
📄 AppointmentServiceTests.cs
   Location: ClinicalPatientManagement.Api.Tests/
   Type: xUnit Tests
   Coverage: AppointmentService business logic
   Test Methods: 18 tests covering CRUD, validation, conflict detection
   Mocking: Uses Moq for dependencies

📄 AppointmentsControllerTests.cs
   Location: ClinicalPatientManagement.Api.Tests/
   Type: xUnit Tests
   Coverage: AppointmentsController endpoints
   Test Methods: 17 tests covering all HTTP operations
   Mocking: Uses Moq for IAppointmentService
```

---

## 📊 Database Schema Reference

### Appointment Table (Already Configured)
```sql
CREATE TABLE Appointments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PatientId INT NOT NULL FOREIGN KEY REFERENCES Patients(Id),
    AppointmentDate DATETIME2 NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Scheduled',
    Notes NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2
);

-- Indexes
CREATE INDEX IX_Appointments_PatientId ON Appointments(PatientId);
CREATE INDEX IX_Appointments_AppointmentDate ON Appointments(AppointmentDate);
```

---

## 🔗 Dependencies Between Files

### Backend Flow
```
AppointmentsController
  ↓ depends on
IAppointmentService ← AppointmentService (implementation)
  ↓ depends on
IAppointmentRepository ← AppointmentRepository (implementation)
  ↓ depends on
ClinicalDbContext (EF Core)
  ↓ interacts with
Database (SQL Server)
```

### Frontend Flow
```
Index.razor & Create.razor (UI)
  ↓ depends on
IAppointmentApiClient ← AppointmentApiClient (implementation)
  ↓ uses
AppointmentModel, CreateAppointmentModel, UpdateAppointmentModel
  ↓ communicates with
REST API (/api/appointments/*)
  ↓ calls
AppointmentsController
```

### Dependency Injection
```
Program.cs (Blazor Client)
  ├── Registers: IAppointmentApiClient → AppointmentApiClient
  └── Registers: HttpClient with base address

DependencyInjectionExtensions.cs (Backend API)
  ├── Registers: IAppointmentRepository → AppointmentRepository
  ├── Registers: IAppointmentService → AppointmentService
  └── Registers: AutoMapper with MappingProfile
```

---

## 📝 File Summary Table

| File | Type | Status | Purpose |
|------|------|--------|---------|
| IAppointmentRepository.cs | Interface | ✅ Created | Repository contract |
| AppointmentRepository.cs | Class | ✅ Created | Data access implementation |
| IAppointmentService.cs | Interface | ✅ Created | Service contract |
| AppointmentService.cs | Class | ✅ Created | Business logic implementation |
| AppointmentsController.cs | Controller | ✅ Created | API endpoints |
| AppointmentDto.cs | DTO | ⚠️ Modified | Updated for appointment operations |
| DependencyInjectionExtensions.cs | Extension | ⚠️ Modified | Service registration |
| MappingProfile.cs | Mapping | ⚠️ Modified | AutoMapper configuration |
| IAppointmentApiClient.cs | Interface | ✅ Created | Client API contract |
| AppointmentApiClient.cs | Class | ✅ Created | HTTP client implementation |
| AppointmentModel.cs | Model | ✅ Created | Client-side model |
| Index.razor | Page | ✅ Created | Appointment list UI |
| Create.razor | Page | ✅ Created | Appointment form UI |
| Program.cs | Config | ⚠️ Modified | Client service registration |
| AppointmentServiceTests.cs | Tests | ✅ Created | Service unit tests (18 tests) |
| AppointmentsControllerTests.cs | Tests | ✅ Created | Controller unit tests (17 tests) |

---

## 🎯 Key Implementation Highlights

### 1. Validation Layer
- Patient existence check before creating appointments
- Appointment date validation (cannot be in past)
- Status validation (limited to valid status values)
- Conflict detection (30-minute buffer)

### 2. Error Handling
- Structured error messages with context
- HTTP status code mapping:
  - 200 OK: Success
  - 201 Created: Resource created
  - 204 No Content: Delete success
  - 400 Bad Request: Validation error
  - 401 Unauthorized: Auth required
  - 404 Not Found: Resource not found
  - 409 Conflict: Duplicate/conflict error
  - 500 Internal Server Error: Unhandled exception

### 3. Logging
- Serilog structured logging in services
- Log levels: Information, Warning, Error
- Context includes: Operation, User data, IDs, timestamps

### 4. Testing
- 35+ unit tests total
- Mock-based testing with Moq
- Coverage: Happy path, edge cases, error conditions
- Follows AAA pattern: Arrange, Act, Assert

---

## 🚀 How to Use

### Creating an Appointment (Client Side)
1. Navigate to `/appointments/create`
2. Select patient from dropdown
3. Choose appointment date and time
4. System checks for conflicts automatically
5. Select status
6. Add optional notes
7. Click "Schedule Appointment"

### Creating an Appointment (API)
```http
POST /api/appointments
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "patientId": 1,
  "appointmentDate": "2024-06-15T14:30:00Z",
  "status": "Scheduled",
  "notes": "Regular checkup"
}
```

### Checking for Conflicts
```http
GET /api/appointments/conflict?patientId=1&appointmentDate=2024-06-15T14:30:00Z
Authorization: Bearer {jwt_token}
```

### Updating Status
```http
PATCH /api/appointments/1/status
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "status": "Completed"
}
```

---

## ✨ Next Steps for Development

1. **Step 8**: Enhance patient search functionality
   - No breaking changes needed here
   - All appointment code remains compatible

2. **Future Enhancements**:
   - Add appointment reminders/notifications
   - Calendar view for appointments
   - Bulk status updates
   - Export appointments to calendar format
   - Appointment duration tracking
   - Resource/room scheduling

---

## 📋 Verification Commands

```bash
# Build solution
dotnet build

# Run tests
dotnet test ClinicalPatientManagement.Api.Tests

# Run specific test class
dotnet test ClinicalPatientManagement.Api.Tests --filter "ClassName=AppointmentServiceTests"

# Check for compilation errors
dotnet build --no-restore
```
