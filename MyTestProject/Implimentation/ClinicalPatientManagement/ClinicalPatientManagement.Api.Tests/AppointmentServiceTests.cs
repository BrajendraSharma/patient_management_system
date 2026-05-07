using AutoMapper;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.Repositories;
using ClinicalPatientManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests;

/// <summary>
/// Helper class for creating async-compatible queryables for testing
/// </summary>
internal static class AsyncQueryableHelper
{
    /// <summary>
    /// Creates an async-compatible IQueryable wrapper
    /// </summary>
    public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source) where T : class
    {
        var list = source.ToList();
        // Create a queryable that implements IAsyncEnumerable
        return new AsyncQueryable<T>(list);
    }
}

/// <summary>
/// IQueryable implementation that also implements IAsyncEnumerable for testing
/// </summary>
internal class AsyncQueryable<T> : IQueryable<T>, IAsyncEnumerable<T> where T : class
{
    private readonly IQueryable<T> _queryable;

    public AsyncQueryable(IEnumerable<T> source)
    {
        _queryable = source.AsQueryable();
    }

    public Type ElementType => _queryable.ElementType;
    public IQueryProvider Provider => _queryable.Provider;
    public Expression Expression => _queryable.Expression;

    public IEnumerator<T> GetEnumerator() => _queryable.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _queryable.GetEnumerator();

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        foreach (var item in _queryable)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;
            yield return item;
            await Task.Delay(0, cancellationToken);
        }
    }
}

