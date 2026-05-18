using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.Services;
using ClinicalPatientManagement.Api.Repositories;
using Moq;
using Xunit;
using AutoMapper;

namespace ClinicalPatientManagement.Api.Tests;

/// <summary>
/// Unit tests for patient history filtering functionality
/// Step 12: Implement Patient History - Test date filtering and consultation retrieval
/// </summary>
public class ConsultationHistoryFilteringTests
{
    private readonly Mock<IConsultationRepository> _repositoryMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly Mock<IPrescriptionService> _prescriptionServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly ConsultationService _service;

    public ConsultationHistoryFilteringTests()
    {
        _repositoryMock = new Mock<IConsultationRepository>();
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _prescriptionServiceMock = new Mock<IPrescriptionService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _cacheServiceMock = new Mock<ICacheService>();
        
        // Setup UnitOfWork to return the repository mocks
        _unitOfWorkMock.Setup(u => u.Consultations).Returns(_repositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Appointments).Returns(_appointmentRepositoryMock.Object);
        
        // Cache service defaults to null returns for cache misses
        
        _service = new ConsultationService(
            _unitOfWorkMock.Object,
            _prescriptionServiceMock.Object,
            _mapperMock.Object,
            _cacheServiceMock.Object);
    }

    #region GetPatientHistoryAsync Tests

