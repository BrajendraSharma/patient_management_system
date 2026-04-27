# Clinical Patient Management System - Implementation Planning Document

## 1. Planning Scope Summary

This planning document outlines the implementation of a web-based clinical patient management system for a single general physician in a single clinic. The system digitizes patient management, appointment scheduling, consultation workflows, prescription generation, patient history tracking, and data export. It is based on the finalized brainstorming analysis, with all ambiguities resolved and assumptions approved. The scope includes patient registration, search, appointments, consultations (vitals, complaints, diagnosis, medications), prescriptions, history, and export to Excel/PDF. Non-functional requirements cover usability, performance, reliability, security, scalability, and compatibility. Technology stack is fixed: Blazor frontend, .NET 8 Web API backend, SQL Server database, ASP.NET Identity/JWT authentication, Serilog logging, xUnit testing.

## 2. Preconditions

- Finalized brainstorming-analysis.md document with resolved ambiguities and approved assumptions.
- Development environment set up with .NET 8 SDK, Visual Studio 2022, SQL Server 2022 Express.
- Access to Azure for hosting (App Service, SQL Database, Key Vault).
- Approval from stakeholders for the resolved assumptions (e.g., patient volume, search behavior, prescription fields).
- No unresolved ambiguities or open questions in the brainstorming analysis.

## 3. High-Level Execution Phases

- Phase 1: Foundation & Setup - Establish project structure, database schema, authentication, and logging.
- Phase 2: Core Features - Implement patient management, appointment scheduling, and search functionality.
- Phase 3: Consultation Workflow - Develop consultation capture, prescription generation, and persistence.
- Phase 4: Advanced Features - Add patient history and data export capabilities.
- Phase 5: Testing & Quality Assurance - Conduct unit, integration, and UAT testing.
- Phase 6: Deployment & Monitoring - Set up DevOps, deploy to production, and validate backups/recovery.

## 4. Detailed Step-by-Step Plan

1. **Step Name**: Set up development environment  
   **Objective**: Prepare the local development setup for the project.  
   **Inputs**: .NET 8 SDK, Visual Studio 2022, SQL Server 2022 Express, Azure CLI.  
   **Expected Outputs**: Installed tools, created solution with Blazor WebAssembly and Web API projects.  
   **Verification Method**: Run `dotnet --version` and confirm .NET 8; open solution in Visual Studio and build successfully.  
   **Requirement Reference(s)**: Non-Functional Requirements - Compatibility (modern browsers), Scalability (single clinic).

2. **Step Name**: Scaffold project structure  
   **Objective**: Create folders and add NuGet packages for the layered architecture.  
   **Inputs**: Solution created in Step 1.  
   **Expected Outputs**: Folders for Controllers, Services, Models, DTOs, Pages, Components; NuGet packages added (EF Core, AutoMapper, xUnit, etc.).  
   **Verification Method**: Inspect project structure in Visual Studio; run `dotnet restore` successfully.  
   **Requirement Reference(s)**: Maintainability (layered structure: Controllers → Services → Repositories).

3. **Step Name**: Create database schema  
   **Objective**: Design and implement SQL Server tables with constraints and indexes.  
   **Inputs**: Approved database schema from brainstorming analysis.  
   **Expected Outputs**: SQL scripts for Patients, Appointments, Consultations, Prescriptions, Medications tables; EF migrations.  
   **Verification Method**: Run migrations and verify tables exist in SQL Server with correct columns/constraints.  
   **Requirement Reference(s)**: Functional Requirements - Patient Management, Appointment Management, Consultation Workflow; Performance (indexes for fast search).

4. **Step Name**: Implement authentication  
   **Objective**: Set up ASP.NET Identity for single-user login and JWT token handling.  
   **Inputs**: Project structure from Step 2.  
   **Expected Outputs**: AuthController, JWT configuration in appsettings.json, login page in Blazor.  
   **Verification Method**: Attempt login; verify JWT token generation and API authorization (401 without token).  
   **Requirement Reference(s)**: Security (single-user authentication, data encryption); Users and Stakeholders (single physician).

5. **Step Name**: Set up logging  
   **Objective**: Integrate Serilog for structured logging and audit trails.  
   **Inputs**: Project structure.  
   **Expected Outputs**: Serilog configuration, logging middleware for critical operations.  
   **Verification Method**: Trigger a log event (e.g., login) and verify logs in console/file.  
   **Requirement Reference(s)**: Reliability (no data loss, audit logging); Security (audit for data access).

6. **Step Name**: Implement patient management  
   **Objective**: Build CRUD for patients including registration, edit, view, and search.  
   **Inputs**: Database schema from Step 3, authentication from Step 4.  
   **Expected Outputs**: PatientsController, PatientService, Patient model/DTO, Blazor pages (Create, Edit, Index).  
   **Verification Method**: Register a patient via UI, search by name/phone, verify data in DB.  
   **Requirement Reference(s)**: Functional Requirements - Patient Management, Patient Search.

7. **Step Name**: Implement appointment scheduling  
   **Objective**: Enable scheduling and tracking appointments with status updates.  
   **Inputs**: Patient management from Step 6.  
   **Expected Outputs**: AppointmentsController, AppointmentService, Appointment model, Blazor pages (Create, Index).  
   **Verification Method**: Schedule appointment, update status, view daily list.  
   **Requirement Reference(s)**: Functional Requirements - Appointment Management, Appointment Tracking.

8. **Step Name**: Enhance patient search  
   **Objective**: Add partial matching, case-insensitive search with results ordered by recent.  
   **Inputs**: Patient management from Step 6.  
   **Expected Outputs**: Updated PatientService with search logic.  
   **Verification Method**: Search partial name, verify case-insensitive and ordering.  
   **Requirement Reference(s)**: Approved Assumption - Search behavior (partial, case-insensitive, ordered by recent).

