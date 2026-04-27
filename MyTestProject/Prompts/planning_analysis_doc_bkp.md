# Clinical Patient Management System - Implementation Planning Document

**Document Version**: 1.1  
**Date**: April 7, 2026  
**Status**: Ready for Development  
**Technology Stack**: Blazor, .NET 8 Web API, SQL Server

---

## Executive Summary

This planning document converts the approved Brainstorming Analysis into a detailed implementation plan. It uses the final, resolved functional and non-functional requirements, locked technology decisions, and clarified assumptions to define architecture, component scope, implementation phases, testing strategy, and the definition of done. The result is a development-ready plan aligned to the approved design and ready for execution.

---

## 1. Technology Stack Summary

### Fixed Technology Selections (Locked – No Deviations)

| Layer | Technology | Version | Justification |
|-------|-----------|---------|---------------|
| **Frontend UI** | Blazor WebAssembly + ASP.NET Core hosted | .NET 8 | Browser-based delivery, responsive UI, shared C# model code, optimized for clinic workflows with modern web browser compatibility. |
| **Backend API** | ASP.NET Core Web API | .NET 8 | Secure REST API with built-in DI, scalable async processing, and seamless EF Core integration. |
| **Database** | SQL Server | 2019 or later / managed cloud equivalent | ACID compliance, encryption at rest, reliable backup/restore, and support for expected clinic workload. |
| **Authentication** | ASP.NET Identity + JWT | .NET built-in | Single physician user model, token-based API security, and simple session management. |
| **ORM** | Entity Framework Core | 8.0 | Migrations, LINQ-based querying, test container support, and maintainability for data access. |
| **Logging** | Serilog | Latest stable | Structured logging, multiple sinks, and diagnostics for application and data access events. |
| **Testing Framework** | xUnit | Latest stable | Standard unit/integration testing framework for .NET with strong ecosystem support. |
| **Mocking** | Moq | Latest stable | Flexible mocking for service and controller tests, especially async behaviors. |
| **Integration Testing** | EF Core Test Containers | Latest stable | Reliable SQL Server tests with isolated containers and reproducible seed data. |
| **Export Libraries** | EPPlus, QuestPDF | Latest stable | Export to Excel and PDF with consistent formatting and professional presentation. |
| **API Documentation** | Swagger/OpenAPI | Built-in | Self-documenting API, developer usability, and API validation. |

### Supporting Libraries

- **AutoMapper**: DTO mapping between persistence and API layers
- **FluentValidation**: Business rule validation in service and API layers
- **MediatR** (optional): Clear command/query separation for complex workflows
- **Hangfire** (optional): Scheduled checks for backup verification or health monitoring

### Compatibility Assurance

- Compatible with Chrome, Edge, and Safari modern versions
- Uses HTTPS/TLS 1.3 for transport security
- Applies SQL Server TDE and encryption-at-rest best practices
- Ensures PDF/Excel exports open correctly in standard office tools

---

## 2. High-Level Architecture

### System Architecture Overview

