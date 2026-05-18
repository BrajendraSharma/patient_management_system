using ClinicalPatientManagement.Api.DTOs;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests.Validation;

/// <summary>
/// Tests for Phase 1 DTO validation - ensures all input validation is properly configured.
/// </summary>
public class DtoValidationTests
{
    private static IEnumerable<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    #region LoginDto Tests

    [Fact]
    public void LoginDto_WithValidCredentials_PassesValidation()
    {
        // Arrange
        var dto = new LoginDto { Username = "doctor", Password = "ValidPassword123!" };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void LoginDto_WithEmptyUsername_FailsValidation()
    {
        // Arrange
        var dto = new LoginDto { Username = "", Password = "ValidPassword123!" };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
        Assert.Single(results.Where(r => r.MemberNames.Contains(nameof(LoginDto.Username))));
    }

    [Fact]
    public void LoginDto_WithEmptyPassword_FailsValidation()
    {
        // Arrange
        var dto = new LoginDto { Username = "doctor", Password = "" };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
        Assert.Single(results.Where(r => r.MemberNames.Contains(nameof(LoginDto.Password))));
    }

    [Fact]
    public void LoginDto_WithNullUsername_FailsValidation()
    {
        // Arrange
        var dto = new LoginDto { Username = null!, Password = "ValidPassword123!" };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
    }

    [Fact]
    public void LoginDto_WithNullPassword_FailsValidation()
    {
        // Arrange
        var dto = new LoginDto { Username = "doctor", Password = null! };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
    }

    #endregion

    #region CreatePatientDto Tests

    [Fact]
    public void CreatePatientDto_WithValidData_PassesValidation()
    {
        // Arrange
        var dto = new CreatePatientDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "+1234567890",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = "Male"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void CreatePatientDto_WithEmptyFirstName_FailsValidation()
    {
        // Arrange
        var dto = new CreatePatientDto
        {
            FirstName = "",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "+1234567890",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = "Male"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
        Assert.Single(results.Where(r => r.MemberNames.Contains(nameof(CreatePatientDto.FirstName))));
    }

    [Fact]
    public void CreatePatientDto_WithInvalidEmail_FailsValidation()
    {
        // Arrange
        var dto = new CreatePatientDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "not-an-email",
            Phone = "+1234567890",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = "Male"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
        Assert.Single(results.Where(r => r.MemberNames.Contains(nameof(CreatePatientDto.Email))));
    }

    [Fact]
    public void CreatePatientDto_WithInvalidPhone_FailsValidation()
    {
        // Arrange
        var dto = new CreatePatientDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "invalid-phone",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = "Male"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
        Assert.Single(results.Where(r => r.MemberNames.Contains(nameof(CreatePatientDto.Phone))));
    }

    [Fact]
    public void CreatePatientDto_WithFirstNameTooLong_FailsValidation()
    {
        // Arrange
        var dto = new CreatePatientDto
        {
            FirstName = new string('a', 101),
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "+1234567890",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = "Male"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
    }

    #endregion

    #region CreateConsultationDto Tests

    [Fact]
    public void CreateConsultationDto_WithValidData_PassesValidation()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "120/80",
            Pulse = 75,
            Complaints = "Headache and fever",
            Diagnosis = "Common cold"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void CreateConsultationDto_WithTemperatureBelowMinimum_FailsValidation()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 25m, // Below minimum 30
            BloodPressure = "120/80",
            Pulse = 75,
            Complaints = "Headache",
            Diagnosis = "Common cold"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
        Assert.Single(results.Where(r => r.MemberNames.Contains(nameof(CreateConsultationDto.Temperature))));
    }

    [Fact]
    public void CreateConsultationDto_WithTemperatureAboveMaximum_FailsValidation()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 50m, // Above maximum 45
            BloodPressure = "120/80",
            Pulse = 75,
            Complaints = "Headache",
            Diagnosis = "Common cold"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
        Assert.Single(results.Where(r => r.MemberNames.Contains(nameof(CreateConsultationDto.Temperature))));
    }

    [Fact]
    public void CreateConsultationDto_WithPulseBelowMinimum_FailsValidation()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "120/80",
            Pulse = 30, // Below minimum 40
            Complaints = "Headache",
            Diagnosis = "Common cold"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
        Assert.Single(results.Where(r => r.MemberNames.Contains(nameof(CreateConsultationDto.Pulse))));
    }

    [Fact]
    public void CreateConsultationDto_WithPulseAboveMaximum_FailsValidation()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "120/80",
            Pulse = 250, // Above maximum 200
            Complaints = "Headache",
            Diagnosis = "Common cold"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
        Assert.Single(results.Where(r => r.MemberNames.Contains(nameof(CreateConsultationDto.Pulse))));
    }

    [Fact]
    public void CreateConsultationDto_WithInvalidBloodPressureFormat_FailsValidation()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "invalid-format",
            Pulse = 75,
            Complaints = "Headache",
            Diagnosis = "Common cold"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
        Assert.Single(results.Where(r => r.MemberNames.Contains(nameof(CreateConsultationDto.BloodPressure))));
    }

    [Fact]
    public void CreateConsultationDto_WithEmptyComplaints_FailsValidation()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "120/80",
            Pulse = 75,
            Complaints = "",
            Diagnosis = "Common cold"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
    }

    [Fact]
    public void CreateConsultationDto_WithEmptyDiagnosis_FailsValidation()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "120/80",
            Pulse = 75,
            Complaints = "Headache",
            Diagnosis = ""
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
    }

    #endregion

    #region CreateMedicationDto Tests

    [Fact]
    public void CreateMedicationDto_WithValidData_PassesValidation()
    {
        // Arrange
        var dto = new CreateMedicationDto
        {
            Name = "Aspirin",
            Dosage = "500mg",
            Frequency = "Twice daily",
            Duration = 7,
            Instructions = "Take with water after meals"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void CreateMedicationDto_WithEmptyName_FailsValidation()
    {
        // Arrange
        var dto = new CreateMedicationDto
        {
            Name = "",
            Dosage = "500mg",
            Frequency = "Twice daily",
            Duration = 7,
            Instructions = "Take with water"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
    }

    [Fact]
    public void CreateMedicationDto_WithNegativeDuration_FailsValidation()
    {
        // Arrange
        var dto = new CreateMedicationDto
        {
            Name = "Aspirin",
            Dosage = "500mg",
            Frequency = "Twice daily",
            Duration = -1,
            Instructions = "Take with water"
        };

        // Act
        var results = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(results);
    }

    #endregion
}
