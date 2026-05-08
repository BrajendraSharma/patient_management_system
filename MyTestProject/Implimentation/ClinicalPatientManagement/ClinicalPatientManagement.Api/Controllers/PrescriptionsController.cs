using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalPatientManagement.Api.Controllers;

/// <summary>
/// REST API controller for prescription management
/// Step 10: Prescription Generation
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;
    private readonly ILogger<PrescriptionsController> _logger;

    public PrescriptionsController(IPrescriptionService prescriptionService, ILogger<PrescriptionsController> logger)
    {
        _prescriptionService = prescriptionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all prescriptions
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetAll()
    {
        _logger.LogInformation("Fetching all prescriptions");
        var prescriptions = await _prescriptionService.GetAllAsync();
        return Ok(prescriptions);
    }

    /// <summary>
    /// Get prescription by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PrescriptionDto>> GetById(int id)
    {
        _logger.LogInformation("Fetching prescription with ID: {PrescriptionId}", id);
        var prescription = await _prescriptionService.GetByIdAsync(id);
        
        if (prescription == null)
        {
            _logger.LogWarning("Prescription not found with ID: {PrescriptionId}", id);
            return NotFound(new { message = $"Prescription with ID {id} not found" });
        }

        return Ok(prescription);
    }

    /// <summary>
    /// Get prescription by consultation ID
    /// </summary>
    [HttpGet("consultation/{consultationId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PrescriptionDto>> GetByConsultationId(int consultationId)
    {
        _logger.LogInformation("Fetching prescription for consultation ID: {ConsultationId}", consultationId);
        var prescription = await _prescriptionService.GetByConsultationIdAsync(consultationId);
        
        if (prescription == null)
        {
            _logger.LogWarning("Prescription not found for consultation ID: {ConsultationId}", consultationId);
            return NotFound(new { message = $"Prescription for consultation {consultationId} not found" });
        }

        return Ok(prescription);
    }

    /// <summary>
    /// Get all prescriptions for a patient
    /// </summary>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetByPatientId(int patientId)
    {
        _logger.LogInformation("Fetching prescriptions for patient ID: {PatientId}", patientId);
        var prescriptions = await _prescriptionService.GetByPatientIdAsync(patientId);
        return Ok(prescriptions);
    }

    /// <summary>
    /// Create a new prescription
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PrescriptionDto>> Create([FromBody] CreatePrescriptionDto dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid prescription creation request");
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Creating new prescription for consultation ID: {ConsultationId}", dto.ConsultationId);
            var prescription = await _prescriptionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = prescription.Id }, prescription);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Bad request: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Conflict: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update prescription medications
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PrescriptionDto>> Update(int id, [FromBody] UpdatePrescriptionDto dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid prescription update request for ID: {PrescriptionId}", id);
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Updating prescription with ID: {PrescriptionId}", id);
            var prescription = await _prescriptionService.UpdateAsync(id, dto);
            return Ok(prescription);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a prescription
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Deleting prescription with ID: {PrescriptionId}", id);
            await _prescriptionService.DeleteAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
    }
}