┌─────────────────────────────────────────────────────────────┐
│ Physician Browser │
│ (Chrome, Edge, Safari) │
└────────────────────┬────────────────────────────────────────┘
│ HTTPS/TLS 1.3
▼
┌─────────────────────────────────────────────────────────────┐
│ Blazor WebAssembly Client │
│ • Pages: Login, PatientManagement, AppointmentList, ConsultationWorkflow, PrescriptionGeneration, PatientHistory, Export │
│ • Components: SearchBar, PatientCard, AppointmentGrid, ConsultationForm, PrescriptionPreview, ExportPanel │
│ • Services: AuthService, PatientService, AppointmentService, ConsultationService, PrescriptionService, ExportService │
└────────────────────┬────────────────────────────────────────┘
│ REST JSON API
▼
┌─────────────────────────────────────────────────────────────┐
│ ASP.NET Core Web API │
│ • Controllers: AuthController, PatientsController, AppointmentsController, ConsultationsController, PrescriptionsController, ExportController │
│ • Services: PatientService, AppointmentService, ConsultationService, PrescriptionService, ExportService, AuditService, BackupVerificationService │
│ • Repositories: PatientRepository, AppointmentRepository, ConsultationRepository, PrescriptionRepository │
│ • Middleware: Authentication, Authorization, ExceptionHandling, RequestLogging │
└────────────────────┬────────────────────────────────────────┘
│ EF Core / SQL Server
▼
┌─────────────────────────────────────────────────────────────┐
│ SQL Server Database │
│ • Tables: Users, Patients, Appointments, Consultations, Vitals, Prescriptions, PrescriptionMedicines, AuditLog │
│ • Constraints: FK integrity, indexes on search keys, date filters, and patient IDs │
│ • Backup: Daily automated backups, 30-day retention, RPO 24h / RTO 4h │
└─────────────────────────────────────────────────────────────┘

### Key Components

- **Authentication**: Single physician account with JWT-based API access.
- **Patient Management**: Create, update, view patient records with contact details and demographics.
- **Appointment Scheduling**: Manage appointment creation, status tracking, and daily lists.
- **Consultation Workflow**: Capture vitals, complaints, diagnosis, medications, and generate consultation records.
- **Prescription Generation**: Produce printable prescriptions with standardized layout and required fields.
- **Patient History**: View historical consultations and exportable visit summaries.
- **Export**: Generate secure Excel (.xlsx) and PDF outputs with prescribed formatting.
- **Logging & Auditing**: Track system events, errors, and critical operations with Serilog.

---

## 3. Requirements Mapping and Implementation Scope

### Functional Scope

| Requirement | Implementation Target |
|-------------|-----------------------|
| Patient registration and profile management | Blazor forms + API endpoints for add/edit/view patient profiles |
| Patient search | Case-insensitive partial name search, exact ID search, display duplicates separately |
| Appointment scheduling | Appointment create/update endpoints, daily schedule view, status update workflow |
| Appointment tracking | Appointment status flow: Scheduled → Completed / Cancelled / No-show |
| Vitals capture | Consultation record fields for temperature, blood pressure, pulse |
| Complaints recording | Free-text complaint entry stored in consultation record |
| Diagnosis documentation | Diagnosis notes field in consultation record |
| Medication management | Prescription medicine line items, dosage/frequency/duration/instructions |
| Prescription generation | Printable single-page prescription layout with required header/footer fields |
| Patient history tracking | Historical visit list, filtered by date, with consultation details |
| Data export | Excel and PDF exports of patient/prescription data with DD-MM-YYYY dates |

### Non-Functional Scope

- Usability: Minimal-entry forms, focused workflow pages, quick search access
- Performance: Page load under 2 seconds for primary views, search response under 1 second for expected dataset
- Reliability: Daily backups, transactional save operations, no data loss requirement
- Security: Single-user login, encrypted transport, secure storage
- Scalability: Designed for up to 300 patients/day and 40 patients/hour peak
- Compatibility: Browser support for Chrome, Edge, Safari

---

## 4. Design Decisions and Clarified Assumptions

### Approved Design Decisions

- Moderate patient volume is defined as up to 300 patients per day, peak load of 40 patients per hour, and up to 25 concurrent active users.
- Patient search supports case-insensitive partial name matching, exact matching for IDs, and duplicate names show separate records using unique IDs.
- Printable prescriptions include Patient Name/ID, Doctor Name/Registration Number, Prescription Date, Medication Name, Dosage, Frequency, Duration, and optional Notes.
- Export formats are Excel (.xlsx) and PDF, with data fields including Patient ID, Patient Name, Prescription Number, Prescription Date, Medication Details, and Doctor Name.
- CSV export is not required in Phase 1; Excel and PDF are sufficient.
- Backups are automated daily with 30-day retention, RPO 24 hours, RTO 4 hours.
- Usability success is measured by a new user completing core tasks within 30 minutes of basic guidance.
- Paper reduction is measured by at least 80% of prescriptions being generated digitally.
- Successful export means files generate without error and open correctly in standard applications.

