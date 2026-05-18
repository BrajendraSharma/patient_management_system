# Step 16: UAT Quick Reference Checklist

**Print this page for quick reference during UAT testing**

---

## Pre-Testing Checklist

### Environment Setup
- [ ] Application running on localhost or test server
- [ ] Browser: Chrome/Firefox/Edge (version 120+)
- [ ] SQL Server database running and accessible
- [ ] Test data created (10+ sample patients)
- [ ] Logging enabled
- [ ] Network connectivity verified

### Test Credentials
- **Physician Email**: _____________________________
- **Password**: _____________________________
- **Test Physician Name**: _____________________________

### Test Data Reference
```
Primary Test Patient: John Doe
  Email: john.doe@example.com
  Phone: 9876543210
  Age: 45, Male
```

---

## Test Execution Quick Reference

### Test Case Numbering

**Authentication (3 tests)**
- 1.1: Login
- 1.2: Protected route redirect
- 1.3: Logout

**Patient Management (6 tests)**
- 2.1: Register patient
- 2.2: Validation - empty fields
- 2.3: Validation - invalid email
- 2.4: Edit patient
- 2.5: Search by name
- 2.6: Search by phone

**Appointments (4 tests)**
- 3.1: Schedule appointment
- 3.2: Validation - past date
- 3.3: Update status
- 3.4: Conflict detection

**Consultations (4 tests)**
- 4.1: Complete consultation
- 4.2: Validation - temperature range
- 4.3: Validation - BP format
- 4.4: Mandatory fields

**Prescriptions (3 tests)**
- 5.1: Generate prescription
- 5.2: Display prescription
- 5.3: Print prescription

**History (3 tests)**
- 6.1: View history
- 6.2: Filter by date
- 6.3: View details

**Export (4 tests)**
- 7.1: Export patients to Excel
- 7.2: Export history to Excel with filtering
- 7.3: Export prescriptions to PDF
- 7.4: Verify date format

**UI/UX (6 tests)**
- 8.1: Navigation menu
- 8.2: Mobile responsive (375px)
- 8.3: Tablet responsive (768px)
- 8.4: Validation messages
- 8.5: Loading indicators
- 8.6: Success/error messages

---

## Validation Ranges & Formats

### Consultation Vitals Validation
```
Temperature: 30°C - 45°C
Blood Pressure: Format XX/XX (e.g., 120/80)
Pulse: 40 - 200 bpm
```

### Date Format Requirement
```
Export/Display Format: DD-MM-YYYY
Example: 12-05-2026
```

### Email Validation
```
Valid: user@domain.com
Invalid: user@domain (missing TLD)
Invalid: userdomaincom (missing @)
```

### Search Requirements
```
Case-Insensitive: "john", "John", "JOHN" all find John Doe
Partial Match: "John" finds "John", "Johnny", "Johnson"
Ordered By: Most recent registration first
Response Time: <1 second
```

---

## Daily Test Summary

### Day 1: Authentication & Patient Management
**Duration**: 1.5-2 hours | **Tests**: 9 | **Date**: _______

| Test # | Name | Status | Notes |
|--------|------|--------|-------|
| 1.1 | Login | [ ] PASS [ ] FAIL | _______ |
| 1.2 | Protected Route | [ ] PASS [ ] FAIL | _______ |
| 1.3 | Logout | [ ] PASS [ ] FAIL | _______ |
| 2.1 | Register Patient | [ ] PASS [ ] FAIL | _______ |
| 2.2 | Validation - Empty | [ ] PASS [ ] FAIL | _______ |
| 2.3 | Validation - Email | [ ] PASS [ ] FAIL | _______ |
| 2.4 | Edit Patient | [ ] PASS [ ] FAIL | _______ |
| 2.5 | Search by Name | [ ] PASS [ ] FAIL | _______ |
| 2.6 | Search by Phone | [ ] PASS [ ] FAIL | _______ |

**Day 1 Summary**: ___ / 9 PASS

---

