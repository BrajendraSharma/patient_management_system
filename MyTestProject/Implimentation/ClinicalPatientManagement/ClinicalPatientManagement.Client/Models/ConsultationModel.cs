using System.ComponentModel.DataAnnotations;

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
public class ConsultationCreateModel
{
    [Required(ErrorMessage = "Temperature is required")]
    [Range(30, 45, ErrorMessage = "Temperature must be between 30°C and 45°C")]
    public decimal Temperature { get; set; }

    [Required(ErrorMessage = "Blood pressure is required")]
    [RegularExpression(@"^\d{2,3}/\d{2,3}$", ErrorMessage = "Blood pressure must be in format XXX/YYY (e.g., 120/80)")]
    public string BloodPressure { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pulse is required")]
    [Range(40, 200, ErrorMessage = "Pulse must be between 40 and 200 bpm")]
    public int Pulse { get; set; }

    [Required(ErrorMessage = "Complaints are required")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "Complaints must be between 5 and 1000 characters")]
    public string Complaints { get; set; } = string.Empty;

    [Required(ErrorMessage = "Diagnosis is required")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "Diagnosis must be between 5 and 1000 characters")]
    public string Diagnosis { get; set; } = string.Empty;
}

/// <summary>
/// DTO for medication in consultation
/// </summary>
public class MedicationCreateModel
{
    [Required(ErrorMessage = "Medication name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Medication name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Dosage is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Dosage must be between 2 and 50 characters")]
    public string Dosage { get; set; } = string.Empty;

    [Required(ErrorMessage = "Frequency is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Frequency must be between 2 and 100 characters")]
    public string Frequency { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duration is required")]
    [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
    public int Duration { get; set; }

    [StringLength(200, ErrorMessage = "Instructions cannot exceed 200 characters")]
    public string Instructions { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating a consultation
/// </summary>
public class UpdateConsultationModel
{
    [Required(ErrorMessage = "Temperature is required")]
    [Range(30, 45, ErrorMessage = "Temperature must be between 30°C and 45°C")]
    public decimal Temperature { get; set; }

    [Required(ErrorMessage = "Blood pressure is required")]
    [RegularExpression(@"^\d{2,3}/\d{2,3}$", ErrorMessage = "Blood pressure must be in format XXX/YYY (e.g., 120/80)")]
    public string BloodPressure { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pulse is required")]
    [Range(40, 200, ErrorMessage = "Pulse must be between 40 and 200 bpm")]
    public int Pulse { get; set; }

    [Required(ErrorMessage = "Complaints are required")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "Complaints must be between 5 and 1000 characters")]
    public string Complaints { get; set; } = string.Empty;

    [Required(ErrorMessage = "Diagnosis is required")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "Diagnosis must be between 5 and 1000 characters")]
    public string Diagnosis { get; set; } = string.Empty;
}
