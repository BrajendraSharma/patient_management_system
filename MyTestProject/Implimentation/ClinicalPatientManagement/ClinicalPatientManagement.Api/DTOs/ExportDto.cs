namespace ClinicalPatientManagement.Api.DTOs;

/// <summary>
/// Request DTO for exporting patient/visit data
/// Step 13: Add data export
/// </summary>
public class ExportRequest
{
    /// <summary>
    /// Type of export (Excel or PDF)
    /// </summary>
    public string Format { get; set; } = "Excel"; // "Excel" or "PDF"

    /// <summary>
    /// Type of data to export (PatientData, VisitHistory, PrescriptionData)
    /// </summary>
    public string DataType { get; set; } = "VisitHistory"; // "PatientData", "VisitHistory", "PrescriptionData"

    /// <summary>
    /// Patient ID for filtering (optional)
    /// </summary>
    public int? PatientId { get; set; }

    /// <summary>
    /// Start date for filtering visits (optional)
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for filtering visits (optional)
    /// </summary>
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// Response DTO for export operation
/// </summary>
public class ExportResponse
{
    /// <summary>
    /// Unique export ID
    /// </summary>
    public string ExportId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Status of export (Pending, Completed, Failed)
    /// </summary>
    public string Status { get; set; } = "Completed";

    /// <summary>
    /// Base64 encoded file content
    /// </summary>
    public string FileContent { get; set; } = string.Empty;

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME type (application/vnd.ms-excel, application/pdf)
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Export timestamp
    /// </summary>
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Number of records exported
    /// </summary>
    public int RecordCount { get; set; }
}

/// <summary>
/// Data structure for patient export row
/// </summary>
public class PatientExportRow
{
    public int PatientId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty; // DD-MM-YYYY format
}

/// <summary>
/// Data structure for visit/consultation export row
/// </summary>
public class VisitExportRow
{
    public int ConsultationId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string ConsultationDate { get; set; } = string.Empty; // DD-MM-YYYY format
    public decimal Temperature { get; set; }
    public string BloodPressure { get; set; } = string.Empty;
    public int Pulse { get; set; }
    public string Complaints { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string Medications { get; set; } = string.Empty; // Comma-separated
}

/// <summary>
/// Data structure for prescription export row
/// </summary>
public class PrescriptionExportRow
{
    public int PrescriptionId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PrescriptionDate { get; set; } = string.Empty; // DD-MM-YYYY format
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Instructions { get; set; } = string.Empty;
}