### Day 2: Appointment Scheduling
**Duration**: 1-1.5 hours | **Tests**: 4 | **Date**: _______

| Test # | Name | Status | Notes |
|--------|------|--------|-------|
| 3.1 | Schedule Appointment | [ ] PASS [ ] FAIL | _______ |
| 3.2 | Validation - Past Date | [ ] PASS [ ] FAIL | _______ |
| 3.3 | Update Status | [ ] PASS [ ] FAIL | _______ |
| 3.4 | Conflict Detection | [ ] PASS [ ] FAIL | _______ |

**Day 2 Summary**: ___ / 4 PASS

---

### Day 3: Consultation Workflow
**Duration**: 1.5 hours | **Tests**: 4 | **Date**: _______

| Test # | Name | Status | Notes |
|--------|------|--------|-------|
| 4.1 | Complete Consultation | [ ] PASS [ ] FAIL | _______ |
| 4.2 | Validation - Temp Range | [ ] PASS [ ] FAIL | _______ |
| 4.3 | Validation - BP Format | [ ] PASS [ ] FAIL | _______ |
| 4.4 | Mandatory Fields | [ ] PASS [ ] FAIL | _______ |

**Day 3 Summary**: ___ / 4 PASS

---

### Day 4: Prescriptions & History
**Duration**: 1.5-2 hours | **Tests**: 6 | **Date**: _______

| Test # | Name | Status | Notes |
|--------|------|--------|-------|
| 5.1 | Generate Prescription | [ ] PASS [ ] FAIL | _______ |
| 5.2 | Display Prescription | [ ] PASS [ ] FAIL | _______ |
| 5.3 | Print Prescription | [ ] PASS [ ] FAIL | _______ |
| 6.1 | View History | [ ] PASS [ ] FAIL | _______ |
| 6.2 | Filter History | [ ] PASS [ ] FAIL | _______ |
| 6.3 | View Details | [ ] PASS [ ] FAIL | _______ |

**Day 4 Summary**: ___ / 6 PASS

---

### Day 5: Export & UI/UX
**Duration**: 2 hours | **Tests**: 10 | **Date**: _______

| Test # | Name | Status | Notes |
|--------|------|--------|-------|
| 7.1 | Export Patients Excel | [ ] PASS [ ] FAIL | _______ |
| 7.2 | Export History Excel | [ ] PASS [ ] FAIL | _______ |
| 7.3 | Export Prescriptions PDF | [ ] PASS [ ] FAIL | _______ |
| 7.4 | Date Format Check | [ ] PASS [ ] FAIL | _______ |
| 8.1 | Navigation Menu | [ ] PASS [ ] FAIL | _______ |
| 8.2 | Mobile Responsive | [ ] PASS [ ] FAIL | _______ |
| 8.3 | Tablet Responsive | [ ] PASS [ ] FAIL | _______ |
| 8.4 | Validation Messages | [ ] PASS [ ] FAIL | _______ |
| 8.5 | Loading Indicators | [ ] PASS [ ] FAIL | _______ |
| 8.6 | Success/Error Msgs | [ ] PASS [ ] FAIL | _______ |

**Day 5 Summary**: ___ / 10 PASS

---

### Day 6: Training & Final Validation
**Duration**: 2-3 hours | **Date**: _______

**Training Session**
- [ ] Pre-training setup complete
- [ ] Trainee available
- [ ] Timer ready
- [ ] Start Time: ___:___
- [ ] End Time: ___:___
- **Total Training Time**: ___ minutes

**Training Completion**
- [ ] All tasks completed
- [ ] Trainee confirmed understanding
- [ ] Feedback collected

**Final Validation**
- [ ] All 33 tests passed
- [ ] Training time <30 minutes
- [ ] No critical issues
- [ ] Ready for sign-off

---

## Critical Test Scenarios (High Priority)

### Scenario 1: Full Patient Journey
```
1. Register new patient ..................... [ ]
2. Schedule appointment ..................... [ ]
3. Mark appointment completed ............... [ ]
4. Enter consultation with vitals ........... [ ]
5. Generate prescription ................... [ ]
6. Verify data saved to database ........... [ ]
Status: PASS [ ] / FAIL [ ]
```

