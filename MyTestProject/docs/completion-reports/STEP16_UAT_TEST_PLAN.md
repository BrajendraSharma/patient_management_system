# Step 16: User Acceptance Testing (UAT) - Test Plan

**Date**: May 12, 2026  
**Phase**: Phase 6 - Testing & Quality Assurance  
**Objective**: Validate core workflows manually and measure usability against 30-minute training requirement  
**Inputs**: Complete application from Steps 1-15  
**Expected Outputs**: UAT test results, workflow validation reports  
**Verification Method**: Manual testing of core workflows, training time measurement <30 minutes  
**Requirement Reference**: Approved Assumption - Usability (30-minute training)

## 1. UAT Overview

User Acceptance Testing (UAT) for the Clinical Patient Management System focuses on:
- **Workflow Validation**: Ensuring all core workflows function as designed
- **Usability Assessment**: Measuring training time and ease of use
- **End-to-End Scenarios**: Testing complete patient journeys
- **Data Integrity**: Verifying data persists correctly through workflows
- **User Experience**: Confirming the system meets physician expectations

## 2. UAT Scope

### In Scope
- Patient registration and management workflows
- Appointment scheduling and status tracking
- Consultation capture with vitals validation
- Prescription generation and printing
- Patient history viewing and filtering
- Data export to Excel/PDF
- Authentication and authorization
- UI responsiveness and navigation
- Error handling and validation messages

### Out of Scope
- Performance load testing (Step 18)
- DevOps/CI-CD configuration (Step 17)
- Security penetration testing
- Database optimization
- Code-level unit testing (Step 14)
- Integration testing (Step 15)

## 3. UAT Test Scenarios

### 3.1 Authentication Workflow

**Test Case 1.1: User Login**
- **Precondition**: Application running, user not authenticated
- **Steps**:
  1. Navigate to application login page
  2. Enter valid physician credentials
  3. Click "Login" button
- **Expected Result**: 
  - User logged in successfully
  - Redirected to dashboard/home page
  - Authentication token stored in localStorage
  - Navigation menu displays authenticated user
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 1.2: Protected Route Access (Unauthorized)**
- **Precondition**: User not authenticated
- **Steps**:
  1. Try to navigate directly to `/patients` without login
- **Expected Result**:
  - Redirected to login page
  - Message: "Please log in to continue"
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 1.3: User Logout**
- **Precondition**: User authenticated and viewing dashboard
- **Steps**:
  1. Click "Logout" button in navigation menu
- **Expected Result**:
  - User logged out successfully
  - Redirected to login page
  - localStorage cleared (JWT token removed)
  - Attempting to navigate to protected routes redirects to login
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

### 3.2 Patient Management Workflow

**Test Case 2.1: Register New Patient**
- **Precondition**: User logged in, viewing Patients page
- **Steps**:
  1. Click "Register New Patient" button
  2. Fill in patient details:
     - Full Name: "John Doe"
     - Age: 45
     - Gender: Male
     - Email: john.doe@example.com
     - Phone: 9876543210
     - Address: "123 Main St, City"
     - Medical History: "Hypertension"
  3. Click "Register" button
- **Expected Result**:
  - Patient registered successfully
  - Confirmation message displayed: "Patient registered successfully"
  - Redirected to patient list
  - New patient appears in the list
  - Data persists in database
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 2.2: Form Validation - Empty Fields**
- **Precondition**: User on patient registration form
- **Steps**:
  1. Leave required fields empty (Full Name, Age, Email, Phone)
  2. Click "Register" button
- **Expected Result**:
  - Form validation errors displayed
  - Error messages for each empty required field
  - Form not submitted
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 2.3: Form Validation - Invalid Email**
- **Precondition**: User on patient registration form
- **Steps**:
  1. Fill all fields with valid data
  2. Enter invalid email: "invalid-email"
  3. Click "Register" button
- **Expected Result**:
  - Validation error: "Invalid email format"
  - Form not submitted
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 2.4: Edit Existing Patient**
- **Precondition**: Patient exists in system
- **Steps**:
  1. Navigate to patient list
  2. Click "Edit" button for a patient
  3. Modify patient information (e.g., phone number)
  4. Click "Save" button
