namespace ClinicalPatientManagement.Client.Models;

/// <summary>
/// Consultation model for Blazor client
/// Step 9: Implement Consultation Creation - Client-side model
/// Step 12: Implement Patient History - Display consultation history
/// </summary>
public class ConsultationModel
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public decimal Temperature { get; set; }
    public string BloodPressure { get; set; } = string.Empty;
    public int Pulse { get; set; }
    public string Complaints { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new consultation
/// </summary>
public class CreateConsultationModel
{
    public int AppointmentId { get; set; }
    public decimal Temperature { get; set; }
    public string BloodPressure { get; set; } = string.Empty;
    public int Pulse { get; set; }
    public string Complaints { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating a consultation
/// </summary>
public class UpdateConsultationModel
{
    public decimal Temperature { get; set; }
    public string BloodPressure { get; set; } = string.Empty;
    public int Pulse { get; set; }
    public string Complaints { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
}
