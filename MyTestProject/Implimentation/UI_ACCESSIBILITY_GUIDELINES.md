# UI/ACCESSIBILITY GUIDELINES
## WCAG 2.1 Level AA Compliance Standards

**Document Version**: 1.0  
**Last Updated**: May 11, 2026  
**Scope**: Clinical Patient Management System - All Blazor Components  
**Standard**: WCAG 2.1 Level AA

---

## 1. EXECUTIVE SUMMARY

This document defines accessibility standards and best practices for all UI components in the Clinical Patient Management System. Compliance with WCAG 2.1 Level AA ensures the application is accessible to users with disabilities including:
- Visual impairments (blindness, low vision, color blindness)
- Hearing impairments (deafness, hard of hearing)
- Motor impairments (paralysis, tremors, slow response time)
- Cognitive impairments (dyslexia, learning disabilities)
- Temporary impairments (broken arm, loud environment)

**Target Compliance**: WCAG 2.1 Level AA (minimum)  
**Testing Tools**: axe DevTools, Lighthouse, WAVE, Pa11y  
**Responsible Team**: Frontend developers, QA specialists

---

## 2. FUNDAMENTAL PRINCIPLES (POUR)

### 2.1 Perceivable
Users must be able to perceive the information presented in the interface.

#### 2.1.1 Text Alternatives
- **Requirement**: All non-text content has text alternatives
- **Implementation**:
  ```blazor
  <!-- Icon with label -->
  <i class="bi bi-pencil" aria-hidden="true"></i>
  <span>Edit</span>
  
  <!-- OR icon-only button with aria-label -->
  <button class="btn btn-sm btn-primary" aria-label="Edit patient record">
      <i class="bi bi-pencil" aria-hidden="true"></i>
  </button>
  
  <!-- Image with alt text -->
  <img src="clinic-logo.png" alt="Clinical Patient Management System Logo" />
  ```
- **Tools to Verify**: axe, WAVE
- **Common Failures**: Missing alt text on images, unlabeled icons, empty aria-labels

#### 2.1.2 Distinguishable Content
- **Requirement**: Text/images are distinguishable from background
- **Color Contrast Ratios**:
  - Normal text: 4.5:1 minimum (WCAG AA)
  - Large text (18pt+ or 14pt bold): 3:1 minimum
  - Graphics/UI components: 3:1 minimum
- **Implementation**:
  ```css
  /* Sufficient contrast */
  body { color: #333; background: #fff; } /* 12.6:1 ratio */
  .alert-info { color: #004085; background: #d1ecf1; } /* 7:1 ratio */
  
  /* Insufficient contrast (AVOID) */
  .text-muted { color: #999; } /* 3.8:1 on white - FAILS for normal text */
  ```
- **Tools to Verify**: WebAIM Contrast Checker, Lighthouse
- **Testing**: Check all text, buttons, links, form fields

#### 2.1.3 Adaptable Content
- **Requirement**: Content must be adaptable to different viewport sizes
- **Implementation**:
  ```blazor
  <!-- Responsive grid -->
  <div class="row">
      <div class="col-12 col-md-6 col-lg-4">
          <div class="card"><!-- Content --></div>
      </div>
  </div>
  
  <!-- Responsive table with horizontal scroll -->
  <div class="table-responsive">
      <table class="table">
          <!-- Content -->
      </table>
  </div>
  ```
- **Tools to Verify**: Chrome DevTools (375px, 768px, 1920px)
- **Common Failures**: Fixed widths, horizontal scrolling on mobile, text too small

### 2.2 Operable
Users must be able to operate the interface.

#### 2.2.1 Keyboard Accessibility
- **Requirement**: All functionality available via keyboard
- **Implementation**:
  ```blazor
  <!-- Tab navigation order -->
  <input type="text" tabindex="1" />
  <input type="email" tabindex="2" />
  <button type="submit" tabindex="3">Submit</button>
  
  <!-- Skip to main content link -->
  <a href="#main-content" class="skip-link">Skip to main content</a>
  <main id="main-content" role="main">
      <!-- Main content -->
  </main>
  
  <!-- Dropdown with keyboard support -->
  <a class="nav-link dropdown-toggle" 
     role="button"
     tabindex="0"
     aria-expanded="false"
     aria-haspopup="true"
     @onclick="ToggleDropdown"
     @onkeydown="HandleDropdownKeydown">
      Menu
  </a>
  
  @code {
      private void HandleDropdownKeydown(KeyboardEventArgs e)
      {
          if (e.Key == "Enter" || e.Key == " ")
          {
              ToggleDropdown();
              e.PreventDefault();
          }
          else if (e.Key == "Escape")
          {
              CloseDropdown();
          }
      }
  }
  ```