9. **Step Name**: Implement consultation creation  
   **Objective**: Capture vitals, complaints, diagnosis in consultation workflow.  
   **Inputs**: Appointments from Step 7.  
   **Expected Outputs**: ConsultationsController, ConsultationService, Consultation model, Create.razor page.  
   **Verification Method**: Complete consultation form, verify data saved in DB.  
   **Requirement Reference(s)**: Functional Requirements - Consultation Workflow (Vitals, Complaints, Diagnosis).

10. **Step Name**: Add prescription generation  
    **Objective**: Generate prescriptions with medications and printable layout.  
    **Inputs**: Consultation from Step 9.  
    **Expected Outputs**: PrescriptionService, Prescription model, View.razor for printable prescription.  
    **Verification Method**: Generate prescription, verify mandatory fields and layout.  
    **Requirement Reference(s)**: Functional Requirements - Medication / Prescription; Approved Assumption - Prescription fields/layout.

11. **Step Name**: Persist consultations with transactions  
    **Objective**: Ensure ACID compliance for consultation and prescription saves.  
    **Inputs**: Consultation and prescription from Steps 9-10.  
    **Expected Outputs**: Transaction logic in ConsultationService.  
    **Verification Method**: Save consultation; verify all data persists or rolls back on failure.  
    **Requirement Reference(s)**: Reliability (no data loss); Database transactions for consultations.

12. **Step Name**: Implement patient history  
    **Objective**: View past visits with date filtering and details.  
    **Inputs**: Consultations from Step 9.  
    **Expected Outputs**: History.razor page, updated ConsultationService for filtering.  
    **Verification Method**: View history, filter by date, display vitals/diagnosis/prescriptions.  
    **Requirement Reference(s)**: Functional Requirements - Patient History.

13. **Step Name**: Add data export  
    **Objective**: Export patient/visit data to Excel/PDF with specified fields.  
    **Inputs**: Patient history from Step 12.  
    **Expected Outputs**: ExportService, Index.razor for export options.  
    **Verification Method**: Export file, verify fields (DD-MM-YYYY dates), open in Excel/PDF.  
    **Requirement Reference(s)**: Functional Requirements - Data Export; Approved Assumption - Export formats/fields.

14. **Step Name**: Write unit tests  
    **Objective**: Achieve >80% coverage on services and business logic.  
    **Inputs**: All implemented services from Steps 6-13.  
    **Expected Outputs**: xUnit test files for services, mocks with Moq.  
    **Verification Method**: Run tests, verify coverage report >80%.  
    **Requirement Reference(s)**: Testing Strategy (unit testing services).

15. **Step Name**: Implement integration tests  
    **Objective**: Test API-to-DB flows with test containers.  
    **Inputs**: Controllers and services.  
    **Expected Outputs**: Integration tests using EF Test Containers.  
    **Verification Method**: Run tests, verify CRUD cycles and authentication.  
    **Requirement Reference(s)**: Testing Strategy (integration testing API/DB).

16. **Step Name**: Perform UAT  
    **Objective**: Validate core workflows manually.  
    **Inputs**: Complete application.  
    **Expected Outputs**: UAT test results.  
    **Verification Method**: Measure training time <30 minutes, validate workflows.  
    **Requirement Reference(s)**: Approved Assumption - Usability (30-minute training).

17. **Step Name**: Set up DevOps  
    **Objective**: Configure CI/CD for deployment.  
    **Inputs**: Complete codebase.  
    **Expected Outputs**: Azure Pipelines YAML, deployment scripts.  
    **Verification Method**: Trigger pipeline, deploy to staging.  
    **Requirement Reference(s)**: Deployment & Monitoring.

18. **Step Name**: Validate production readiness  
    **Objective**: Test backups, recovery, and performance.  
    **Inputs**: Deployed app.  
    **Expected Outputs**: Backup verification, performance test results.  
    **Verification Method**: Restore backup, confirm RPO/RTO, load test meets targets.  
    **Requirement Reference(s)**: Reliability (backups, RTO 4h); Performance (load testing).

## 5. Testing Strategy

Unit Testing: Cover services with xUnit and Moq for >80% coverage, testing business rules like search and validation.  
Integration Testing: Use EF Test Containers for API-DB tests, validating end-to-end flows like patient CRUD and consultations.  
User Acceptance Testing: Manual testing of workflows (registration, appointment, prescription), measuring usability against 30-minute training.  
Performance Testing: Load test for 40 patients/hour, 25 users, ensuring <2s loads and <1s search.  
Security Testing: Verify JWT auth, HTTPS encryption, SQL TDE, audit logs.

## 6. Risk Areas During Implementation

- Performance degradation under load if indexes are not optimized (mitigate with DB tuning).
- Data loss if transactions fail (mitigate with ACID and backups).
- Usability issues if UI is not minimal (mitigate with physician feedback).
- Security breaches if auth is misconfigured (mitigate with JWT validation).
- Export failures if libraries don't handle formats correctly (mitigate with testing).

## 7. Open Questions & Blocking Items

None. All ambiguities resolved in brainstorming analysis.

## 8. Explicit Planning Assumptions (If Any)

- Moderate patient volume: 300 patients/day, 40/hour peak, 25 concurrent users (justified by approved assumption).
- Search behavior: Partial, case-insensitive, ordered by recent (approved).
- Prescription fields/layout: Mandatory fields as specified, standardized (approved).
- Export: Excel/PDF with specified fields, DD-MM-YYYY (approved).
- Backups: Daily automated, 30-day retention, RPO 24h, RTO 4h (approved).