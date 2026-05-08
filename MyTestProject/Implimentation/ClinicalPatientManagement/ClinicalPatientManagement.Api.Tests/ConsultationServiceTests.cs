using AutoMapper;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.Repositories;
using ClinicalPatientManagement.Api.Services;
using Moq;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests;

/// <summary>
/// Unit tests for ConsultationService
/// Step 9: Implement Consultation Creation - Testing business logic layer
/// Coverage: >80% of service methods and validation logic
/// </summary>
public class ConsultationServiceTests
{
    private readonly Mock<IConsultationRepository> _repositoryMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ConsultationService _service;

    public ConsultationServiceTests()
    {
        _repositoryMock = new Mock<IConsultationRepository>();
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new ConsultationService(_repositoryMock.Object, _appointmentRepositoryMock.Object, _mapperMock.Object);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateConsultation()
    {
        // Arrange
        var createDto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "120/80",
            Pulse = 72,
            Complaints = "Headache and fever",
            Diagnosis = "Common cold"
        };
        var consultation = new Consultation 
        { 
            Id = 1, 
            AppointmentId = 1, 
            Temperature = 37.5m, 
            BloodPressure = "120/80", 
            Pulse = 72,
            Complaints = "Headache and fever",
            Diagnosis = "Common cold"
        };
        var consultationDto = new ConsultationDto 
        { 
            Id = 1, 
            AppointmentId = 1, 
            Temperature = 37.5m, 
            BloodPressure = "120/80", 
            Pulse = 72,
            Complaints = "Headache and fever",
            Diagnosis = "Common cold"
        };

        _appointmentRepositoryMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.ExistsByAppointmentIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<Consultation>(createDto)).Returns(consultation);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Consultation>(), It.IsAny<CancellationToken>())).ReturnsAsync(consultation);
        _mapperMock.Setup(m => m.Map<ConsultationDto>(consultation)).Returns(consultationDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(37.5m, result.Temperature);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Consultation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidTemperature_ShouldThrow()
    {
        // Arrange
        var createDto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 50m, // Out of range
            BloodPressure = "120/80",
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_WithInvalidBloodPressureFormat_ShouldThrow()
    {
        // Arrange
        var createDto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "invalid-format",
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_WithInvalidPulse_ShouldThrow()
    {
        // Arrange
        var createDto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "120/80",
            Pulse = 250, // Out of range
            Complaints = "Headache",
            Diagnosis = "Cold"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_WithNonexistentAppointment_ShouldThrow()
    {
        // Arrange
        var createDto = new CreateConsultationDto
        {
            AppointmentId = 999,
            Temperature = 37.5m,
            BloodPressure = "120/80",
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };

        _appointmentRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_WithExistingConsultation_ShouldThrow()
    {
        // Arrange
        var createDto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "120/80",
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };

        _appointmentRepositoryMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.ExistsByAppointmentIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnConsultation()
    {
        // Arrange
        int consultationId = 1;
        var consultation = new Consultation 
        { 
            Id = consultationId, 
            AppointmentId = 1, 
            Temperature = 37.5m, 
            BloodPressure = "120/80", 
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };
        var consultationDto = new ConsultationDto 
        { 
            Id = consultationId, 
            AppointmentId = 1, 
            Temperature = 37.5m, 
            BloodPressure = "120/80", 
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(consultationId, It.IsAny<CancellationToken>())).ReturnsAsync(consultation);
        _mapperMock.Setup(m => m.Map<ConsultationDto>(consultation)).Returns(consultationDto);

        // Act
        var result = await _service.GetByIdAsync(consultationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(consultationId, result.Id);
        _repositoryMock.Verify(r => r.GetByIdAsync(consultationId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Consultation)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateConsultation()
    {
        // Arrange
        int consultationId = 1;
        var updateDto = new UpdateConsultationDto
        {
            Temperature = 38.5m,
            BloodPressure = "130/90",
            Pulse = 80,
            Complaints = "High fever",
            Diagnosis = "Influenza"
        };
        var existingConsultation = new Consultation 
        { 
            Id = consultationId, 
            AppointmentId = 1, 
            Temperature = 37.5m, 
            BloodPressure = "120/80", 
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };
        var updatedConsultation = new Consultation 
        { 
            Id = consultationId, 
            AppointmentId = 1, 
            Temperature = 38.5m, 
            BloodPressure = "130/90", 
            Pulse = 80,
            Complaints = "High fever",
            Diagnosis = "Influenza"
        };
        var consultationDto = new ConsultationDto 
        { 
            Id = consultationId, 
            AppointmentId = 1, 
            Temperature = 38.5m, 
            BloodPressure = "130/90", 
            Pulse = 80,
            Complaints = "High fever",
            Diagnosis = "Influenza"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(consultationId, It.IsAny<CancellationToken>())).ReturnsAsync(existingConsultation);
        _mapperMock.Setup(m => m.Map(updateDto, existingConsultation)).Returns(updatedConsultation);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Consultation>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedConsultation);
        _mapperMock.Setup(m => m.Map<ConsultationDto>(updatedConsultation)).Returns(consultationDto);

        // Act
        var result = await _service.UpdateAsync(consultationId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(consultationId, result.Id);
        Assert.Equal(38.5m, result.Temperature);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Consultation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonexistentId_ShouldThrow()
    {
        // Arrange
        var updateDto = new UpdateConsultationDto 
        { 
            Temperature = 37.5m, 
            BloodPressure = "120/80", 
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Consultation)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteConsultation()
    {
        // Arrange
        int consultationId = 1;
        _repositoryMock.Setup(r => r.DeleteAsync(consultationId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(consultationId);

        // Assert
        Assert.True(result);
        _repositoryMock.Verify(r => r.DeleteAsync(consultationId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region ValidateConsultationData Tests

    [Fact]
    public void ValidateConsultationData_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "120/80",
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };

        // Act
        var isValid = _service.ValidateConsultationData(dto, out var errors);

        // Assert
        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateConsultationData_WithInvalidTemperature_ShouldReturnFalse()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 50m, // Out of range
            BloodPressure = "120/80",
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };

        // Act
        var isValid = _service.ValidateConsultationData(dto, out var errors);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Temperature must be between 30 and 45 degrees Celsius", errors);
    }

    [Fact]
    public void ValidateConsultationData_WithInvalidBloodPressure_ShouldReturnFalse()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "invalid",
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };

        // Act
        var isValid = _service.ValidateConsultationData(dto, out var errors);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Blood pressure must be in format XXX/XXX (e.g., 120/80)", errors);
    }

    [Fact]
    public void ValidateConsultationData_WithMissingComplaints_ShouldReturnFalse()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "120/80",
            Pulse = 72,
            Complaints = "", // Empty
            Diagnosis = "Cold"
        };

        // Act
        var isValid = _service.ValidateConsultationData(dto, out var errors);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Complaints are required", errors);
    }

    [Fact]
    public void ValidateConsultationData_WithMissingDiagnosis_ShouldReturnFalse()
    {
        // Arrange
        var dto = new CreateConsultationDto
        {
            AppointmentId = 1,
            Temperature = 37.5m,
            BloodPressure = "120/80",
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "" // Empty
        };

        // Act
        var isValid = _service.ValidateConsultationData(dto, out var errors);

        // Assert
        Assert.False(isValid);
        Assert.Contains("Diagnosis is required", errors);
    }

    #endregion

    #region GetByAppointmentIdAsync Tests

    [Fact]
    public async Task GetByAppointmentIdAsync_WithValidAppointmentId_ShouldReturnConsultation()
    {
        // Arrange
        int appointmentId = 1;
        var consultation = new Consultation 
        { 
            Id = 1, 
            AppointmentId = appointmentId, 
            Temperature = 37.5m, 
            BloodPressure = "120/80", 
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };
        var consultationDto = new ConsultationDto 
        { 
            Id = 1, 
            AppointmentId = appointmentId, 
            Temperature = 37.5m, 
            BloodPressure = "120/80", 
            Pulse = 72,
            Complaints = "Headache",
            Diagnosis = "Cold"
        };

        _repositoryMock.Setup(r => r.GetByAppointmentIdAsync(appointmentId, It.IsAny<CancellationToken>())).ReturnsAsync(consultation);
        _mapperMock.Setup(m => m.Map<ConsultationDto>(consultation)).Returns(consultationDto);

        // Act
        var result = await _service.GetByAppointmentIdAsync(appointmentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(appointmentId, result.AppointmentId);
    }

    [Fact]
    public async Task GetByAppointmentIdAsync_WithNonexistentAppointment_ShouldReturnNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByAppointmentIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Consultation)null);

        // Act
        var result = await _service.GetByAppointmentIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetByPatientIdAsync Tests

    [Fact]
    public async Task GetByPatientIdAsync_WithValidPatientId_ShouldReturnConsultations()
    {
        // Arrange
        int patientId = 1;
        var consultations = new List<Consultation>
        {
            new Consultation 
            { 
                Id = 1, 
                AppointmentId = 1, 
                Temperature = 37.5m, 
                BloodPressure = "120/80", 
                Pulse = 72,
                Complaints = "Headache",
                Diagnosis = "Cold"
            },
            new Consultation 
            { 
                Id = 2, 
                AppointmentId = 2, 
                Temperature = 38.5m, 
                BloodPressure = "130/90", 
                Pulse = 80,
                Complaints = "Fever",
                Diagnosis = "Flu"
            }
        };
        var consultationDtos = new List<ConsultationDto>
        {
            new ConsultationDto { Id = 1, AppointmentId = 1, Temperature = 37.5m, BloodPressure = "120/80", Pulse = 72, Complaints = "Headache", Diagnosis = "Cold" },
            new ConsultationDto { Id = 2, AppointmentId = 2, Temperature = 38.5m, BloodPressure = "130/90", Pulse = 80, Complaints = "Fever", Diagnosis = "Flu" }
        };

        _repositoryMock.Setup(r => r.GetByPatientIdAsync(patientId, It.IsAny<CancellationToken>())).ReturnsAsync(consultations);
        _mapperMock.Setup(m => m.Map<IEnumerable<ConsultationDto>>(consultations)).Returns(consultationDtos);

        // Act
        var result = await _service.GetByPatientIdAsync(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    #endregion

    #region ExistsAsync Tests

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        int consultationId = 1;
        _repositoryMock.Setup(r => r.ExistsAsync(consultationId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _service.ExistsAsync(consultationId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _service.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    #endregion
}
