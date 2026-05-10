namespace ClinicalPatientManagement.Client.Models;

/// <summary>
/// Medication model for displaying prescription medications in history
/// Step 12: Patient History - Prescription display
/// </summary>
public class MedicationInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Instructions { get; set; } = string.Empty;
}

/// <summary>
/// Consultation model for Blazor client
/// Step 9: Implement Consultation Creation - Client-side model
/// Step 12: Implement Patient History - Display consultation history with prescriptions
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
    
    /// <summary>
    /// Medications prescribed in this consultation
    /// Step 12: Patient History - Display prescriptions
    /// </summary>
    public List<MedicationInfo> Medications { get; set; } = new();
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
