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
/// Medication DTO for prescription medications
/// </summary>
public class MedicationDto
{
    public int Id { get; set; }
    public int PrescriptionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Input model for creating a prescription
/// </summary>
public class CreatePrescriptionDto
{
    public int ConsultationId { get; set; }
    public List<CreateMedicationDto> Medications { get; set; } = new List<CreateMedicationDto>();
}

/// <summary>
/// Input model for creating medication in a prescription
/// </summary>
public class CreateMedicationDto
{
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Instructions { get; set; } = string.Empty;
}

/// <summary>
/// Input model for updating a prescription
/// </summary>
public class UpdatePrescriptionDto
{
    public List<UpdateMedicationDto> Medications { get; set; } = new List<UpdateMedicationDto>();
}

/// <summary>
/// Input model for updating medication in a prescription
/// </summary>
public class UpdateMedicationDto
{
    public int? Id { get; set; } // If null, it's a new medication
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Instructions { get; set; } = string.Empty;
}
