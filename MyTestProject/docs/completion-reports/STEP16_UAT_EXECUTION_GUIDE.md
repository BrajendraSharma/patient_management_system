# Step 16: UAT Execution Guide

**Date**: May 12, 2026  
**Purpose**: Detailed step-by-step guide for executing the UAT test plan  
**Audience**: QA/Testing team, Physician user  
**Duration**: 6 days (1-2 hours per day) + final comprehensive testing

## 1. Pre-UAT Checklist

Before starting UAT, ensure the following prerequisites are met:

### Environment Setup
- [ ] Application deployed to test/staging environment (or local dev environment)
- [ ] SQL Server database initialized with test data
- [ ] Sample patients created (at least 10 patients with various data)
- [ ] Sample appointments created
- [ ] HTTPS working (if in production environment)
- [ ] Logging enabled and accessible
- [ ] Email notifications configured (if applicable)

### Test Data Preparation
```
Sample Patients to Create:
1. John Doe - Age 45, Male, john.doe@email.com, 9876543210
2. Jane Smith - Age 32, Female, jane.smith@email.com, 9876543211
3. Michael Johnson - Age 58, Male, michael.j@email.com, 9876543212
4. Sarah Williams - Age 28, Female, sarah.w@email.com, 9876543213
5. David Brown - Age 42, Male, david.brown@email.com, 9876543214
6. Emily Davis - Age 35, Female, emily.davis@email.com, 9876543215
7. Robert Miller - Age 50, Male, robert.m@email.com, 9876543216
8. Lisa Anderson - Age 41, Female, lisa.a@email.com, 9876543217
9. James Taylor - Age 55, Male, james.t@email.com, 9876543218
10. Jennifer White - Age 38, Female, jennifer.w@email.com, 9876543219
```

### Credentials
- [ ] Test physician account credentials available
- [ ] Test user confirmed to work
- [ ] Password reset functionality tested (if applicable)

### Equipment
- [ ] Desktop/Laptop with modern browser (Chrome 120+, Firefox 120+, Edge 120+)
- [ ] Mobile device or browser with mobile viewport for responsive testing
- [ ] Tablet for intermediate viewport testing
- [ ] Printer or PDF printer driver configured
- [ ] Excel installed for export validation

### Documentation
- [ ] UAT Test Plan document (STEP16_UAT_TEST_PLAN.md) printed or available
- [ ] Test result tracking spreadsheet open
- [ ] Test execution log template ready
- [ ] Camera or screenshot tool for defect documentation

## 2. Day 1: Authentication and Patient Management (Test Cases 1.1-2.6)

### Duration: 1.5-2 hours

### Setup
1. Open application in browser
2. Ensure you're at the login page
3. Have credentials ready

### Test Execution

#### Test Case 1.1: User Login
```
Step 1: Navigate to application login page (should already be there)
Step 2: Enter valid physician credentials
        Username/Email: [physician email]
        Password: [physician password]
Step 3: Click "Login" button
```
- **Expected**: Login successful, redirected to dashboard
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL
- **Screenshot**: [If failed, capture screenshot]

#### Test Case 1.2: Protected Route Access (Unauthorized)
```
Step 1: Open new browser window/tab
Step 2: Clear localStorage (Open DevTools → Application → LocalStorage → Clear All)
Step 3: Navigate directly to: https://localhost:5001/patients
```
- **Expected**: Redirected to login page
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 1.3: User Logout
```
Step 1: From dashboard, look for "Logout" button in navigation menu
Step 2: Click "Logout" button
Step 3: Verify page redirects to login
Step 4: Open DevTools → Application → LocalStorage
Step 5: Verify JWT token is removed
```
- **Expected**: Logged out, redirected to login, localStorage cleared
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 2.1: Register New Patient
```
Step 1: From dashboard, click "Patients" menu
Step 2: Click "Register New Patient" button
Step 3: Fill form with data:
        Full Name: John Doe
        Age: 45
        Gender: Male
        Email: john.doe@example.com
        Phone: 9876543210
        Address: 123 Main St, City
        Medical History: Hypertension
Step 4: Click "Register" button
```
- **Expected**: Patient registered, confirmation message, appears in list
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL
- **Database Verification**: Open SQL Server → Query: SELECT * FROM Patients WHERE Email='john.doe@example.com'
- **Result**: [ ] Data present [ ] Data missing