    [Fact]
    public async Task GetPatientHistoryAsync_WithNoFilter_ShouldReturnAllConsultationsOrderedByDateDescending()
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
                Diagnosis = "Cold",
                CreatedAt = new DateTime(2024, 01, 15)
            },
            new Consultation
            {
                Id = 2,
                AppointmentId = 2,
                Temperature = 36.8m,
                BloodPressure = "118/78",
                Pulse = 70,
                Complaints = "Fever",
                Diagnosis = "Flu",
                CreatedAt = new DateTime(2024, 01, 20)
            },
            new Consultation
            {
                Id = 3,
                AppointmentId = 3,
                Temperature = 37.2m,
                BloodPressure = "122/82",
                Pulse = 74,
                Complaints = "Cough",
                Diagnosis = "Bronchitis",
                CreatedAt = new DateTime(2024, 01, 10)
            }
        };

        var expectedDtos = new List<ConsultationDto>
        {
            new ConsultationDto { Id = 2, CreatedAt = new DateTime(2024, 01, 20) },
            new ConsultationDto { Id = 1, CreatedAt = new DateTime(2024, 01, 15) },
            new ConsultationDto { Id = 3, CreatedAt = new DateTime(2024, 01, 10) }
        };

        _repositoryMock.Setup(r => r.GetByPatientIdAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(consultations);
        _mapperMock.Setup(m => m.Map<IEnumerable<ConsultationDto>>(It.IsAny<List<Consultation>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetPatientHistoryAsync(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Equal(2, result.First().Id); // Most recent first
        Assert.Equal(3, result.Last().Id);  // Oldest last
    }

    [Fact]
    public async Task GetPatientHistoryAsync_WithStartDate_ShouldFilterConsultationsFromStartDate()
    {
        // Arrange
        int patientId = 1;
        var startDate = new DateTime(2024, 01, 15);
        var consultations = new List<Consultation>
        {
            new Consultation { Id = 1, CreatedAt = new DateTime(2024, 01, 15) },
            new Consultation { Id = 2, CreatedAt = new DateTime(2024, 01, 20) },
            new Consultation { Id = 3, CreatedAt = new DateTime(2024, 01, 10) } // Before start date
        };

        var expectedDtos = new List<ConsultationDto>
        {
            new ConsultationDto { Id = 2, CreatedAt = new DateTime(2024, 01, 20) },
            new ConsultationDto { Id = 1, CreatedAt = new DateTime(2024, 01, 15) }
        };

        _repositoryMock.Setup(r => r.GetByPatientIdAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(consultations);
        _mapperMock.Setup(m => m.Map<IEnumerable<ConsultationDto>>(It.IsAny<List<Consultation>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetPatientHistoryAsync(patientId, startDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, dto => Assert.True(dto.CreatedAt >= startDate));
    }

    [Fact]
    public async Task GetPatientHistoryAsync_WithEndDate_ShouldFilterConsultationsUpToEndDate()
    {
        // Arrange
        int patientId = 1;
        var endDate = new DateTime(2024, 01, 18);
        var consultations = new List<Consultation>
        {
            new Consultation { Id = 1, CreatedAt = new DateTime(2024, 01, 15) },
            new Consultation { Id = 2, CreatedAt = new DateTime(2024, 01, 20) }, // After end date
            new Consultation { Id = 3, CreatedAt = new DateTime(2024, 01, 10) }
        };

        var expectedDtos = new List<ConsultationDto>
        {
            new ConsultationDto { Id = 1, CreatedAt = new DateTime(2024, 01, 15) },
            new ConsultationDto { Id = 3, CreatedAt = new DateTime(2024, 01, 10) }
        };

        _repositoryMock.Setup(r => r.GetByPatientIdAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(consultations);
        _mapperMock.Setup(m => m.Map<IEnumerable<ConsultationDto>>(It.IsAny<List<Consultation>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetPatientHistoryAsync(patientId, null, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, dto => Assert.True(dto.CreatedAt.Date <= endDate.Date));
    }

    [Fact]
    public async Task GetPatientHistoryAsync_WithDateRange_ShouldFilterConsultationsBetweenDates()
    {
        // Arrange
        int patientId = 1;
        var startDate = new DateTime(2024, 01, 12);
        var endDate = new DateTime(2024, 01, 18);
        var consultations = new List<Consultation>
        {
            new Consultation { Id = 1, CreatedAt = new DateTime(2024, 01, 15) }, // Within range
            new Consultation { Id = 2, CreatedAt = new DateTime(2024, 01, 20) }, // After range
            new Consultation { Id = 3, CreatedAt = new DateTime(2024, 01, 10) }  // Before range
        };

        var expectedDtos = new List<ConsultationDto>
        {
            new ConsultationDto { Id = 1, CreatedAt = new DateTime(2024, 01, 15) }
        };

        _repositoryMock.Setup(r => r.GetByPatientIdAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(consultations);
        _mapperMock.Setup(m => m.Map<IEnumerable<ConsultationDto>>(It.IsAny<List<Consultation>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetPatientHistoryAsync(patientId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
        Assert.All(result, dto => Assert.True(dto.CreatedAt >= startDate && dto.CreatedAt.Date <= endDate.Date));
    }

    [Fact]
    public async Task GetPatientHistoryAsync_WithInvalidDateRange_ShouldThrowArgumentException()
    {
        // Arrange
        int patientId = 1;
        var startDate = new DateTime(2024, 01, 20);
        var endDate = new DateTime(2024, 01, 15);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetPatientHistoryAsync(patientId, startDate, endDate));
        Assert.Contains("Start date cannot be greater than end date", ex.Message);
    }

    [Fact]
    public async Task GetPatientHistoryAsync_WithNoConsultations_ShouldReturnEmptyList()
    {
        // Arrange
        int patientId = 1;
        var consultations = new List<Consultation>();

        _repositoryMock.Setup(r => r.GetByPatientIdAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(consultations);
        _mapperMock.Setup(m => m.Map<IEnumerable<ConsultationDto>>(It.IsAny<List<Consultation>>()))
            .Returns(new List<ConsultationDto>());

        // Act
        var result = await _service.GetPatientHistoryAsync(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPatientHistoryAsync_WithMultipleConsultations_ShouldOrderByDateDescending()
    {
        // Arrange
        int patientId = 1;
        var consultations = new List<Consultation>
        {
            new Consultation { Id = 3, CreatedAt = new DateTime(2024, 01, 10, 09, 00, 00) },
            new Consultation { Id = 1, CreatedAt = new DateTime(2024, 01, 20, 14, 30, 00) },
            new Consultation { Id = 2, CreatedAt = new DateTime(2024, 01, 15, 10, 15, 00) }
        };

        var expectedDtos = new List<ConsultationDto>
        {
            new ConsultationDto { Id = 1, CreatedAt = new DateTime(2024, 01, 20, 14, 30, 00) },
            new ConsultationDto { Id = 2, CreatedAt = new DateTime(2024, 01, 15, 10, 15, 00) },
            new ConsultationDto { Id = 3, CreatedAt = new DateTime(2024, 01, 10, 09, 00, 00) }
        };

        _repositoryMock.Setup(r => r.GetByPatientIdAsync(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(consultations);
        _mapperMock.Setup(m => m.Map<IEnumerable<ConsultationDto>>(It.IsAny<List<Consultation>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetPatientHistoryAsync(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        
        var resultList = result.ToList();
        for (int i = 0; i < resultList.Count - 1; i++)
        {
            Assert.True(resultList[i].CreatedAt >= resultList[i + 1].CreatedAt, 
                "Results should be ordered by date descending");
        }
    }

    #endregion
}