- **Expected Result**:
  - Patient updated successfully
  - Confirmation message displayed
  - Changes persisted in database
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 2.5: Search Patient by Name**
- **Precondition**: Multiple patients registered, user on patient list
- **Steps**:
  1. Click search box
  2. Enter partial name: "John"
  3. Observe results
- **Expected Result**:
  - Case-insensitive search results
  - Patients containing "john" or "John" displayed
  - Results ordered by most recent registration
  - Search responds within 1 second
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 2.6: Search Patient by Phone**
- **Precondition**: Multiple patients registered, user on patient list
- **Steps**:
  1. Click search box
  2. Enter partial phone: "987"
  3. Observe results
- **Expected Result**:
  - Patients with phone containing "987" displayed
  - Results accurate and timely
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

### 3.3 Appointment Scheduling Workflow

**Test Case 3.1: Schedule New Appointment**
- **Precondition**: Patient exists, user on Appointments page
- **Steps**:
  1. Click "Schedule Appointment" button
  2. Select patient from dropdown
  3. Select appointment date: "Tomorrow at 2:00 PM"
  4. Click "Schedule" button
- **Expected Result**:
  - Appointment scheduled successfully
  - Confirmation message displayed
  - Appointment appears in appointment list
  - Data persisted in database
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 3.2: Appointment Validation - Past Date**
- **Precondition**: User on appointment creation form
- **Steps**:
  1. Select patient
  2. Try to set appointment date to a past date
  3. Click "Schedule" button
- **Expected Result**:
  - Validation error: "Appointment date cannot be in the past"
  - Form not submitted
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 3.3: Update Appointment Status**
- **Precondition**: Appointment exists in scheduled status
- **Steps**:
  1. Navigate to appointment details
  2. Click "Status" dropdown
  3. Change status to "Completed"
  4. Click "Save" button
- **Expected Result**:
  - Status updated successfully
  - Confirmation message displayed
  - Status change persisted in database
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 3.4: Appointment Conflict Detection**
- **Precondition**: Appointment already scheduled for tomorrow 2:00 PM
- **Steps**:
  1. Try to schedule another appointment for same patient at same time
  2. Click "Schedule" button
- **Expected Result**:
  - Validation error: "Patient already has appointment at this time"
  - Form not submitted (double-booking prevented)
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

### 3.4 Consultation Workflow

**Test Case 4.1: Complete Consultation - Vitals Capture**
- **Precondition**: Appointment scheduled and completed status set
- **Steps**:
  1. Navigate to consultation creation
  2. Select the appointment
  3. Fill vitals:
     - Temperature: 37.5°C
     - Blood Pressure: 120/80
     - Pulse: 72 bpm
  4. Fill complaints: "Headache for 2 days"
  5. Fill diagnosis: "Tension headache"
  6. Click "Save" button
- **Expected Result**:
  - Consultation saved successfully
  - Confirmation message displayed
  - All vitals and details persisted
  - Transaction completed atomically
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 4.2: Vitals Validation - Temperature Out of Range**
- **Precondition**: User on consultation creation form
- **Steps**:
  1. Enter temperature: 50°C (out of valid range 30-45°C)
  2. Try to submit form
- **Expected Result**:
  - Validation error: "Temperature must be between 30°C and 45°C"
  - Form not submitted
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 4.3: Vitals Validation - Blood Pressure Format**
- **Precondition**: User on consultation creation form
- **Steps**:
  1. Enter blood pressure in invalid format: "120-80"
  2. Try to submit form
- **Expected Result**:
  - Validation error: "Blood Pressure format must be XX/XX (e.g., 120/80)"
  - Form not submitted
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 4.4: Mandatory Fields Validation**
- **Precondition**: User on consultation creation form
- **Steps**:
  1. Leave temperature field empty
  2. Try to submit form
- **Expected Result**:
  - Validation error: "Temperature is required"
  - Form not submitted
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

### 3.5 Prescription Workflow

