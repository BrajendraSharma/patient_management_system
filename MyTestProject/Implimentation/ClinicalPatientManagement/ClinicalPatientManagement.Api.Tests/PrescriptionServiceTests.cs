using AutoMapper;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Mappings;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.Repositories;
using ClinicalPatientManagement.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests;

public class PrescriptionServiceTests
{
    private readonly IMapper _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
    private readonly ILogger<PrescriptionService> _logger = Mock.Of<ILogger<PrescriptionService>>();

    private IPrescriptionService CreateService(IPrescriptionRepository prescriptionRepo, IConsultationRepository consultationRepo)
        => new PrescriptionService(prescriptionRepo, consultationRepo, _mapper, _logger);

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreatePrescription()
    {
        var prescrRepoMock = new Mock<IPrescriptionRepository>();
        var consultRepoMock = new Mock<IConsultationRepository>();
        
        var createDto = new CreatePrescriptionDto { ConsultationId = 1, Medications = new List<CreateMedicationDto> { new() { Name = "Aspirin", Dosage = "500mg", Frequency = "Twice daily", Duration = 7, Instructions = "Take with water" } } };
        var consultation = new Consultation { Id = 1, AppointmentId = 1, Temperature = 37.5m, BloodPressure = "120/80", Pulse = 75, Complaints = "Headache", Diagnosis = "Common headache" };
        var createdPrescription = new Prescription { Id = 1, ConsultationId = 1, PrescriptionDate = DateTime.UtcNow, Medications = new List<Medication> { new() { Id = 1, PrescriptionId = 1, Name = "Aspirin", Dosage = "500mg", Frequency = "Twice daily", Duration = 7, Instructions = "Take with water" } } };

        consultRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(consultation);
        prescrRepoMock.Setup(r => r.ExistsByConsultationIdAsync(It.IsAny<int>())).ReturnsAsync(false);
        prescrRepoMock.Setup(r => r.CreateAsync(It.IsAny<Prescription>())).ReturnsAsync(createdPrescription);

        var service = CreateService(prescrRepoMock.Object, consultRepoMock.Object);
        var result = await service.CreateAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Single(result.Medications);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidConsultationId_ShouldThrowArgumentException()
    {
        var prescrRepoMock = new Mock<IPrescriptionRepository>();
        var consultRepoMock = new Mock<IConsultationRepository>();
        var createDto = new CreatePrescriptionDto { ConsultationId = 999, Medications = new List<CreateMedicationDto> { new() { Name = "Aspirin", Dosage = "500mg", Frequency = "Twice daily", Duration = 7 } } };

        consultRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Consultation)null);