/// <summary>
/// Unit tests for AppointmentService
/// Step 7: Appointment Scheduling - Business Logic Layer Testing
/// </summary>
public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> _mockAppointmentRepository;
    private readonly Mock<IPatientRepository> _mockPatientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AppointmentService _appointmentService;

    public AppointmentServiceTests()
    {
        _mockAppointmentRepository = new Mock<IAppointmentRepository>();
        _mockPatientRepository = new Mock<IPatientRepository>();
        _mockMapper = new Mock<IMapper>();
        
        _appointmentService = new AppointmentService(
            _mockAppointmentRepository.Object,
            _mockPatientRepository.Object,
            _mockMapper.Object
        );
    }

    #region Create Tests
    
    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateAppointment()
    {
        // Arrange
        var createDto = new CreateAppointmentDto
        {
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Scheduled",
            Notes = "Test appointment"
        };

        var appointment = new Appointment
        {
            Id = 1,
            PatientId = 1,
            AppointmentDate = createDto.AppointmentDate,
            Status = createDto.Status,
            Notes = createDto.Notes
        };

        var appointmentDto = new AppointmentDto
        {
            Id = 1,
            PatientId = 1,
            AppointmentDate = createDto.AppointmentDate,
            Status = createDto.Status,
            Notes = createDto.Notes
        };

        _mockMapper.Setup(m => m.Map<Appointment>(createDto)).Returns(appointment);
        _mockAppointmentRepository.Setup(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>())).ReturnsAsync(appointment);
        _mockPatientRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockAppointmentRepository.Setup(r => r.HasConflictAsync(1, createDto.AppointmentDate, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockMapper.Setup(m => m.Map<AppointmentDto>(appointment)).Returns(appointmentDto);

        // Act
        var result = await _appointmentService.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.PatientId);
        _mockAppointmentRepository.Verify(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateAppointmentDto
        {
            PatientId = -1,  // Invalid
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Scheduled",
            Notes = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _appointmentService.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_WithPastDate_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateAppointmentDto
        {
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(-1),  // Past date
            Status = "Scheduled",
            Notes = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _appointmentService.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_WithNonexistentPatient_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateAppointmentDto
        {
            PatientId = 999,  // Non-existent
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Scheduled",
            Notes = ""
        };

        _mockPatientRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _appointmentService.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_WithConflictingAppointment_ShouldThrowException()
    {
        // Arrange
        var appointmentDate = DateTime.UtcNow.AddDays(1);
        var createDto = new CreateAppointmentDto
        {
            PatientId = 1,
            AppointmentDate = appointmentDate,
            Status = "Scheduled",
            Notes = ""
        };

        _mockPatientRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockAppointmentRepository.Setup(r => r.HasConflictAsync(1, appointmentDate, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _appointmentService.CreateAsync(createDto));
    }

    #endregion

    #region Get Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnAppointment()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 1,
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Scheduled"
        };

        var appointmentDto = new AppointmentDto
        {
            Id = 1,
            PatientId = 1,
            AppointmentDate = appointment.AppointmentDate,
            Status = "Scheduled"
        };

        _mockAppointmentRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(appointment);
        _mockMapper.Setup(m => m.Map<AppointmentDto>(appointment)).Returns(appointmentDto);

        // Act
        var result = await _appointmentService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockAppointmentRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Appointment?)null);

        // Act
        var result = await _appointmentService.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByPatientIdAsync_ShouldReturnAppointments()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new Appointment { Id = 1, PatientId = 1, AppointmentDate = DateTime.UtcNow.AddDays(1), Status = "Scheduled" },
            new Appointment { Id = 2, PatientId = 1, AppointmentDate = DateTime.UtcNow.AddDays(2), Status = "Scheduled" }
        };

        var appointmentDtos = new List<AppointmentDto>
        {
            new AppointmentDto { Id = 1, PatientId = 1, Status = "Scheduled" },
            new AppointmentDto { Id = 2, PatientId = 1, Status = "Scheduled" }
        };

        // Mock the repository to return an async-compatible queryable
        _mockAppointmentRepository.Setup(r => r.GetByPatientId(1))
            .Returns(appointments.AsAsyncQueryable());
        
        _mockMapper.Setup(m => m.Map<IEnumerable<AppointmentDto>>(It.IsAny<IEnumerable<Appointment>>()))
            .Returns(appointmentDtos);

        // Act
        var result = await _appointmentService.GetByPatientIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateAppointment()
    {
        // Arrange
        var updateDto = new UpdateAppointmentDto
        {
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(2),
            Status = "Scheduled",
            Notes = "Updated notes"
        };

        var existingAppointment = new Appointment
        {
            Id = 1,
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Scheduled"
        };

        var updatedAppointment = new Appointment
        {
            Id = 1,
            PatientId = updateDto.PatientId,
            AppointmentDate = updateDto.AppointmentDate,
            Status = updateDto.Status,
            Notes = updateDto.Notes
        };

        var appointmentDto = new AppointmentDto
        {
            Id = 1,
            PatientId = updateDto.PatientId,
            AppointmentDate = updateDto.AppointmentDate,
            Status = updateDto.Status
        };

        _mockAppointmentRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingAppointment);
        _mockAppointmentRepository.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedAppointment);
        _mockAppointmentRepository.Setup(r => r.HasConflictAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockMapper.Setup(m => m.Map<AppointmentDto>(updatedAppointment)).Returns(appointmentDto);

        // Act
        var result = await _appointmentService.UpdateAsync(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        _mockAppointmentRepository.Verify(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteAppointment()
    {
        // Arrange
        _mockAppointmentRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _appointmentService.DeleteAsync(1);

        // Assert
        Assert.True(result);
        _mockAppointmentRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        _mockAppointmentRepository.Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _appointmentService.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void ValidateAppointmentData_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var createDto = new CreateAppointmentDto
        {
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Scheduled",
            Notes = "Test"
        };

        // Act
        var result = _appointmentService.ValidateAppointmentData(createDto, out var errors);

        // Assert
        Assert.True(result);
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateAppointmentData_WithInvalidPatientId_ShouldReturnFalse()
    {
        // Arrange
        var createDto = new CreateAppointmentDto
        {
            PatientId = -1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Scheduled"
        };

        // Act
        var result = _appointmentService.ValidateAppointmentData(createDto, out var errors);

        // Assert
        Assert.False(result);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void ValidateAppointmentData_WithInvalidStatus_ShouldReturnFalse()
    {
        // Arrange
        var createDto = new CreateAppointmentDto
        {
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "InvalidStatus"
        };

        // Act
        var result = _appointmentService.ValidateAppointmentData(createDto, out var errors);

        // Assert
        Assert.False(result);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void ValidateAppointmentData_WithNullDto_ShouldReturnFalse()
    {
        // Act
        var result = _appointmentService.ValidateAppointmentData(null, out var errors);

        // Assert
        Assert.False(result);
        Assert.NotEmpty(errors);
    }

    #endregion

    #region Status Update Tests

    [Fact]
    public async Task UpdateStatusAsync_WithValidStatus_ShouldUpdateAppointmentStatus()
    {
        // Arrange
        var appointment = new Appointment
        {
            Id = 1,
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Scheduled"
        };

        var updatedAppointment = new Appointment
        {
            Id = 1,
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Completed"
        };

        var appointmentDto = new AppointmentDto
        {
            Id = 1,
            PatientId = 1,
            Status = "Completed"
        };

        _mockAppointmentRepository.Setup(r => r.UpdateStatusAsync(1, "Completed", It.IsAny<CancellationToken>())).ReturnsAsync(updatedAppointment);
        _mockMapper.Setup(m => m.Map<AppointmentDto>(updatedAppointment)).Returns(appointmentDto);

        // Act
        var result = await _appointmentService.UpdateStatusAsync(1, "Completed");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Completed", result.Status);
    }

    #endregion

    #region Conflict Check Tests

    [Fact]
    public async Task HasConflictAsync_WithConflict_ShouldReturnTrue()
    {
        // Arrange
        var appointmentDate = DateTime.UtcNow.AddDays(1);
        _mockAppointmentRepository.Setup(r => r.HasConflictAsync(1, appointmentDate, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _appointmentService.HasConflictAsync(1, appointmentDate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasConflictAsync_WithoutConflict_ShouldReturnFalse()
    {
        // Arrange
        var appointmentDate = DateTime.UtcNow.AddDays(1);
        _mockAppointmentRepository.Setup(r => r.HasConflictAsync(1, appointmentDate, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _appointmentService.HasConflictAsync(1, appointmentDate);

        // Assert
        Assert.False(result);
    }

    #endregion
}
