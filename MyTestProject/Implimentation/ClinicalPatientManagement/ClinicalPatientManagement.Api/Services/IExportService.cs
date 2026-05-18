namespace ClinicalPatientManagement.Api.Services;

using ClinicalPatientManagement.Api.DTOs;

/// <summary>
/// Service interface for data export operations
/// Step 13: Add data export functionality for Excel and PDF formats
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Export patient/visit data to Excel or PDF format
    /// </summary>
    /// <param name="request">Export request with format, data type, and filter criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ExportResponse with file content and metadata</returns>
    Task<ExportResponse> ExportDataAsync(ExportRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get patient history data for export
    /// </summary>
    /// <param name="patientId">Patient ID to filter by</param>
    /// <param name="startDate">Start date for filtering (optional)</param>
    /// <param name="endDate">End date for filtering (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of visit export rows with formatted dates (DD-MM-YYYY)</returns>
    Task<List<VisitExportRow>> GetPatientVisitsForExportAsync(
        int patientId, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all patients data for export
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of patient export rows with formatted dates (DD-MM-YYYY)</returns>
    Task<List<PatientExportRow>> GetPatientsForExportAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get prescription data for export
    /// </summary>
    /// <param name="patientId">Patient ID to filter by (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of prescription export rows with formatted dates (DD-MM-YYYY)</returns>
    Task<List<PrescriptionExportRow>> GetPrescriptionsForExportAsync(int? patientId = null, CancellationToken cancellationToken = default);
}
