using Xunit;
using Moq;
using ClinicalPatientManagement.Api.Services;
using ClinicalPatientManagement.Api.Repositories;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.DTOs;
using AutoMapper;

namespace ClinicalPatientManagement.Api.Tests;

/// <summary>
/// Unit tests for ExportService
/// Step 13: Add data export
/// </summary>
public class ExportServiceTests
{
    private readonly Mock<IPatientRepository> _patientRepoMock;
    private readonly Mock<IConsultationRepository> _consultationRepoMock;
    private readonly Mock<IPrescriptionRepository> _prescriptionRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ExportService _service;

    public ExportServiceTests()
    {
        _patientRepoMock = new Mock<IPatientRepository>();
        _consultationRepoMock = new Mock<IConsultationRepository>();
        _prescriptionRepoMock = new Mock<IPrescriptionRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new ExportService(
            _patientRepoMock.Object,
            _consultationRepoMock.Object,
            _prescriptionRepoMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task ExportDataAsync_WithPatientDataType_ReturnsExcelFile()
    {
        // Arrange
        var request = new ExportRequest
        {
            Format = "Excel",
            DataType = "PatientData"
        };

        var patients = new List<Patient>
        {
            new Patient
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890",
                Email = "john@example.com",
                Gender = "Male",
                DateOfBirth = new DateTime(1994, 5, 15)
            }
        };

        _patientRepoMock.Setup(r => r.GetAll()).Returns(patients.AsQueryable());

        // Act
        var result = await _service.ExportDataAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Completed", result.Status);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.MimeType);
        Assert.Contains("PatientData_", result.FileName);
        Assert.Equal(1, result.RecordCount);
    }

    [Fact]
    public async Task ExportDataAsync_WithVisitHistoryType_RequiresPatientId()
    {
        // Arrange
        var request = new ExportRequest
        {
            Format = "Excel",
            DataType = "VisitHistory"
            // No PatientId
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ExportDataAsync(request));
    }

    [Fact]
    public async Task ExportDataAsync_WithInvalidFormat_ThrowsException()
    {
        // Arrange
        var request = new ExportRequest
        {
            Format = "InvalidFormat",
            DataType = "PatientData"
        };

        var patients = new List<Patient>();
        _patientRepoMock.Setup(r => r.GetAll()).Returns(patients.AsQueryable());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ExportDataAsync(request));
    }