### Key System Constraints

- Single physician user model only; no multi-user permissions or role-based access in Phase 1.
- Single clinic scope; no multi-clinic or multi-tenant architecture required.
- All major functionality must remain browser-based.
- No customization of prescription layout in Phase 1.

---

## 5. Implementation Phases

### Phase 1: Core Clinic Workflow

Deliverables:
- User authentication and physician login
- Patient registration, edit, and profile view
- Appointment scheduling and status tracking
- Consultation workflow with vitals, complaints, diagnosis, and medication
- Prescription generation and print preview
- Basic patient history view
- Export to PDF and Excel
- Logging and error handling

Acceptance Criteria:
- Users can register/search patients and schedule appointments
- Consultations capture vitals, diagnosis, and medications reliably
- Prescriptions print correctly with required fields
- Patient history exposes past consultations and visit data
- Export files open in standard applications without errors

### Phase 2: Reliability, Reporting, and QA

Deliverables:
- Automated daily backups and restore verification
- Improved search filtering and result ordering
- Comprehensive unit and integration tests
- Performance tuning for typical clinic load
- User validation of usability and form flow

Acceptance Criteria:
- Backup process validates daily and retention rules
- Tests cover critical business workflows and edge cases
- Search performance meets expectations for up to 300 daily patients
- UI is validated for core tasks within 30 minutes for a new user

### Phase 3: Stabilization and Handoff

Deliverables:
- Final bug fixes and polished UI details
- Deployment readiness documentation
- Training notes for physician onboarding
- Post-release monitoring and logging configuration

Acceptance Criteria:
- No critical open defects in core workflow
- Production deployment checklist complete
- Physician sign-off on usability and export behavior

---

## 6. Testing Strategy

### Unit Testing

- Validate service logic for patient CRUD, appointment state transitions, consultation creation, and prescription generation.
- Test search filtering, partial matching, case-insensitive lookup, and duplicate record handling.
- Cover business rules for required fields, date formatting, and export payload composition.

### Integration Testing

- Use EF Core Test Containers with SQL Server for API-level tests.
- Validate end-to-end flows: register patient → schedule appointment → complete consultation → generate prescription → export file.
- Verify authentication, authorization, and request pipeline behavior.

### Acceptance Testing

- Confirm key workflows with the physician persona: patient registration, appointment scheduling, consultation workflow, history lookup, and exports.
- Validate printable prescription layout and content.
- Test browser compatibility across Chrome, Edge, and Safari.

### Performance Validation

- Measure page load and search latency against target thresholds.
- Confirm system supports 300 patients/day and 40 patients/hour peaks without functional degradation.

---

## 7. Risks and Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Undefined patient volume | Performance issues | Use approved 300/day, 40/hour limits and tune search/indexes accordingly |
| Prescription formatting ambiguity | Incorrect output | Use standardized single-page template and lock fields for Phase 1 |
| Backup strategy gaps | Data loss | Apply daily automated backups with 30-day retention and verification checks |
| Vague usability criteria | Misaligned UX | Measure against 30-minute core-task onboarding and iterate with physician feedback |
| Export failures | Lost data / user frustration | Validate PDF/Excel generation in tests and QA across standard viewers |

---

## 8. Definition of Done

- All functional requirements from the approved Brainstorming Analysis are implemented.
- Approved assumptions are reflected in design and development.
- Core workflows pass unit, integration, and acceptance tests.
- Prescription generation and export features work reliably.
- Daily backup strategy is implemented and validated.
- Browser compatibility for Chrome, Edge, and Safari is confirmed.
- Documentation and deployment readiness are complete.
- No critical defects remain and the solution is ready for production handoff.