**Test Case 5.1: Generate Prescription**
- **Precondition**: Consultation with vitals and diagnosis completed
- **Steps**:
  1. Navigate to consultation details
  2. Click "Generate Prescription" button
  3. Add medications:
     - Medication 1: Name "Paracetamol", Dosage "500mg", Frequency "Twice daily", Duration "5 days"
     - Medication 2: Name "Ibuprofen", Dosage "400mg", Frequency "Once daily", Duration "3 days"
  4. Click "Save" button
- **Expected Result**:
  - Prescription generated successfully
  - Confirmation message displayed
  - Prescription details persisted in database
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 5.2: Prescription View and Display**
- **Precondition**: Prescription generated
- **Steps**:
  1. Navigate to prescription view
  2. Verify all sections displayed:
     - Clinic/doctor header
     - Patient information
     - Consultation vitals
     - Diagnosis
     - Medications table
     - Signature area
- **Expected Result**:
  - All prescription sections displayed correctly
  - Layout is clean and professional
  - Data is accurate
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 5.3: Print Prescription**
- **Precondition**: Prescription view open
- **Steps**:
  1. Click "Print" button
  2. Verify browser print preview
  3. Send to PDF printer or print to paper
- **Expected Result**:
  - Print dialog appears
  - Print preview shows clean, professional layout
  - Headers and footers formatted correctly
  - All data visible and readable
  - PDF file generated with good quality
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

### 3.6 Patient History Workflow

**Test Case 6.1: View Patient History**
- **Precondition**: Patient has multiple past consultations
- **Steps**:
  1. Navigate to patient details
  2. Click "View History" button
  3. Observe consultation list
- **Expected Result**:
  - All past consultations displayed
  - List ordered by most recent first
  - Each entry shows date, vitals summary, diagnosis
  - All data accurate
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 6.2: Filter History by Date Range**
- **Precondition**: Patient history view open with multiple consultations
- **Steps**:
  1. Set date filter: Start date "1 month ago", End date "today"
  2. Click "Filter" button
- **Expected Result**:
  - Consultations filtered to specified date range
  - Only consultations within range displayed
  - Count accurate
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 6.3: View Consultation Details from History**
- **Precondition**: Patient history view open
- **Steps**:
  1. Click on a consultation entry
  2. Verify all details displayed:
     - Vitals (Temperature, BP, Pulse)
     - Complaints
     - Diagnosis
     - Associated prescription (if any)
- **Expected Result**:
  - Consultation details displayed completely
  - All data accurate and readable
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

### 3.7 Data Export Workflow

**Test Case 7.1: Export Patient Data to Excel**
- **Precondition**: Multiple patients registered, user on Export page
- **Steps**:
  1. Select "Patient Data" from datatype dropdown
  2. Select "Excel" from format dropdown
  3. Click "Export" button
  4. Save the downloaded file
  5. Open file in Excel
- **Expected Result**:
  - File downloads successfully
  - File opens in Excel without errors
  - All patient data columns present
  - Dates formatted as DD-MM-YYYY
  - Data accurate and complete
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 7.2: Export Visit History with Date Filtering**
- **Precondition**: Multiple consultations exist, user on Export page
- **Steps**:
  1. Select "Visit History" from datatype dropdown
  2. Enter Patient ID or select from dropdown
  3. Set date range: last 30 days
  4. Select "Excel" format
  5. Click "Export" button
  6. Verify file content
- **Expected Result**:
  - File downloads successfully
  - Consultations filtered to specified date range
  - All vitals and diagnosis included
  - Dates formatted as DD-MM-YYYY
  - Filename includes timestamp
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 7.3: Export Prescription Data to PDF**
- **Precondition**: Prescriptions exist, user on Export page
- **Steps**:
  1. Select "Prescriptions" from datatype dropdown
  2. Select "PDF" format
  3. Click "Export" button
  4. Open downloaded PDF file
- **Expected Result**:
  - PDF downloads successfully
  - PDF opens without errors
  - All prescription data included
  - Headers and formatting correct
  - Dates formatted as DD-MM-YYYY
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 7.4: Export Data to PDF (Text-Based)**
- **Precondition**: Patient data or visit history ready to export
- **Steps**:
  1. Select datatype and "PDF" format
  2. Click "Export" button
  3. Open PDF file