#### Test Case 2.2: Form Validation - Empty Fields
```
Step 1: Click "Register New Patient" again
Step 2: Leave all fields empty
Step 3: Click "Register" button
```
- **Expected**: Validation errors displayed for required fields
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL
- **Error Messages Seen**: _______________________

#### Test Case 2.3: Form Validation - Invalid Email
```
Step 1: Click "Register New Patient"
Step 2: Fill fields with valid data except:
        Email: "invalid-email" (no @ symbol)
Step 3: Click "Register" button
```
- **Expected**: Validation error for email format
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 2.4: Edit Existing Patient
```
Step 1: Go to Patients list
Step 2: Find John Doe (or any patient)
Step 3: Click "Edit" button
Step 4: Change phone number to: 1234567890
Step 5: Click "Save" button
```
- **Expected**: Patient updated, confirmation message, changes persisted
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 2.5: Search Patient by Name
```
Step 1: On Patients list, locate search box
Step 2: Type: "john"
Step 3: Observe results
Step 4: Record time: _________ seconds
```
- **Expected**: Returns patients with "john" (case-insensitive), <1 second response
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL
- **Response Time**: ___ seconds

#### Test Case 2.6: Search Patient by Phone
```
Step 1: Search box, enter: "987"
Step 2: Observe results
```
- **Expected**: Patients with "987" in phone returned
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

### Day 1 Summary
- Tests Passed: ___ / 9
- Tests Failed: ___
- Issues Found: ___
- Notes: _______________________

---

## 3. Day 2: Appointment Scheduling (Test Cases 3.1-3.4)

### Duration: 1-1.5 hours

#### Test Case 3.1: Schedule New Appointment
```
Step 1: Click "Appointments" in menu
Step 2: Click "Schedule Appointment" button
Step 3: Select patient: "John Doe" (from dropdown)
Step 4: Set date/time: Tomorrow at 2:00 PM
Step 5: Click "Schedule" button
```
- **Expected**: Appointment scheduled, confirmation message
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL
- **Database Check**: SELECT * FROM Appointments WHERE PatientId=[patient_id]

#### Test Case 3.2: Appointment Validation - Past Date
```
Step 1: Click "Schedule Appointment"
Step 2: Select a patient
Step 3: Try to set date to: Yesterday (e.g., using date picker, go back one day)
Step 4: Click "Schedule" button
```
- **Expected**: Validation error displayed, form not submitted
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL
- **Error Message**: _______________________

#### Test Case 3.3: Update Appointment Status
```
Step 1: Go to Appointments list
Step 2: Click on the appointment you just created
Step 3: Change status dropdown from "Scheduled" to "Completed"
Step 4: Click "Save" button
```
- **Expected**: Status updated, confirmation message
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 3.4: Appointment Conflict Detection
```
Step 1: Try to schedule another appointment for same patient (John Doe)
Step 2: Same time: Tomorrow at 2:00 PM
Step 3: Click "Schedule" button
```
- **Expected**: Validation error: "Patient already has appointment at this time"
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

### Day 2 Summary
- Tests Passed: ___ / 4
- Tests Failed: ___
- Issues Found: ___

---

## 4. Day 3: Consultation Workflow (Test Cases 4.1-4.4)

### Duration: 1.5 hours

#### Test Case 4.1: Complete Consultation - Vitals Capture
```
Step 1: Go to Appointments
Step 2: Find the appointment you marked as "Completed"
Step 3: Click "Add Consultation" or navigate to consultation creation
Step 4: Form should be pre-filled with appointment details
Step 5: Fill vitals:
        Temperature: 37.5
        Blood Pressure: 120/80
        Pulse: 72
Step 6: Fill complaints: "Headache for 2 days"
Step 7: Fill diagnosis: "Tension headache"
Step 8: Click "Save" button
```
- **Expected**: Consultation saved, confirmation message
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL
- **Database Check**: SELECT * FROM Consultations WHERE AppointmentId=[appointment_id]

