using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ClinicalPatientManagement.Api.Controllers;

/// <summary>
/// Controller for data export operations
/// Step 13: Add data export endpoints for Excel and PDF formats
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;
    private readonly ILogger _logger;

    public ExportController(IExportService exportService)
    {
        _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
        _logger = Log.ForContext<ExportController>();
    }

    /// <summary>
    /// Export data to Excel or PDF format
    /// POST /api/export
    /// Step 13: Add data export
    /// </summary>
    /// <param name="request">Export request with format, data type, and filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ExportResponse with file content and metadata</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExportResponse>> ExportData(
        ExportRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.Warning("Invalid export request");
                return BadRequest(ModelState);
            }

            _logger.Information("Export request received: Format={Format}, DataType={DataType}",
                request.Format, request.DataType);

            var response = await _exportService.ExportDataAsync(request, cancellationToken);

            // Return file for download if content exists
            if (!string.IsNullOrEmpty(response.FileContent))
            {
                var fileBytes = Convert.FromBase64String(response.FileContent);
                return File(fileBytes, response.MimeType, response.FileName);
            }

            // Return metadata response if no file content
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Warning(ex, "Invalid export parameters");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error exporting data");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while exporting data" });
        }
    }

    /// <summary>
    /// Get export formats supported by the system
    /// GET /api/export/formats
    /// </summary>
    /// <returns>List of supported export formats</returns>
    [HttpGet("formats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<string>> GetSupportedFormats()
    {
        var formats = new[] { "Excel", "PDF" };
        return Ok(formats);
    }

    /// <summary>
    /// Get export data types supported by the system
    /// GET /api/export/data-types
    /// </summary>
    /// <returns>List of supported export data types</returns>
    [HttpGet("data-types")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<string>> GetSupportedDataTypes()
    {
        var dataTypes = new[] { "PatientData", "VisitHistory", "PrescriptionData" };
        return Ok(dataTypes);
    }

    /// <summary>
    /// Export patient visit history
    /// GET /api/export/patient/{patientId}/visits
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="startDate">Start date for filtering (optional)</param>
    /// <param name="endDate">End date for filtering (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of visit export rows</returns>
    [HttpGet("patient/{patientId}/visits")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<VisitExportRow>>> GetPatientVisits(
        int patientId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (patientId <= 0)
                return BadRequest(new { message = "Invalid patient ID" });

            _logger.Information("Fetching visits for export: PatientId={PatientId}", patientId);

            var visits = await _exportService.GetPatientVisitsForExportAsync(
                patientId, startDate, endDate, cancellationToken);

            return Ok(visits);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching patient visits");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while fetching visits" });
        }
    }

    /// <summary>
    /// Export all patients data
    /// GET /api/export/patients
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of patient export rows</returns>
    [HttpGet("patients")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PatientExportRow>>> GetAllPatients(CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Fetching all patients for export");

            var patients = await _exportService.GetPatientsForExportAsync(cancellationToken);
            return Ok(patients);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching patients");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while fetching patients" });
        }
    }

    /// <summary>
    /// Export prescriptions data
    /// GET /api/export/prescriptions
    /// </summary>
    /// <param name="patientId">Patient ID for filtering (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of prescription export rows</returns>
    [HttpGet("prescriptions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PrescriptionExportRow>>> GetPrescriptions(
        [FromQuery] int? patientId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching prescriptions for export: PatientId={PatientId}", patientId);

            var prescriptions = await _exportService.GetPrescriptionsForExportAsync(patientId, cancellationToken);
            return Ok(prescriptions);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching prescriptions");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while fetching prescriptions" });
        }
    }
}