- **Expected Result**:
  - PDF generated successfully (text-based, not image)
  - Data readable and properly formatted
  - Headers included with timestamp
  - Dates in DD-MM-YYYY format
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

### 3.8 UI/UX and Navigation Workflow

**Test Case 8.1: Navigation Menu Accessibility**
- **Precondition**: User logged in
- **Steps**:
  1. Verify all main menu items visible and clickable:
     - Home/Dashboard
     - Patients
     - Appointments
     - History
     - Export
     - Logout
  2. Click each menu item
  3. Verify correct page loads
- **Expected Result**:
  - All navigation items accessible
  - Navigation responsive and clear
  - Correct pages load on click
  - No broken links
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 8.2: Responsive Design - Mobile View (375px)**
- **Precondition**: Any page of application
- **Steps**:
  1. Resize browser to 375px width (mobile view)
  2. Navigate through all pages
  3. Verify:
     - Navigation menu collapses to hamburger menu
     - Forms are readable on mobile screen
     - Buttons are clickable
     - No horizontal scrolling needed
     - Tables have horizontal scroll if needed
- **Expected Result**:
  - All pages responsive on mobile
  - Layout adapts correctly
  - Content readable without zoom
  - User can complete workflows on mobile
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 8.3: Responsive Design - Tablet View (768px)**
- **Precondition**: Any page of application
- **Steps**:
  1. Resize browser to 768px width (tablet view)
  2. Navigate through pages
  3. Verify proper layout adaptation
- **Expected Result**:
  - Pages render correctly on tablet size
  - Two-column layouts appear where applicable
  - All content accessible without excessive scrolling
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 8.4: Form Validation Messages Display**
- **Precondition**: User on any form (registration, appointment, consultation)
- **Steps**:
  1. Try to submit empty form
  2. Observe validation messages
- **Expected Result**:
  - Clear, specific error messages displayed
  - Error messages highlighted in red
  - Error messages for each field displayed near that field
  - ValidationSummary shows all errors
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 8.5: Loading Indicators**
- **Precondition**: Performing async operations (search, export)
- **Steps**:
  1. Trigger long-running operation (e.g., export)
  2. Observe UI during operation
- **Expected Result**:
  - Loading spinner displays while operation in progress
  - UI indicates processing status
  - Button disabled during operation (no duplicate submissions)
  - Spinner removed when operation completes
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

**Test Case 8.6: Success and Error Messages**
- **Precondition**: Performing operations that generate feedback messages
- **Steps**:
  1. Complete a successful operation (e.g., save patient)
  2. Observe success message
  3. Perform invalid operation (e.g., invalid form submission)
  4. Observe error message
- **Expected Result**:
  - Success messages display with green alert styling
  - Error messages display with red alert styling
  - Messages auto-dismiss after 5 seconds
  - User can manually close message
- **Status**: [ ] PASS [ ] FAIL
- **Notes**: _____________________

## 4. Training Time Measurement

### Objective
Measure how long it takes an average physician user to complete basic workflows without any prior system experience.

### Training Procedure

**Prerequisite**: Physician user (or proxy) unfamiliar with the system, but familiar with general computer usage.

**Training Timeline**:

**Minute 0-5: System Overview & Login**
- Demonstrate login screen
- Show navigation menu
- Explain main workflow: Patient → Appointment → Consultation → Prescription
- Have trainee log in

**Minute 5-10: Patient Management**
- Demonstrate registering a new patient
- Have trainee register a new patient
- Show patient search and edit functionality

**Minute 10-15: Appointment Scheduling**
- Demonstrate scheduling an appointment
- Show status updates
- Have trainee schedule an appointment

**Minute 15-20: Consultation Entry**
- Demonstrate consultation form
- Explain vitals capture and validation
- Show how to handle validation errors
- Have trainee enter a consultation

**Minute 20-25: Prescription Generation**
- Demonstrate prescription generation
- Show print functionality
- Have trainee generate and preview a prescription

**Minute 25-30: History and Export**
- Demonstrate patient history viewing
- Show date filtering
- Demonstrate data export
- Show Excel and PDF options

