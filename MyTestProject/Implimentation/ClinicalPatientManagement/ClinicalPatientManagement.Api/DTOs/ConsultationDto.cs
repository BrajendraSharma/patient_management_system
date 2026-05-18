using System.ComponentModel.DataAnnotations;

namespace ClinicalPatientManagement.Api.DTOs;

/// <summary>
/// Custom validation for blood pressure format (Phase 1 security)
/// </summary>
public class BloodPressureFormatAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return false;

        var bp = value.ToString()!;
        var parts = bp.Split('/');
        
        if (parts.Length != 2)
            return false;

        if (!int.TryParse(parts[0], out int systolic) || !int.TryParse(parts[1], out int diastolic))
            return false;

        // Typical ranges: 60-200 systolic, 40-120 diastolic
        return systolic >= 60 && systolic <= 200 && diastolic >= 40 && diastolic <= 120;
    }
}

/// <summary>
/// Data Transfer Object for Consultation entity with validation (Phase 1)
/// </summary>
public class ConsultationDto
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }

    [Range(30, 45, ErrorMessage = "Temperature must be between 30°C and 45°C")]
    public decimal Temperature { get; set; }

    [BloodPressureFormat(ErrorMessage = "Blood pressure must be in format ###/### (e.g., 120/80)")]
    public string BloodPressure { get; set; } = string.Empty;

    [Range(40, 200, ErrorMessage = "Pulse must be between 40 and 200 bpm")]
    public int Pulse { get; set; }

    [Required(ErrorMessage = "Complaints are required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Complaints must be between 5 and 500 characters")]
    public string Complaints { get; set; } = string.Empty;

    [Required(ErrorMessage = "Diagnosis is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Diagnosis must be between 5 and 500 characters")]
    public string Diagnosis { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Data Transfer Object for creating a new consultation with validation (Phase 1)
/// </summary>
public class CreateConsultationDto
{
    [Range(1, int.MaxValue, ErrorMessage = "AppointmentId must be valid")]
    public int AppointmentId { get; set; }

    [Range(30, 45, ErrorMessage = "Temperature must be between 30°C and 45°C")]
    public decimal Temperature { get; set; }

    [BloodPressureFormat(ErrorMessage = "Blood pressure must be in format ###/### (e.g., 120/80)")]
    public string BloodPressure { get; set; } = string.Empty;

    [Range(40, 200, ErrorMessage = "Pulse must be between 40 and 200 bpm")]
    public int Pulse { get; set; }

    [Required(ErrorMessage = "Complaints are required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Complaints must be between 5 and 500 characters")]
    public string Complaints { get; set; } = string.Empty;

    [Required(ErrorMessage = "Diagnosis is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Diagnosis must be between 5 and 500 characters")]
    public string Diagnosis { get; set; } = string.Empty;

    public List<CreateMedicationDto> Medications { get; set; } = new();
}

/// <summary>
/// Data Transfer Object for updating a consultation with validation (Phase 1)
/// </summary>
public class UpdateConsultationDto
{
    [Range(30, 45, ErrorMessage = "Temperature must be between 30°C and 45°C")]
    public decimal Temperature { get; set; }

    [BloodPressureFormat(ErrorMessage = "Blood pressure must be in format ###/### (e.g., 120/80)")]
    public string BloodPressure { get; set; } = string.Empty;

    [Range(40, 200, ErrorMessage = "Pulse must be between 40 and 200 bpm")]
    public int Pulse { get; set; }

    [Required(ErrorMessage = "Complaints are required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Complaints must be between 5 and 500 characters")]
    public string Complaints { get; set; } = string.Empty;

    [Required(ErrorMessage = "Diagnosis is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Diagnosis must be between 5 and 500 characters")]
    public string Diagnosis { get; set; } = string.Empty;
}
