using ClinicalPatientManagement.Api.Controllers;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Services;
using Moq;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests;

/// <summary>
/// Unit tests for AppointmentsController
/// Step 7: Appointment Scheduling - Interface Adapter Layer Testing
/// </summary>
public class AppointmentsControllerTests
{
    private readonly Mock<IAppointmentService> _mockAppointmentService;
    private readonly AppointmentsController _controller;

    public AppointmentsControllerTests()
    {
        _mockAppointmentService = new Mock<IAppointmentService>();
        _controller = new AppointmentsController(_mockAppointmentService.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ShouldReturnOkWithAppointments()
    {
        // Arrange
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto { Id = 1, PatientId = 1, Status = "Scheduled" },
            new AppointmentDto { Id = 2, PatientId = 2, Status = "Completed" }
        };

        _mockAppointmentService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(appointments);

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returnedAppointments = Assert.IsAssignableFrom<IEnumerable<AppointmentDto>>(okResult.Value);
        Assert.Equal(2, returnedAppointments.Count());
    }

    [Fact]
    public async Task GetAll_WithException_ShouldReturnInternalServerError()
    {
        // Arrange
        _mockAppointmentService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        var statusResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnOkWithAppointment()
    {
        // Arrange
        var appointmentDto = new AppointmentDto { Id = 1, PatientId = 1, Status = "Scheduled" };
        _mockAppointmentService.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(appointmentDto);

        // Act
        var result = await _controller.GetById(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returnedAppointment = Assert.IsType<AppointmentDto>(okResult.Value);
        Assert.Equal(1, returnedAppointment.Id);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        _mockAppointmentService.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((AppointmentDto?)null);

        // Act
        var result = await _controller.GetById(999, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateAppointmentDto
        {
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Scheduled"
        };

        var createdAppointmentDto = new AppointmentDto
        {
            Id = 1,
            PatientId = 1,
            AppointmentDate = createDto.AppointmentDate,
            Status = "Scheduled"
        };

        _mockAppointmentService.Setup(s => s.CreateAsync(createDto, It.IsAny<CancellationToken>())).ReturnsAsync(createdAppointmentDto);

        // Act
        var result = await _controller.Create(createDto, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(AppointmentsController.GetById), createdResult.ActionName);
        Assert.Equal(1, ((AppointmentDto)createdResult.Value!).Id);
    }

    [Fact]
    public async Task Create_WithConflict_ShouldReturnConflict()
    {
        // Arrange
        var createDto = new CreateAppointmentDto
        {
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Scheduled"
        };

        _mockAppointmentService.Setup(s => s.CreateAsync(createDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Patient already has an appointment"));

        // Act
        var result = await _controller.Create(createDto, CancellationToken.None);

        // Assert
        var conflictResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ConflictObjectResult>(result.Result);
        Assert.Equal(409, conflictResult.StatusCode);
    }

    [Fact]
    public async Task Create_WithException_ShouldReturnInternalServerError()
    {
        // Arrange
        var createDto = new CreateAppointmentDto { PatientId = 1, AppointmentDate = DateTime.UtcNow.AddDays(1) };
        _mockAppointmentService.Setup(s => s.CreateAsync(createDto, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Create(createDto, CancellationToken.None);

        // Assert
        var statusResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WithValidData_ShouldReturnOkWithUpdatedAppointment()
    {
        // Arrange
        var updateDto = new UpdateAppointmentDto
        {
            PatientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(2),
            Status = "Scheduled"
        };

        var updatedAppointmentDto = new AppointmentDto
        {
            Id = 1,
            PatientId = 1,
            AppointmentDate = updateDto.AppointmentDate,
            Status = "Scheduled"
        };

        _mockAppointmentService.Setup(s => s.UpdateAsync(1, updateDto, It.IsAny<CancellationToken>())).ReturnsAsync(updatedAppointmentDto);

        // Act
        var result = await _controller.Update(1, updateDto, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returnedAppointment = Assert.IsType<AppointmentDto>(okResult.Value);
        Assert.Equal(1, returnedAppointment.Id);
    }

    [Fact]
    public async Task Update_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new UpdateAppointmentDto { PatientId = 1, AppointmentDate = DateTime.UtcNow.AddDays(1) };
        _mockAppointmentService.Setup(s => s.UpdateAsync(999, updateDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Appointment with ID 999 not found"));

        // Act
        var result = await _controller.Update(999, updateDto, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    #endregion

    #region UpdateStatus Tests

    [Fact]
    public async Task UpdateStatus_WithValidStatus_ShouldReturnOkWithUpdatedAppointment()
    {
        // Arrange
        var statusDto = new UpdateStatusDto { Status = "Completed" };
        var updatedAppointmentDto = new AppointmentDto
        {
            Id = 1,
            PatientId = 1,
            Status = "Completed"
        };

        _mockAppointmentService.Setup(s => s.UpdateStatusAsync(1, "Completed", It.IsAny<CancellationToken>())).ReturnsAsync(updatedAppointmentDto);

        // Act
        var result = await _controller.UpdateStatus(1, statusDto, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returnedAppointment = Assert.IsType<AppointmentDto>(okResult.Value);
        Assert.Equal("Completed", returnedAppointment.Status);
    }

    [Fact]
    public async Task UpdateStatus_WithInvalidStatus_ShouldReturnBadRequest()
    {
        // Arrange
        var statusDto = new UpdateStatusDto { Status = null };

        // Act
        var result = await _controller.UpdateStatus(1, statusDto, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        _mockAppointmentService.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        _mockAppointmentService.Setup(s => s.DeleteAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(999, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    #endregion

    #region GetByPatientId Tests

    [Fact]
    public async Task GetByPatientId_ShouldReturnOkWithPatientAppointments()
    {
        // Arrange
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto { Id = 1, PatientId = 1, Status = "Scheduled" },
            new AppointmentDto { Id = 2, PatientId = 1, Status = "Completed" }
        };

        _mockAppointmentService.Setup(s => s.GetByPatientIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(appointments);

        // Act
        var result = await _controller.GetByPatientId(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returnedAppointments = Assert.IsAssignableFrom<IEnumerable<AppointmentDto>>(okResult.Value);
        Assert.Equal(2, returnedAppointments.Count());
    }

    #endregion

    #region CheckConflict Tests

    [Fact]
    public async Task CheckConflict_WithConflict_ShouldReturnTrue()
    {
        // Arrange
        var appointmentDate = DateTime.UtcNow.AddDays(1);
        _mockAppointmentService.Setup(s => s.HasConflictAsync(1, appointmentDate, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _controller.CheckConflict(1, appointmentDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
        
        // Access the hasConflict property via reflection since it's an anonymous object
        var hasConflictProperty = okResult.Value.GetType().GetProperty("hasConflict");
        Assert.NotNull(hasConflictProperty);
        var conflictValue = hasConflictProperty.GetValue(okResult.Value);
        Assert.True((bool)conflictValue);
    }

    [Fact]
    public async Task CheckConflict_WithoutConflict_ShouldReturnFalse()
    {
        // Arrange
        var appointmentDate = DateTime.UtcNow.AddDays(1);
        _mockAppointmentService.Setup(s => s.HasConflictAsync(1, appointmentDate, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _controller.CheckConflict(1, appointmentDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
        
        // Access the hasConflict property via reflection since it's an anonymous object
        var hasConflictProperty = okResult.Value.GetType().GetProperty("hasConflict");
        Assert.NotNull(hasConflictProperty);
        var conflictValue = hasConflictProperty.GetValue(okResult.Value);
        Assert.False((bool)conflictValue);
    }

    #endregion
}