- **Tab Order Guidelines**:
  - Natural reading order (left-to-right, top-to-bottom)
  - Use default tab order when possible
  - Only set tabindex="0" for custom widgets
  - Never use tabindex > 0 (Creates confusion)
- **Tools to Verify**: Keyboard navigation testing, axe
- **Common Failures**: Can't access form fields with Tab, buttons not clickable with Enter

#### 2.2.2 Focus Indicator
- **Requirement**: Focus must be visible at all times
- **Implementation**:
  ```css
  /* Focus visible - works on modern browsers */
  button:focus-visible,
  a:focus-visible,
  input:focus-visible,
  select:focus-visible,
  textarea:focus-visible {
      outline: 3px solid #0066cc;
      outline-offset: 2px;
  }
  
  /* Fallback for older browsers */
  button:focus,
  a:focus,
  input:focus {
      outline: 3px solid #0066cc;
  }
  
  /* Remove default outline only if custom outline provided */
  button:focus:not(:focus-visible) {
      outline: none;
  }
  
  /* High contrast mode */
  @media (prefers-contrast: more) {
      button:focus-visible,
      input:focus-visible {
          outline-width: 4px;
      }
  }
  ```
- **Minimum Requirements**:
  - Outline width: 2px minimum (preferably 3px)
  - Color: High contrast (blue #0066cc or #2563eb recommended)
  - Always visible (never hidden with `outline: none`)
- **Tools to Verify**: Manual keyboard testing, Lighthouse
- **Common Failures**: No visible focus, outline removed with CSS

#### 2.2.3 Sufficient Time
- **Requirement**: Users have enough time to read and use content
- **Implementation**:
  - No time-outs for form submission (or extend before timeout)
  - No auto-advancing content
  - Pause/stop controls for animations
  - Session timeout warning with extension option
- **Code Example** (if needed):
  ```blazor
  @* Session timeout warning *@
  <div class="alert alert-warning" role="alert" aria-live="assertive">
      <strong>Session Timeout Warning</strong>
      <p>Your session will expire in <span id="timeout-counter">5</span> minutes.</p>
      <button class="btn btn-primary" @onclick="ExtendSession">
          Extend Session
      </button>
  </div>
  ```

#### 2.2.4 Seizure Prevention
- **Requirement**: No content flashes more than 3 times per second
- **Implementation**:
  - Keep animations smooth (no strobing)
  - Avoid flashing content
  - Use CSS animations with reasonable timing
- **Current Status**: ✅ No animations exceed 3 flashes/second

### 2.3 Understandable
Users must be able to understand the interface and content.

#### 2.3.1 Readable Text
- **Requirement**: Text is readable and understandable
- **Implementation**:
  ```blazor
  <!-- Clear, simple language -->
  <label for="phone" class="form-label">
      Phone Number <span class="text-muted">(123) 456-7890</span>
  </label>
  
  <!-- Avoid jargon or explain acronyms -->
  <p>
      The Electronic Health Record (EHR) system stores your medical information.
  </p>
  
  <!-- Use descriptive headings -->
  <h1>Patient Registration</h1>
  <h2>Personal Information</h2>
  <h3>Address Details</h3>
  
  <!-- Provide clear instructions -->
  <div class="form-text">
      Enter a phone number in format: (123) 456-7890
  </div>
  ```
- **Language Guidelines**:
  - Use simple, clear language
  - Short sentences (< 20 words)
  - Define technical terms
  - Use lists for multiple items
  - Use headings to structure content
- **Tools to Verify**: Readability checkers, user testing
- **Common Failures**: Unclear instructions, jargon without explanation

#### 2.3.2 Predictable Navigation
- **Requirement**: Navigation is consistent and predictable
- **Implementation**:
  ```blazor
  <!-- Consistent navigation in same location -->
  <Navigation /> @* Same for all pages *@
  
  <!-- Predictable link behavior -->
  <a href="/patients">View Patients</a> @* Opens page *@
  <a href="https://example.com" target="_blank">External Site</a> @* Warns user *@
  
  <!-- Breadcrumb navigation -->
  <nav aria-label="breadcrumb">
      <ol class="breadcrumb">
          <li class="breadcrumb-item"><a href="/">Home</a></li>
          <li class="breadcrumb-item"><a href="/patients">Patients</a></li>
          <li class="breadcrumb-item active" aria-current="page">John Doe</li>
      </ol>
  </nav>
  
  <!-- Consistent page layout -->
  <MainLayout> @* Consistent for all pages *@
      <!-- Header, navigation, content, footer always in same place -->
  </MainLayout>
  ```
- **Guidelines**:
  - Keep navigation in same location
  - Use consistent terminology
  - Make link destination clear
  - Use breadcrumbs for complex navigation
- **Tools to Verify**: Automated testing, user testing

#### 2.3.3 Input Assistance
- **Requirement**: Users are helped to avoid and correct mistakes
- **Implementation**:
  ```blazor
  <!-- Clear error messages -->
  <div class="mb-3">
      <label for="temperature" class="form-label">
          Temperature (°C) <span class="text-danger">*</span>
      </label>
      <InputNumber 
          id="temperature"
          class="form-control"
          @bind-value="consultation.Temperature"
          aria-required="true"
          aria-describedby="temperatureHelp temperatureError" />
      <div id="temperatureHelp" class="form-text">
          Enter value between 30 and 45°C
      </div>
      <ValidationMessage 
          For="@(() => consultation.Temperature)"
          id="temperatureError"
          role="alert"
          aria-live="assertive" />
  </div>
  
  <!-- Form submission confirmation -->
  <div class="alert alert-success" role="alert" aria-live="polite">
      Patient saved successfully!
  </div>
  
  <!-- Undo/recover from errors -->
  <button class="btn btn-secondary" @onclick="UndoLastAction">
      Undo Last Action
  </button>
  ```
- **Error Prevention Guidelines**:
  - Provide clear labels and instructions
  - Identify errors clearly
  - Suggest corrections
  - Allow easy correction
  - Use client-side validation
- **Tools to Verify**: User testing, manual testing

### 2.4 Robust
Content must be robust enough to be interpreted reliably.

#### 2.4.1 HTML Validity
- **Requirement**: Valid HTML (W3C)
- **Implementation**:
  ```blazor
  <!-- Proper nesting -->
  <div class="card">
      <div class="card-header">
          <h5>Title</h5>
      </div>
      <div class="card-body">
          <p>Content</p>
      </div>
  </div>
  
  <!-- Proper use of semantic HTML -->
  <nav><!-- Navigation --></nav>
  <main><!-- Main content --></main>
  <section><!-- Content section --></section>
  <article><!-- Article content --></article>
  <aside><!-- Side content --></aside>
  <footer><!-- Footer --></footer>
  ```
- **Validation**:
  ```bash
  # Validate HTML
  # Use W3C HTML Validator
  # https://validator.w3.org/
  ```
- **Tools to Verify**: W3C HTML Validator, axe

#### 2.4.2 ARIA Implementation
- **Requirement**: ARIA used correctly when native HTML insufficient
- **Implementation**:
  ```blazor
  <!-- Use native HTML first -->
  <button>Save</button> @* Prefer native over ARIA *@
  <a href="/page">Link</a> @* Prefer native over ARIA *@
  
  <!-- Use ARIA only when necessary -->
  <div role="button" tabindex="0" @onclick="HandleClick">
      Custom Button
  </div>
  
  <!-- Proper ARIA usage -->
  <a class="nav-link dropdown-toggle"
     role="button"
     tabindex="0"
     aria-expanded="false"
     aria-haspopup="true">
      Menu
  </a>
  
  <!-- aria-label for icon-only buttons -->
  <button aria-label="Close menu" @onclick="CloseMenu">
      <i class="bi bi-x" aria-hidden="true"></i>
  </button>
  
  <!-- aria-live for dynamic content -->
  <div aria-live="polite" aria-atomic="true">
      @statusMessage
  </div>
  
  <!-- aria-label for complex components -->
  <table aria-label="Patient appointment schedule">
      <!-- Table content -->
  </table>
  ```
- **ARIA Rules**:
  - Use native HTML when possible
  - Use ARIA to supplement, not replace, HTML
  - Don't use ARIA incorrectly
  - Always provide accessible names
  - Use aria-label, aria-labelledby, or text content
- **Common ARIA Misuses**:
  - `<div role="button">` instead of `<button>`
  - `aria-label` on `<input type="text">` (use `<label>` instead)
  - `role="link"` on `<button>` (use `<a>` instead)
  - Multiple `aria-label` attributes (invalid)
- **Tools to Verify**: axe, WAVE

---

## 3. COMPONENT ACCESSIBILITY PATTERNS

### 3.1 Forms

#### Pattern: Form Field with Validation
```blazor
<div class="mb-3">
    <label for="firstName" class="form-label">
        First Name 
        <span class="text-danger" aria-label="required">*</span>
    </label>
    <InputText 
        id="firstName" 
        class="form-control"
        @bind-value="patient.FirstName"
        placeholder="Enter first name"
        aria-required="true"
        aria-describedby="firstNameHelp"
        aria-invalid="@(!IsValid(patient.FirstName))" />
    <div id="firstNameHelp" class="form-text">
        Maximum 100 characters
    </div>
    <ValidationMessage 
        For="@(() => patient.FirstName)"
        role="alert" 
        aria-live="assertive" />
</div>
```

#### Pattern: Form Group
```blazor
<fieldset class="border rounded p-3">
    <legend class="float-none w-auto px-2">
        Patient Information
    </legend>
    <!-- Form fields -->
</fieldset>
```

### 3.2 Tables

#### Pattern: Accessible Table
```blazor
<div class="table-responsive">
    <table class="table table-hover">
        <caption>Patient appointment schedule</caption>
        <thead class="table-light">
            <tr>
                <th scope="col">Date</th>
                <th scope="col">Patient</th>
                <th scope="col">Status</th>
                <th scope="col">Actions</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var appointment in appointments)
            {
                <tr>
                    <td>@appointment.Date</td>
                    <td>@appointment.PatientName</td>
                    <td>
                        <span class="badge bg-success">@appointment.Status</span>
                    </td>
                    <td>
                        <button class="btn btn-sm btn-primary"
                                aria-label="Edit appointment for @appointment.PatientName">
                            Edit
                        </button>
                    </td>
                </tr>
            }
        </tbody>
    </table>
</div>
```

### 3.3 Navigation

#### Pattern: Accessible Navigation Menu
```blazor
<nav class="navbar navbar-expand-lg navbar-dark bg-dark" 
     aria-label="Main application navigation">
    <div class="container-fluid">
        <a class="navbar-brand" href="/">
            <i class="bi bi-hospital" aria-hidden="true"></i>
            Patient Management System
        </a>
        
        <button class="navbar-toggler" 
                type="button" 
                data-bs-toggle="collapse" 
                data-bs-target="#navbarNav"
                aria-controls="navbarNav"
                aria-expanded="false"
                aria-label="Toggle navigation menu">
            <span class="navbar-toggler-icon"></span>
        </button>
        
        <div class="collapse navbar-collapse" id="navbarNav">
            <ul class="navbar-nav ms-auto">
                <li class="nav-item">
                    <a class="nav-link" href="/dashboard" aria-current="page">Dashboard</a>
                </li>
                <li class="nav-item dropdown">
                    <a class="nav-link dropdown-toggle" 
                       href="#" 
                       id="patientMenu" 
                       role="button" 
                       aria-expanded="false"
                       aria-haspopup="true">
                        Patients
                    </a>
                    <ul class="dropdown-menu" aria-labelledby="patientMenu">
                        <li><a class="dropdown-item" href="/patients">View All</a></li>
                        <li><a class="dropdown-item" href="/patients/create">New Patient</a></li>
                    </ul>
                </li>
            </ul>
        </div>
    </div>
</nav>
```

### 3.4 Alerts & Messages

#### Pattern: Accessible Alert
```blazor
<div class="alert alert-success" role="alert" aria-live="polite" aria-atomic="true">
    <i class="bi bi-check-circle" aria-hidden="true"></i>
    <strong>Success!</strong> Patient saved successfully.
</div>

<div class="alert alert-danger" role="alert" aria-live="assertive" aria-atomic="true">
    <i class="bi bi-exclamation-circle" aria-hidden="true"></i>
    <strong>Error!</strong> Unable to save patient record.
</div>
```

### 3.5 Buttons

#### Pattern: Accessible Button
```blazor
<!-- Standard button -->
<button class="btn btn-primary" type="button" @onclick="SavePatient">
    <i class="bi bi-save" aria-hidden="true"></i>
    Save Patient
</button>

<!-- Icon-only button -->
<button class="btn btn-sm btn-warning" 
        aria-label="Edit patient information"
        @onclick="@(() => EditPatient(patient.Id))">
    <i class="bi bi-pencil" aria-hidden="true"></i>
</button>

<!-- Button with description -->
<button class="btn btn-danger" aria-label="Delete patient permanently" @onclick="DeletePatient">
    <i class="bi bi-trash" aria-hidden="true"></i>
    Delete
</button>
```

---

## 4. TESTING CHECKLIST

### Automated Testing
- [ ] axe DevTools: 0 violations
- [ ] Lighthouse: Accessibility > 90
- [ ] WAVE: 0 errors
- [ ] HTML Validator: 0 errors
- [ ] Color contrast: ≥ 4.5:1 for text

### Manual Testing
- [ ] Keyboard navigation: All elements accessible
- [ ] Screen reader: All content announced correctly
- [ ] Focus indicator: Visible on all interactive elements
- [ ] Zoom 200%: All content readable
- [ ] Responsive design: 375px, 768px, 1920px viewports
- [ ] High contrast mode: Colors sufficient
- [ ] Reduced motion: Animations reduced/disabled

### Browser/AT Testing
- [ ] Chrome + keyboard
- [ ] Firefox + keyboard
- [ ] Edge + keyboard
- [ ] Safari + keyboard
- [ ] NVDA (Windows)
- [ ] JAWS (if available)
- [ ] Safari VoiceOver (macOS)

---

## 5. COMMON FAILURES & SOLUTIONS

| Failure | Cause | Solution |
|---------|-------|----------|
| Images without alt text | Missing `alt` attribute | Add descriptive `alt="..."` |
| Low color contrast | Insufficient color difference | Use contrast checker, adjust colors |
| Keyboard trap | Focus can't leave element | Implement proper focus management |
| Missing form labels | No `<label>` element | Add `<label for="id">` and match input `id` |
| Unlabeled buttons | No text or aria-label | Add button text or `aria-label` |
| No focus indicator | Outline removed with CSS | Add `button:focus-visible { outline: ... }` |
| Inaccessible dropdown | No keyboard support | Add `@onkeydown` handler for Enter/Escape |
| Skip navigation missing | Can't jump to content | Add skip link in header |
| Layout table for styling | Confuses screen readers | Use CSS flexbox/grid instead |
| Page structure unclear | No proper heading hierarchy | Use H1, H2, H3 correctly |

---

## 6. RESOURCES & TOOLS

### Documentation
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [ARIA Authoring Practices](https://www.w3.org/WAI/ARIA/apg/)
- [WebAIM Articles](https://webaim.org/)
- [A11y Project](https://www.a11yproject.com/)

### Tools
- **axe DevTools**: Chrome/Firefox extension for accessibility audit
- **Lighthouse**: Chrome DevTools built-in accessibility audit
- **WAVE**: WebAIM accessibility evaluation tool
- **Pa11y**: Command-line accessibility testing
- **WebAIM Contrast Checker**: Color contrast ratio checker

### Testing
- **NVDA**: Free screen reader for Windows
- **JAWS**: Commercial screen reader
- **VoiceOver**: Built-in macOS/iOS screen reader
- **Chrome DevTools**: Device emulation, accessibility tree

---

## 7. SIGN-OFF & COMPLIANCE

**This document defines accessibility standards for the Clinical Patient Management System.**

- ✅ Standard: **WCAG 2.1 Level AA**
- ✅ Implementation Date: **May 11, 2026**
- ✅ Review Date: **Quarterly**
- ✅ Responsible Party: **Development Team Lead**

All developers must follow these guidelines when creating or modifying components.

---

**Document Version**: 1.0  
**Last Updated**: May 11, 2026  
**Status**: Active
