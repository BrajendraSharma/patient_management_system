using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ClinicalPatientManagement.Api.Controllers;

/// <summary>
/// API Controller for Consultation management operations
/// Step 9: Implement Consultation Creation - Interface Adapter Layer
/// All endpoints require JWT authentication (from Step 4)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsultationsController : ControllerBase
{
    private readonly IConsultationService _consultationService;
    private readonly ILogger _logger;

    public ConsultationsController(IConsultationService consultationService)
    {
        _consultationService = consultationService ?? throw new ArgumentNullException(nameof(consultationService));
        _logger = Log.ForContext<ConsultationsController>();
    }

    /// <summary>
    /// Get all consultations
    /// GET /api/consultations
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var consultations = await _consultationService.GetAllAsync(cancellationToken);
            return Ok(consultations);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving consultations");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving consultations");
        }
    }

    /// <summary>
    /// Get consultation by ID
    /// GET /api/consultations/{id}
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsultationDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var consultation = await _consultationService.GetByIdAsync(id, cancellationToken);
            if (consultation == null)
            {
                _logger.Warning("Consultation with ID {ConsultationId} not found", id);
                return NotFound(new { message = $"Consultation with ID {id} not found" });
            }
            return Ok(consultation);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving consultation with ID {ConsultationId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the consultation");
        }
    }

    /// <summary>
    /// Create new consultation
    /// POST /api/consultations
    /// Step 9: Implement Consultation Creation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ConsultationDto>> Create(CreateConsultationDto createDto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var consultation = await _consultationService.CreateAsync(createDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = consultation.Id }, consultation);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Warning(ex, "Invalid consultation data");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error creating consultation");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the consultation");
        }
    }

    /// <summary>
    /// Update consultation
    /// PUT /api/consultations/{id}
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsultationDto>> Update(int id, UpdateConsultationDto updateDto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var consultation = await _consultationService.UpdateAsync(id, updateDto, cancellationToken);
            return Ok(consultation);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Warning(ex, "Consultation not found or invalid data for ID {ConsultationId}", id);
            if (ex.Message.Contains("not found"))
                return NotFound(new { message = ex.Message });
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error updating consultation with ID {ConsultationId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the consultation");
        }
    }

    /// <summary>
    /// Delete consultation
    /// DELETE /api/consultations/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _consultationService.DeleteAsync(id, cancellationToken);
            if (!result)
            {
                _logger.Warning("Consultation with ID {ConsultationId} not found for deletion", id);
                return NotFound(new { message = $"Consultation with ID {id} not found" });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting consultation with ID {ConsultationId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the consultation");
        }
    }

    /// <summary>
    /// Get consultation by appointment ID
    /// GET /api/consultations/appointment/{appointmentId}
    /// </summary>
    [HttpGet("appointment/{appointmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsultationDto>> GetByAppointmentId(int appointmentId, CancellationToken cancellationToken)
    {
        try
        {
            var consultation = await _consultationService.GetByAppointmentIdAsync(appointmentId, cancellationToken);
            if (consultation == null)
            {
                _logger.Warning("Consultation for appointment {AppointmentId} not found", appointmentId);
                return NotFound(new { message = $"Consultation for appointment {appointmentId} not found" });
            }
            return Ok(consultation);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving consultation for appointment {AppointmentId}", appointmentId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the consultation");
        }
    }

    /// <summary>
    /// Get all consultations for a patient
    /// GET /api/consultations/patient/{patientId}
    /// </summary>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetByPatientId(int patientId, CancellationToken cancellationToken)
    {
        try
        {
            var consultations = await _consultationService.GetByPatientIdAsync(patientId, cancellationToken);
            return Ok(consultations);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving consultations for patient {PatientId}", patientId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving consultations");
        }
    }

    /// <summary>
    /// Get patient consultation history with optional date filtering
    /// GET /api/consultations/history/{patientId}?startDate=2024-01-01&endDate=2024-12-31
    /// Step 12: Implement Patient History - View past visits with date filtering
    /// </summary>
    [HttpGet("history/{patientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetPatientHistory(
        int patientId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information(
                "Retrieving consultation history for patient {PatientId} from {StartDate} to {EndDate}",
                patientId,
                startDate?.ToString("yyyy-MM-dd") ?? "null",
                endDate?.ToString("yyyy-MM-dd") ?? "null");

            var history = await _consultationService.GetPatientHistoryAsync(
                patientId,
                startDate,
                endDate,
                cancellationToken);

            return Ok(history);
        }
        catch (ArgumentException ex)
        {
            _logger.Warning(ex, "Invalid date range for patient history: {PatientId}", patientId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving consultation history for patient {PatientId}", patientId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the consultation history");
        }
    }
}