### Scenario 2: Data Validation
```
1. Try empty form submission ............... [ ]
2. Try invalid email ....................... [ ]
3. Try past date appointment ............... [ ]
4. Try out-of-range vitals ................. [ ]
Status: All errors caught? YES [ ] / NO [ ]
```

### Scenario 3: Search & Filter
```
1. Search patient by partial name ......... [ ]
2. Search patient by phone ................ [ ]
3. Filter history by date range ........... [ ]
4. Response time <1 second? YES [ ] / NO [ ]
Status: All working? YES [ ] / NO [ ]
```

---

## Responsive Design Testing Quick Checks

### Mobile (375px)
- [ ] Hamburger menu appears
- [ ] Forms readable without zoom
- [ ] Buttons clickable
- [ ] No horizontal scroll
- [ ] Tables scrollable

### Tablet (768px)
- [ ] Two-column layouts work
- [ ] All content accessible
- [ ] No excessive scrolling

### Desktop (1920px)
- [ ] Full layout utilized
- [ ] Spacing appropriate
- [ ] Professional appearance

---

## Browser Testing Checklist

### Chrome (Version: _____)
- [ ] All tests executed
- [ ] All features work
- [ ] Console no errors

### Firefox (Version: _____)
- [ ] All tests executed
- [ ] All features work
- [ ] Console no errors

### Edge (Version: _____)
- [ ] All tests executed
- [ ] All features work
- [ ] Console no errors

---

## Database Verification Queries

### Patient Created
```sql
SELECT * FROM Patients WHERE Email='john.doe@example.com'
Expected: 1 row returned
```

### Appointment Created
```sql
SELECT * FROM Appointments WHERE PatientId=[patient_id]
Expected: 1+ rows for scheduled appointments
```

### Consultation Saved
```sql
SELECT * FROM Consultations WHERE AppointmentId=[appointment_id]
Expected: Vitals, complaints, diagnosis present
```

### Prescription Created
```sql
SELECT * FROM Prescriptions WHERE ConsultationId=[consultation_id]
Expected: Medications table populated
```

---

## Performance Targets

| Metric | Target | Actual | Status |
|--------|--------|--------|--------|
| Search Response | <1 sec | ___ sec | [ ] ✓ [ ] ✗ |
| Page Load | <2 sec | ___ sec | [ ] ✓ [ ] ✗ |
| Training Time | <30 min | ___ min | [ ] ✓ [ ] ✗ |
| Form Validation | Instant | ___ | [ ] ✓ [ ] ✗ |
| Export to Excel | <5 sec | ___ sec | [ ] ✓ [ ] ✗ |

---

## Common Issues & Quick Fixes

| Issue | Quick Fix | Tried |
|-------|-----------|-------|
| Login fails | Clear cache & cookies | [ ] |
| Validation not showing | Open DevTools console | [ ] |
| Data not saved | Check DB connection | [ ] |
| Search slow | Verify DB indexes | [ ] |
| Export fails | Check file permissions | [ ] |
| Print not working | Check browser print settings | [ ] |

---

## Final Sign-Off

### Test Results
- **Total Tests**: 33
- **Passed**: ___
- **Failed**: ___
- **Pass Rate**: ___%

### Training
- **Training Time**: ___ minutes
- **Pass Criteria Met**: [ ] YES [ ] NO

### Issues
- **Critical Issues**: ___
- **High Priority**: ___
- **Low Priority**: ___

### Recommendation
- [ ] APPROVED - Ready for Step 17
- [ ] CONDITIONAL - Approved with issues
- [ ] NOT APPROVED - Requires fixes

### Sign-Off
**Tester**: _________________________ **Date**: _______  
**Physician**: _________________________ **Date**: _______  
**PM**: _________________________ **Date**: _______

---

**Keep this checklist with you during UAT testing for quick reference!**
