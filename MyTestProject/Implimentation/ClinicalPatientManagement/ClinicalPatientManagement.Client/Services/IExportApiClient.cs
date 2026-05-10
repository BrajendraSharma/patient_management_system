using ClinicalPatientManagement.Client.Models;

namespace ClinicalPatientManagement.Client.Services;

/// <summary>
/// Client interface for API export operations
/// Step 13: Add data export
/// </summary>
public interface IExportApiClient
{
    /// <summary>
    /// Export data to Excel or PDF format
    /// </summary>
    /// <param name="format">Export format (Excel or PDF)</param>
    /// <param name="dataType">Type of data to export (PatientData, VisitHistory, PrescriptionData)</param>
    /// <param name="patientId">Patient ID for filtering (optional)</param>
    /// <param name="startDate">Start date for filtering (optional)</param>
    /// <param name="endDate">End date for filtering (optional)</param>
    /// <returns>Byte array of file content</returns>
    Task<byte[]> ExportDataAsync(
        string format,
        string dataType,
        int? patientId = null,
        DateTime? startDate = null,
        DateTime? endDate = null);

    /// <summary>
    /// Get supported export formats
    /// </summary>
    Task<IEnumerable<string>> GetSupportedFormatsAsync();

    /// <summary>
    /// Get supported export data types
    /// </summary>
    Task<IEnumerable<string>> GetSupportedDataTypesAsync();

    /// <summary>
    /// Get patient visits for export
    /// </summary>
    Task<IEnumerable<VisitExportRow>> GetPatientVisitsAsync(
        int patientId,
        DateTime? startDate = null,
        DateTime? endDate = null);

    /// <summary>
    /// Get all patients for export
    /// </summary>
    Task<IEnumerable<PatientExportRow>> GetPatientsAsync();

    /// <summary>
    /// Get prescriptions for export
    /// </summary>
    Task<IEnumerable<PrescriptionExportRow>> GetPrescriptionsAsync(int? patientId = null);
}

/// <summary>
/// Export response model
/// </summary>
public class ExportResponse
{
    public string ExportId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string FileContent { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public DateTime ExportedAt { get; set; }
    public int RecordCount { get; set; }
}

/// <summary>
/// Patient export row model
/// </summary>
public class PatientExportRow
{
    public int PatientId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
}

/// <summary>
/// Visit export row model
/// </summary>
public class VisitExportRow
{
    public int ConsultationId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string ConsultationDate { get; set; } = string.Empty;
    public decimal Temperature { get; set; }
    public string BloodPressure { get; set; } = string.Empty;
    public int Pulse { get; set; }
    public string Complaints { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string Medications { get; set; } = string.Empty;
}

/// <summary>
/// Prescription export row model
/// </summary>
public class PrescriptionExportRow
{
    public int PrescriptionId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PrescriptionDate { get; set; } = string.Empty;
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Instructions { get; set; } = string.Empty;
}