        var service = CreateService(prescrRepoMock.Object, consultRepoMock.Object);
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(createDto));
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnPrescription()
    {
        var prescrRepoMock = new Mock<IPrescriptionRepository>();
        var consultRepoMock = new Mock<IConsultationRepository>();
        var prescription = new Prescription { Id = 1, ConsultationId = 1, PrescriptionDate = DateTime.UtcNow };

        prescrRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(prescription);

        var service = CreateService(prescrRepoMock.Object, consultRepoMock.Object);
        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var prescrRepoMock = new Mock<IPrescriptionRepository>();
        var consultRepoMock = new Mock<IConsultationRepository>();

        prescrRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Prescription)null);

        var service = CreateService(prescrRepoMock.Object, consultRepoMock.Object);
        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdatePrescription()
    {
        var prescrRepoMock = new Mock<IPrescriptionRepository>();
        var consultRepoMock = new Mock<IConsultationRepository>();
        var updateDto = new UpdatePrescriptionDto { Medications = new List<UpdateMedicationDto> { new() { Name = "Ibuprofen", Dosage = "200mg", Frequency = "Three times daily", Duration = 5, Instructions = "Take with food" } } };
        var existingPrescription = new Prescription { Id = 1, ConsultationId = 1, PrescriptionDate = DateTime.UtcNow, Medications = new List<Medication>() };
        var updatedPrescription = new Prescription { Id = 1, ConsultationId = 1, PrescriptionDate = DateTime.UtcNow, Medications = new List<Medication> { new() { Id = 1, PrescriptionId = 1, Name = "Ibuprofen", Dosage = "200mg", Frequency = "Three times daily", Duration = 5, Instructions = "Take with food" } } };

        prescrRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingPrescription);
        prescrRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Prescription>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedPrescription);
        prescrRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var service = CreateService(prescrRepoMock.Object, consultRepoMock.Object);
        var result = await service.UpdateAsync(1, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Ibuprofen", result.Medications.First().Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeletePrescription()
    {
        var prescrRepoMock = new Mock<IPrescriptionRepository>();
        var consultRepoMock = new Mock<IConsultationRepository>();
        var prescription = new Prescription { Id = 1, ConsultationId = 1, PrescriptionDate = DateTime.UtcNow };

        prescrRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(prescription);
        prescrRepoMock.Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        prescrRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var service = CreateService(prescrRepoMock.Object, consultRepoMock.Object);
        await service.DeleteAsync(1);

        prescrRepoMock.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByConsultationIdAsync_ShouldReturnPrescription()
    {
        var prescrRepoMock = new Mock<IPrescriptionRepository>();
        var consultRepoMock = new Mock<IConsultationRepository>();
        var prescription = new Prescription { Id = 1, ConsultationId = 1, PrescriptionDate = DateTime.UtcNow };

        prescrRepoMock.Setup(r => r.GetByConsultationIdAsync(It.IsAny<int>())).ReturnsAsync(prescription);

        var service = CreateService(prescrRepoMock.Object, consultRepoMock.Object);
        var result = await service.GetByConsultationIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.ConsultationId);
    }

    [Fact]
    public async Task GetByPatientIdAsync_ShouldReturnPrescriptions()
    {
        var prescrRepoMock = new Mock<IPrescriptionRepository>();
        var consultRepoMock = new Mock<IConsultationRepository>();
        var prescriptions = new List<Prescription> { new() { Id = 1, ConsultationId = 1, PrescriptionDate = DateTime.UtcNow } };

        prescrRepoMock.Setup(r => r.GetByPatientIdAsync(It.IsAny<int>())).ReturnsAsync(prescriptions);

        var service = CreateService(prescrRepoMock.Object, consultRepoMock.Object);
        var result = await service.GetByPatientIdAsync(1);

        Assert.NotEmpty(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyMedications_ShouldThrowArgumentException()
    {
        var prescrRepoMock = new Mock<IPrescriptionRepository>();
        var consultRepoMock = new Mock<IConsultationRepository>();
        var createDto = new CreatePrescriptionDto { ConsultationId = 1, Medications = new List<CreateMedicationDto>() };

        var service = CreateService(prescrRepoMock.Object, consultRepoMock.Object);
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(createDto));
    }

    [Fact]
    public async Task ExistsAsync_WithExistingPrescription_ShouldReturnTrue()
    {
        var prescrRepoMock = new Mock<IPrescriptionRepository>();
        var consultRepoMock = new Mock<IConsultationRepository>();

        prescrRepoMock.Setup(r => r.ExistsByConsultationIdAsync(It.IsAny<int>())).ReturnsAsync(true);

        var service = CreateService(prescrRepoMock.Object, consultRepoMock.Object);
        var result = await service.ExistsAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingPrescription_ShouldReturnFalse()
    {
        var prescrRepoMock = new Mock<IPrescriptionRepository>();
        var consultRepoMock = new Mock<IConsultationRepository>();

        prescrRepoMock.Setup(r => r.ExistsByConsultationIdAsync(It.IsAny<int>())).ReturnsAsync(false);

        var service = CreateService(prescrRepoMock.Object, consultRepoMock.Object);
        var result = await service.ExistsAsync(999);

        Assert.False(result);
    }
}