    [Fact]
    public async Task GetPatientsForExportAsync_ReturnsFormattedPatientData()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new Patient
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890",
                Email = "john@example.com",
                Gender = "Male",
                DateOfBirth = new DateTime(1994, 5, 15)
            },
            new Patient
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Phone = "0987654321",
                Email = "jane@example.com",
                Gender = "Female",
                DateOfBirth = new DateTime(1996, 7, 20)
            }
        };

        _patientRepoMock.Setup(r => r.GetAll()).Returns(patients.AsQueryable());

        // Act
        var result = await _service.GetPatientsForExportAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        // Results should be ordered by FullName, so Jane comes before John alphabetically
        Assert.Equal("Jane Smith", result[0].FullName);
        Assert.Equal("20-07-1996", result[0].DateOfBirth); // DD-MM-YYYY format
        Assert.Equal("John Doe", result[1].FullName);
        Assert.Equal("15-05-1994", result[1].DateOfBirth); // DD-MM-YYYY format
    }

    [Fact]
    public async Task GetPatientVisitsForExportAsync_WithValidPatientId_ReturnsFormattedVisitData()
    {
        // Arrange
        int patientId = 1;
        var patient = new Patient 
        { 
            Id = patientId, 
            FirstName = "John",
            LastName = "Doe"
        };
        var appointment = new Appointment { Id = 1, PatientId = patientId, Patient = patient };
        
        var consultations = new List<Consultation>
        {
            new Consultation
            {
                Id = 1,
                AppointmentId = appointment.Id,
                Appointment = appointment,
                Temperature = 37.5m,
                BloodPressure = "120/80",
                Pulse = 72,
                Complaints = "Fever",
                Diagnosis = "Common cold",
                CreatedAt = new DateTime(2024, 5, 10, 10, 30, 0)
            }
        };

        _consultationRepoMock
            .Setup(r => r.GetByPatientIdAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(consultations);

        _prescriptionRepoMock
            .Setup(r => r.GetByConsultationIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Prescription)null);

        // Act
        var result = await _service.GetPatientVisitsForExportAsync(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("John Doe", result[0].PatientName);
        Assert.Equal("10-05-2024", result[0].ConsultationDate); // DD-MM-YYYY format
        Assert.Equal(37.5m, result[0].Temperature);
        Assert.Equal("120/80", result[0].BloodPressure);
    }

    [Fact]
    public async Task GetPatientVisitsForExportAsync_WithDateFilter_AppliesDateRange()
    {
        // Arrange
        int patientId = 1;
        var startDate = new DateTime(2024, 5, 1);
        var endDate = new DateTime(2024, 5, 31);

        var consultations = new List<Consultation>
        {
            new Consultation
            {
                Id = 1,
                Temperature = 37.5m,
                CreatedAt = new DateTime(2024, 5, 15) // Within range
            },
            new Consultation
            {
                Id = 2,
                Temperature = 36.0m,
                CreatedAt = new DateTime(2024, 4, 15) // Outside range
            }
        };

        _consultationRepoMock
            .Setup(r => r.GetByPatientIdAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(consultations);

        _prescriptionRepoMock
            .Setup(r => r.GetByConsultationIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Prescription)null);

        // Act
        var result = await _service.GetPatientVisitsForExportAsync(patientId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1, result[0].ConsultationId); // Only first consultation within range
    }

    [Fact]
    public async Task GetPrescriptionsForExportAsync_ReturnsFormattedPrescriptionData()
    {
        // Arrange
        var patient = new Patient 
        { 
            Id = 1, 
            FirstName = "John",
            LastName = "Doe"
        };
        var appointment = new Appointment { Id = 1, PatientId = 1, Patient = patient };
        var consultation = new Consultation { Id = 1, Appointment = appointment };

        var medications = new List<Medication>
        {
            new Medication
            {
                Name = "Aspirin",
                Dosage = "500mg",
                Frequency = "3x daily",
                Duration = 7,
                Instructions = "Take with water"
            }
        };

        var prescriptions = new List<Prescription>
        {
            new Prescription
            {
                Id = 1,
                ConsultationId = 1,
                Consultation = consultation,
                PrescriptionDate = new DateTime(2024, 5, 10),
                Medications = medications
            }
        };

        _prescriptionRepoMock.Setup(r => r.GetAll()).Returns(prescriptions.AsQueryable());

        // Act
        var result = await _service.GetPrescriptionsForExportAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("John Doe", result[0].PatientName);
        Assert.Equal("10-05-2024", result[0].PrescriptionDate); // DD-MM-YYYY format
        Assert.Equal("Aspirin", result[0].MedicationName);
        Assert.Equal("500mg", result[0].Dosage);
    }

    [Fact]
    public async Task GetPrescriptionsForExportAsync_WithPatientIdFilter_FiltersCorrectly()
    {
        // Arrange
        int patientId = 1;
        var patient1 = new Patient 
        { 
            Id = 1, 
            FirstName = "John",
            LastName = "Doe"
        };
        var patient2 = new Patient 
        { 
            Id = 2, 
            FirstName = "Jane",
            LastName = "Smith"
        };
        
        var medications = new List<Medication>
        {
            new Medication { Name = "Aspirin", Dosage = "500mg" }
        };

        var prescriptions = new List<Prescription>
        {
            new Prescription
            {
                Id = 1,
                Consultation = new Consultation
                {
                    Appointment = new Appointment { PatientId = 1, Patient = patient1 }
                },
                Medications = medications
            },
            new Prescription
            {
                Id = 2,
                Consultation = new Consultation
                {
                    Appointment = new Appointment { PatientId = 2, Patient = patient2 }
                },
                Medications = medications
            }
        };

        _prescriptionRepoMock.Setup(r => r.GetAll()).Returns(prescriptions.AsQueryable());

        // Act
        var result = await _service.GetPrescriptionsForExportAsync(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("John Doe", result[0].PatientName);
    }

    [Fact]
    public async Task ExportDataAsync_WithNullRequest_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ExportDataAsync(null));
    }

    [Fact]
    public async Task ExportDataAsync_WithEmptyData_ReturnsValidResponse()
    {
        // Arrange
        var request = new ExportRequest
        {
            Format = "Excel",
            DataType = "PatientData"
        };

        var emptyPatients = new List<Patient>();
        _patientRepoMock.Setup(r => r.GetAll()).Returns(emptyPatients.AsQueryable());

        // Act
        var result = await _service.ExportDataAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Completed", result.Status);
        Assert.Equal(0, result.RecordCount);
        Assert.Empty(result.FileContent);
    }

    [Fact]
    public async Task ExportDataAsync_ValidateFileNameGeneration()
    {
        // Arrange
        var request = new ExportRequest
        {
            Format = "PDF",
            DataType = "PatientData"
        };

        var patients = new List<Patient>
        {
            new Patient { Id = 1, FirstName = "John", LastName = "Doe" }
        };

        _patientRepoMock.Setup(r => r.GetAll()).Returns(patients.AsQueryable());

        // Act
        var result = await _service.ExportDataAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("PatientData_", result.FileName);
        Assert.EndsWith(".pdf", result.FileName);
        Assert.Equal("application/pdf", result.MimeType);
    }
}
