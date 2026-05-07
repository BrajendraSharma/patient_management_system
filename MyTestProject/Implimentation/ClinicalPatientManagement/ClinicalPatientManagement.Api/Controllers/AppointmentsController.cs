using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ClinicalPatientManagement.Api.Controllers;

/// <summary>
/// API Controller for Appointment management operations
/// Step 7: Appointment Scheduling - Interface Adapter Layer
/// All endpoints require JWT authentication (from Step 4)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger _logger;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
        _logger = Log.ForContext<AppointmentsController>();
    }

    /// <summary>
    /// Get all appointments
    /// GET /api/appointments
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentService.GetAllAsync(cancellationToken);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving appointments");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving appointments");
        }
    }

    /// <summary>
    /// Get appointment by ID
    /// GET /api/appointments/{id}
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await _appointmentService.GetByIdAsync(id, cancellationToken);
            if (appointment == null)
            {
                _logger.Warning("Appointment with ID {AppointmentId} not found", id);
                return NotFound(new { message = $"Appointment with ID {id} not found" });
            }
            return Ok(appointment);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving appointment with ID {AppointmentId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the appointment");
        }
    }

    /// <summary>
    /// Get appointments for a specific patient
    /// GET /api/appointments/patient/{patientId}
    /// </summary>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByPatientId(int patientId, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentService.GetByPatientIdAsync(patientId, cancellationToken);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving appointments for patient {PatientId}", patientId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving patient appointments");
        }
    }

    /// <summary>
    /// Get appointments for a patient within date range
    /// GET /api/appointments/patient/{patientId}/range?startDate=2024-01-01&endDate=2024-12-31
    /// </summary>
    [HttpGet("patient/{patientId}/range")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByPatientIdAndDateRange(
        int patientId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        try
        {
            if (startDate > endDate)
            {
                return BadRequest(new { message = "Start date must be before end date" });
            }

            var appointments = await _appointmentService.GetByPatientIdAndDateRangeAsync(patientId, startDate, endDate, cancellationToken);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving appointments for patient {PatientId}", patientId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving appointments");
        }
    }

    /// <summary>
    /// Get appointments for a specific date
    /// GET /api/appointments/date/{date:datetime}
    /// </summary>
    [HttpGet("date/{date:datetime}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByDate(DateTime date, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentService.GetByDateAsync(date, cancellationToken);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving appointments for date {Date}", date.Date);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving appointments");
        }
    }

    /// <summary>
    /// Get upcoming appointments for a patient
    /// GET /api/appointments/upcoming/{patientId}
    /// </summary>
    [HttpGet("upcoming/{patientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetUpcomingAppointments(int patientId, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(patientId, cancellationToken);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving upcoming appointments for patient {PatientId}", patientId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving appointments");
        }
    }

    /// <summary>
    /// Create new appointment
    /// POST /api/appointments
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AppointmentDto>> Create(CreateAppointmentDto createDto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appointment = await _appointmentService.CreateAsync(createDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Warning(ex, "Invalid appointment data or conflict");
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error creating appointment");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the appointment");
        }
    }

    /// <summary>
    /// Update existing appointment
    /// PUT /api/appointments/{id}
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AppointmentDto>> Update(int id, UpdateAppointmentDto updateDto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appointment = await _appointmentService.UpdateAsync(id, updateDto, cancellationToken);
            return Ok(appointment);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Warning(ex, "Appointment not found or validation failed");
            if (ex.Message.Contains("not found"))
                return NotFound(new { message = ex.Message });
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error updating appointment with ID {AppointmentId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the appointment");
        }
    }

    /// <summary>
    /// Update appointment status
    /// PATCH /api/appointments/{id}/status
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentDto>> UpdateStatus(int id, [FromBody] UpdateStatusDto statusDto, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(statusDto?.Status))
            {
                return BadRequest(new { message = "Status is required" });
            }

            var appointment = await _appointmentService.UpdateStatusAsync(id, statusDto.Status, cancellationToken);
            return Ok(appointment);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Warning(ex, "Appointment not found");
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.Warning(ex, "Invalid status value");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error updating appointment status");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the appointment");
        }
    }

    /// <summary>
    /// Delete appointment by ID
    /// DELETE /api/appointments/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _appointmentService.DeleteAsync(id, cancellationToken);
            if (!result)
            {
                _logger.Warning("Appointment with ID {AppointmentId} not found", id);
                return NotFound(new { message = $"Appointment with ID {id} not found" });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting appointment with ID {AppointmentId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the appointment");
        }
    }

    /// <summary>
    /// Check if patient has appointment conflict at specified date/time
    /// GET /api/appointments/conflict?patientId={patientId}&appointmentDate={date}
    /// </summary>
    [HttpGet("conflict")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<object>> CheckConflict(
        [FromQuery] int patientId,
        [FromQuery] DateTime appointmentDate,
        CancellationToken cancellationToken)
    {
        try
        {
            if (patientId <= 0)
            {
                return BadRequest(new { message = "Patient ID must be greater than 0" });
            }

            var hasConflict = await _appointmentService.HasConflictAsync(patientId, appointmentDate, cancellationToken);
            return Ok(new { hasConflict, message = hasConflict ? "Patient has a conflicting appointment" : "No conflicts found" });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error checking appointment conflict");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while checking for conflicts");
        }
    }
}

/// <summary>
/// DTO for updating appointment status via PATCH endpoint
/// </summary>
public class UpdateStatusDto
{
    public string? Status { get; set; }
}
