using System.ComponentModel.DataAnnotations;

namespace ClinicalPatientManagement.Api.DTOs;

/// <summary>
/// Read model for Prescription - returned from API
/// </summary>
public class PrescriptionDto
{
    public int Id { get; set; }
    public int ConsultationId { get; set; }
    public DateTime PrescriptionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Include related consultation data for context
    public ConsultationDto? Consultation { get; set; }
    
    // Include medications for full prescription view
    public List<MedicationDto> Medications { get; set; } = new List<MedicationDto>();
}

/// <summary>
/// Medication DTO for prescription medications with validation (Phase 1)
/// </summary>
public class MedicationDto
{
    public int Id { get; set; }
    public int PrescriptionId { get; set; }

    [Required(ErrorMessage = "Medication name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Dosage is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Dosage must be between 1 and 50 characters")]
    public string Dosage { get; set; } = string.Empty;

    [Required(ErrorMessage = "Frequency is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Frequency must be between 1 and 100 characters")]
    public string Frequency { get; set; } = string.Empty;

    [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
    public int Duration { get; set; }

    [StringLength(500, ErrorMessage = "Instructions must not exceed 500 characters")]
    public string Instructions { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Input model for creating a prescription with validation (Phase 1)
/// </summary>
public class CreatePrescriptionDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ConsultationId must be valid")]
    public int ConsultationId { get; set; }

    [Required(ErrorMessage = "At least one medication is required")]
    [MinLength(1, ErrorMessage = "At least one medication must be provided")]
    public List<CreateMedicationDto> Medications { get; set; } = new List<CreateMedicationDto>();
}

/// <summary>
/// Input model for creating medication in a prescription with validation (Phase 1)
/// </summary>
public class CreateMedicationDto
{
    [Required(ErrorMessage = "Medication name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Dosage is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Dosage must be between 1 and 50 characters")]
    public string Dosage { get; set; } = string.Empty;

    [Required(ErrorMessage = "Frequency is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Frequency must be between 1 and 100 characters")]
    public string Frequency { get; set; } = string.Empty;

    [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
    public int Duration { get; set; }

    [StringLength(500, ErrorMessage = "Instructions must not exceed 500 characters")]
    public string Instructions { get; set; } = string.Empty;
}

/// <summary>
/// Input model for updating a prescription with validation (Phase 1)
/// </summary>
public class UpdatePrescriptionDto
{
    [Required(ErrorMessage = "Medications list is required")]
    public List<UpdateMedicationDto> Medications { get; set; } = new List<UpdateMedicationDto>();
}

/// <summary>
/// Input model for updating medication in a prescription with validation (Phase 1)
/// </summary>
public class UpdateMedicationDto
{
    public int? Id { get; set; } // If null, it's a new medication

    [Required(ErrorMessage = "Medication name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Dosage is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Dosage must be between 1 and 50 characters")]
    public string Dosage { get; set; } = string.Empty;

    [Required(ErrorMessage = "Frequency is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Frequency must be between 1 and 100 characters")]
    public string Frequency { get; set; } = string.Empty;

    [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
    public int Duration { get; set; }

    [StringLength(500, ErrorMessage = "Instructions must not exceed 500 characters")]
    public string Instructions { get; set; } = string.Empty;
}