#### Test Case 4.2: Vitals Validation - Temperature Out of Range
```
Step 1: Click "Add Consultation"
Step 2: Enter Temperature: 50 (out of valid 30-45°C range)
Step 3: Try to submit
```
- **Expected**: Validation error displayed
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL
- **Error Message**: _______________________

#### Test Case 4.3: Vitals Validation - Blood Pressure Format
```
Step 1: In consultation form
Step 2: Enter Blood Pressure: "120-80" (wrong format, should be 120/80)
Step 3: Try to submit
```
- **Expected**: Validation error for format
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 4.4: Mandatory Fields Validation
```
Step 1: In consultation form
Step 2: Fill only some fields, leave Temperature empty
Step 3: Try to submit
```
- **Expected**: Error that Temperature is required
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

### Day 3 Summary
- Tests Passed: ___ / 4
- Tests Failed: ___
- Issues Found: ___

---

## 5. Day 4: Prescription and History (Test Cases 5.1-6.3)

### Duration: 1.5-2 hours

#### Test Case 5.1: Generate Prescription
```
Step 1: Go to Consultations or from consultation details
Step 2: Click "Generate Prescription" button
Step 3: Add medications:
        - Name: Paracetamol, Dosage: 500mg, Frequency: Twice daily, Duration: 5 days
        - Name: Ibuprofen, Dosage: 400mg, Frequency: Once daily, Duration: 3 days
Step 4: Click "Save" button
```
- **Expected**: Prescription saved, confirmation message
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 5.2: Prescription View and Display
```
Step 1: Navigate to the prescription you just created
Step 2: Verify all sections displayed:
        [ ] Clinic/Doctor header
        [ ] Patient information (name, ID, age)
        [ ] Consultation vitals (Temperature, BP, Pulse)
        [ ] Diagnosis section
        [ ] Medications table (Name, Dosage, Frequency, Duration)
        [ ] Signature area
Step 3: Take screenshot for verification
```
- **Expected**: All sections present and formatted correctly
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 5.3: Print Prescription
```
Step 1: From prescription view, click "Print" button
Step 2: Browser print dialog should appear
Step 3: In print preview, verify:
        [ ] Layout is clean and professional
        [ ] All data visible and readable
        [ ] Headers and footers correct
        [ ] No cut-off content
Step 4: Send to PDF printer or save as PDF
Step 5: Open PDF file and verify quality
```
- **Expected**: Print preview clean, PDF quality good
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 6.1: View Patient History
```
Step 1: Go to Patients list
Step 2: Click on John Doe (patient with consultations)
Step 3: Click "View History" button
Step 4: Observe consultation list
```
- **Expected**: All past consultations displayed, ordered by most recent
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL
- **Consultations Count**: ___

#### Test Case 6.2: Filter History by Date Range
```
Step 1: In patient history view
Step 2: Set filter:
        Start Date: 30 days ago
        End Date: Today
Step 3: Click "Filter" button
```
- **Expected**: Consultations filtered to date range
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 6.3: View Consultation Details from History
```
Step 1: Click on a consultation in history
Step 2: Verify all details displayed:
        [ ] Vitals (Temp, BP, Pulse)
        [ ] Complaints text
        [ ] Diagnosis text
        [ ] Associated prescription
```
- **Expected**: All details displayed correctly
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

### Day 4 Summary
- Tests Passed: ___ / 6
- Tests Failed: ___
- Issues Found: ___

---

## 6. Day 5: Data Export and UI/UX (Test Cases 7.1-8.6)

### Duration: 2 hours

#### Test Case 7.1: Export Patient Data to Excel
```
Step 1: Click "Export" in menu
Step 2: Select "Patient Data" from datatype dropdown
Step 3: Select "Excel" from format dropdown
Step 4: Click "Export" button
Step 5: Save file (e.g., export_patients_[date].xlsx)
Step 6: Open in Excel
```
- **Expected**: File opens in Excel, all patient data present, dates DD-MM-YYYY
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL
- **File Size**: ___ KB
- **Date Format Verified**: [ ] YES

