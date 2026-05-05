using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ClinicalPatientManagement.Api.Controllers;

/// <summary>
/// API Controller for Patient management operations
/// Step 6: Patient Management - Interface Adapter Layer
/// All endpoints require JWT authentication (from Step 4)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly ILogger _logger;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
        _logger = Log.ForContext<PatientsController>();
    }

    /// <summary>
    /// Get all patients
    /// GET /api/patients
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var patients = await _patientService.GetAllAsync(cancellationToken);
            return Ok(patients);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving patients");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving patients");
        }
    }

    /// <summary>
    /// Get patient by ID
    /// GET /api/patients/{id}
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var patient = await _patientService.GetByIdAsync(id, cancellationToken);
            if (patient == null)
            {
                _logger.Warning("Patient with ID {PatientId} not found", id);
                return NotFound(new { message = $"Patient with ID {id} not found" });
            }
            return Ok(patient);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving patient with ID {PatientId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the patient");
        }
    }

    /// <summary>
    /// Create new patient
    /// POST /api/patients
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PatientDto>> Create(CreatePatientDto createDto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patient = await _patientService.CreateAsync(createDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Warning(ex, "Invalid patient data");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error creating patient");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the patient");
        }
    }

    /// <summary>
    /// Update patient
    /// PUT /api/patients/{id}
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> Update(int id, UpdatePatientDto updateDto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patient = await _patientService.UpdateAsync(id, updateDto, cancellationToken);
            return Ok(patient);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Warning(ex, "Patient not found or invalid data for ID {PatientId}", id);
            if (ex.Message.Contains("not found"))
                return NotFound(new { message = ex.Message });
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error updating patient with ID {PatientId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the patient");
        }
    }

    /// <summary>
    /// Delete patient
    /// DELETE /api/patients/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _patientService.DeleteAsync(id, cancellationToken);
            if (!result)
            {
                _logger.Warning("Patient with ID {PatientId} not found for deletion", id);
                return NotFound(new { message = $"Patient with ID {id} not found" });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting patient with ID {PatientId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the patient");
        }
    }

    /// <summary>
    /// Search patients by name or phone
    /// GET /api/patients/search?term={searchTerm}
    /// Requirement Step 8: Enhance patient search
    /// </summary>
    [HttpGet("search/{searchTerm}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<PatientDto>>> Search(string searchTerm, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "Search term cannot be empty" });
            }

            var patients = await _patientService.SearchAsync(searchTerm, cancellationToken);
            return Ok(patients);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error searching patients with term: {SearchTerm}", searchTerm);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching patients");
        }
    }
}