**Minute 30: Independent Tasks**
- Ask trainee to complete a full workflow independently:
  1. Register a new patient
  2. Schedule an appointment
  3. Enter a consultation
  4. Generate prescription
  5. Export data

### Training Success Criteria
- [ ] Trainee completes all tasks within 30 minutes of instruction
- [ ] Trainee can navigate without assistance after training
- [ ] Trainee understands validation error messages
- [ ] Trainee successfully completes independent workflow task
- [ ] Trainee confirms the system is easy to use

### Training Result
- **Training Time (minutes)**: _______
- **Training Completed**: [ ] YES [ ] NO
- **Trainee Feedback**: _________________________________
- **Trainer Notes**: _________________________________

## 5. UAT Execution Summary

### Test Execution Schedule

**Day 1**: Authentication and Patient Management (Test Cases 1.1-2.6)
**Day 2**: Appointment Scheduling (Test Cases 3.1-3.4)
**Day 3**: Consultation Workflow (Test Cases 4.1-4.4)
**Day 4**: Prescription and History (Test Cases 5.1-6.3)
**Day 5**: Data Export and UI/UX (Test Cases 7.1-8.6)
**Day 6**: Training Time Measurement and Final Validation

### Overall UAT Status

| Category | Total Tests | Passed | Failed | Pass Rate |
|----------|-------------|--------|--------|-----------|
| Authentication | 3 | ___ | ___ | __% |
| Patient Management | 6 | ___ | ___ | __% |
| Appointments | 4 | ___ | ___ | __% |
| Consultations | 4 | ___ | ___ | __% |
| Prescriptions | 3 | ___ | ___ | __% |
| Patient History | 3 | ___ | ___ | __% |
| Data Export | 4 | ___ | ___ | __% |
| UI/UX Navigation | 6 | ___ | ___ | __% |
| **TOTAL** | **33** | **___** | **___** | **___%** |

### Acceptance Criteria
- ✓ All core workflows (33 test cases) pass with 100% success rate
- ✓ Training time <30 minutes (measured and documented)
- ✓ No critical issues or blockers identified
- ✓ Physician confirms system meets requirements
- ✓ Usability assessment: "System is easy to learn and use"

## 6. UAT Exit Criteria

### Must Have (Blocking Issues)
- [ ] Authentication works reliably
- [ ] Patient data persists correctly
- [ ] Appointment scheduling prevents double-booking
- [ ] All form validations work correctly
- [ ] Consultation data saves with no data loss
- [ ] Prescriptions generate and print correctly
- [ ] Search functionality works
- [ ] Training time <30 minutes

### Should Have (High Priority)
- [ ] All responsive design tests pass
- [ ] Export functionality works for all formats
- [ ] Patient history filtering accurate
- [ ] Navigation intuitive and responsive
- [ ] Error messages clear and helpful

### Nice to Have (Lower Priority)
- [ ] Print preview looks professional
- [ ] Loading indicators smooth
- [ ] Auto-dismiss messages work

## 7. Issues and Defects Found

Use this section to document any issues discovered during UAT:

| Issue # | Severity | Component | Description | Status | Resolution |
|---------|----------|-----------|-------------|--------|-----------|
| 1 | ___ | ___ | ___ | ___ | ___ |
| 2 | ___ | ___ | ___ | ___ | ___ |
| 3 | ___ | ___ | ___ | ___ | ___ |

## 8. UAT Sign-Off

**UAT Execution By**: _________________________ (Name)  
**Date**: _________________________  
**Testing Duration**: _________________________ (hours)  
**Overall Result**: [ ] PASS [ ] FAIL  
**Recommended for Production**: [ ] YES [ ] NO  

**Physician/User Sign-Off**: _________________________ (Signature)  
**Date**: _________________________  

**Project Manager Sign-Off**: _________________________ (Signature)  
**Date**: _________________________  

## 9. Next Steps

Upon completion of Step 16 UAT with passing results:
- **Step 17**: Set up DevOps (CI/CD pipelines)
- **Step 18**: Validate production readiness (backups, performance)
- **Production Deployment**: After all steps complete and approved

---

**Document Version**: 1.0  
**Last Updated**: May 12, 2026  
**Status**: Ready for Execution