#### Test Case 7.2: Export Visit History with Date Filtering
```
Step 1: In Export page
Step 2: Select "Visit History" from datatype
Step 3: Select patient (or by ID if required)
Step 4: Set date range: Last 30 days
Step 5: Select "Excel" format
Step 6: Click "Export"
Step 7: Save and open file
```
- **Expected**: Consultations filtered to 30 days, DD-MM-YYYY format
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL
- **Records Exported**: ___

#### Test Case 7.3: Export Prescription Data to PDF
```
Step 1: In Export page
Step 2: Select "Prescriptions" datatype
Step 3: Select "PDF" format
Step 4: Click "Export"
Step 5: Open PDF file
```
- **Expected**: PDF opens correctly, all data visible, DD-MM-YYYY format
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 8.1: Navigation Menu Accessibility
```
Step 1: From any page, verify navigation menu
Step 2: Check all items are present:
        [ ] Home/Dashboard
        [ ] Patients
        [ ] Appointments
        [ ] History
        [ ] Export
        [ ] Logout
Step 3: Click each item
Step 4: Verify correct pages load
```
- **Expected**: All items accessible, correct pages load
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 8.2: Responsive Design - Mobile View (375px)
```
Step 1: Open browser DevTools
Step 2: Toggle device toolbar (375px width)
Step 3: Navigate through all pages
Step 4: Verify:
        [ ] Navigation collapses to hamburger menu
        [ ] Forms readable without zoom
        [ ] Buttons clickable
        [ ] No unwanted horizontal scroll
        [ ] Tables have scroll if needed
```
- **Expected**: All pages responsive on mobile
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL
- **Issues Found**: _______________________

#### Test Case 8.3: Responsive Design - Tablet View (768px)
```
Step 1: Resize to 768px width
Step 2: Test pages and workflows
```
- **Expected**: Proper tablet layout
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 8.4: Form Validation Messages Display
```
Step 1: Go to patient registration form
Step 2: Leave fields empty
Step 3: Try to submit
```
- **Expected**: Clear error messages near fields, ValidationSummary
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 8.5: Loading Indicators
```
Step 1: Trigger async operation (e.g., search, export)
Step 2: Observe UI during operation
```
- **Expected**: Loading spinner appears, button disabled
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

#### Test Case 8.6: Success and Error Messages
```
Step 1: Save patient successfully
Step 2: Observe success message styling
Step 3: Try invalid operation
Step 4: Observe error message styling
```
- **Expected**: Green success, red error, messages readable
- **Actual**: _______________________
- **Status**: [ ] PASS [ ] FAIL

### Day 5 Summary
- Tests Passed: ___ / 12
- Tests Failed: ___
- Issues Found: ___

---

## 7. Day 6: Training Time Measurement & Final Validation

### Duration: 2-3 hours

### Pre-Training Setup
```
1. Clear all test data from system
2. Prepare fresh database with no patient/appointment/consultation data
3. Have training checklist ready
4. Have stopwatch or timer ready
5. Have new trainee (physician user) available
```

### Training Execution (30 minutes)

**Start Time**: ___:___ | **End Time**: ___:___

**Minute 0-5: System Overview & Login**
- [ ] Explain system purpose
- [ ] Show login screen
- [ ] Show navigation menu after login
- [ ] Explain main workflow
- [ ] Trainee logs in successfully

**Minute 5-10: Patient Management**
- [ ] Demonstrate patient registration
- [ ] Trainee registers a patient
- [ ] Show search functionality

**Minute 10-15: Appointment Scheduling**
- [ ] Demonstrate appointment scheduling
- [ ] Trainee schedules appointment
- [ ] Show status updates

**Minute 15-20: Consultation Entry**
- [ ] Demonstrate consultation form
- [ ] Explain vitals fields
- [ ] Trainee enters consultation

**Minute 20-25: Prescription Generation**
- [ ] Demonstrate prescription generation
- [ ] Show print functionality
- [ ] Trainee generates prescription

**Minute 25-30: History and Export**
- [ ] Demonstrate history viewing
- [ ] Show export functionality
- [ ] Trainee exports data

### Independent Task (Minute 30+)
Ask trainee to complete workflow independently:
```
[ ] 1. Register new patient
[ ] 2. Schedule appointment for that patient
[ ] 3. Mark appointment as completed
[ ] 4. Enter consultation with vitals
[ ] 5. Generate prescription
[ ] 6. Export data to Excel
[ ] 7. Navigate to history and filter
```

**Time to Complete**: ___ minutes

### Training Assessment

**Completion**:
- [ ] All tasks completed
- [ ] Trainee required assistance: (describe): _______________________
- [ ] Trainee understood validation errors: [ ] YES [ ] NO
- [ ] Trainee could navigate independently: [ ] YES [ ] NO

**Trainee Feedback**:
```
Question 1: How easy was it to learn the system?
  Response: _______________________

Question 2: Were the error messages helpful?
  Response: _______________________

Question 3: Would you recommend any changes?
  Response: _______________________

Question 4: Can you use this system confidently?
  Response: [ ] YES [ ] NO [ ] With more practice
```

**Trainer Notes**:
```
_______________________________________________________
_______________________________________________________
_______________________________________________________
```

### Overall UAT Results Summary

#### Test Results by Category

| Category | Tests | Passed | Failed | Pass % |
|----------|-------|--------|--------|--------|
| Authentication | 3 | ___ | ___ | __% |
| Patient Management | 6 | ___ | ___ | __% |
| Appointments | 4 | ___ | ___ | __% |
| Consultations | 4 | ___ | ___ | __% |
| Prescriptions | 3 | ___ | ___ | __% |
| History | 3 | ___ | ___ | __% |
| Export | 4 | ___ | ___ | __% |
| UI/UX | 6 | ___ | ___ | __% |
| **TOTAL** | **33** | **___** | **___** | **___%** |

#### Critical Issues Found
```
Issue 1: [Describe]
Issue 2: [Describe]
Issue 3: [Describe]
```

#### UAT Acceptance Decision

- [ ] **PASS**: All critical tests passed, training <30 minutes, recommended for Step 17
- [ ] **CONDITIONAL PASS**: Minor issues found, can proceed with action items
- [ ] **FAIL**: Critical issues found, must fix before proceeding

### Sign-Off

**Test Executed By**: _________________________ (Name & Signature)  
**Date**: _________________________  

**Physician Confirms Readiness**: [ ] YES [ ] NO  
**Signature**: _________________________ **Date**: _________________________  

**Project Manager Approves**: [ ] YES [ ] NO  
**Signature**: _________________________ **Date**: _________________________  

---

## 8. Troubleshooting During UAT

### Issue: Login fails

**Troubleshooting Steps**:
1. Verify credentials are correct
2. Check if application is running
3. Clear browser cache and cookies
4. Try incognito/private window
5. Check server logs for errors

### Issue: Validation errors not showing

**Troubleshooting Steps**:
1. Check browser console for JavaScript errors (F12)
2. Verify DataAnnotations are in model
3. Check EditForm is using DataAnnotationsValidator
4. Verify form has ValidationSummary component

### Issue: Data not persisting

**Troubleshooting Steps**:
1. Check database connection string
2. Verify migrations were applied
3. Check SQL Server is running
4. Look for errors in application logs
5. Verify API endpoints are returning success

### Issue: Search is slow

**Troubleshooting Steps**:
1. Verify database indexes exist on search columns
2. Check if running on local or remote database
3. Profile query in SQL Server Management Studio
4. Check for N+1 query problems

---

## 9. Test Reporting

After completing all UAT test cases, generate report using template:

**Report Template**:
```
CLINICAL PATIENT MANAGEMENT SYSTEM - UAT REPORT
Date: [Date]
Tested By: [Name]

EXECUTIVE SUMMARY:
[1-2 sentence summary of UAT results]

TEST RESULTS:
Total Test Cases: 33
Passed: ___
Failed: ___
Pass Rate: ___%

TRAINING RESULTS:
Training Time: ___ minutes (Target: <30 minutes)
Status: [PASS/FAIL]

ISSUES FOUND:
[List any issues by severity]

RECOMMENDATION:
[ ] Approved for Step 17
[ ] Approved with conditions
[ ] Not approved - requires fixes

SIGN-OFF:
[Signatures and dates]
```

---

**Document Version**: 1.0  
**Last Updated**: May 12, 2026
