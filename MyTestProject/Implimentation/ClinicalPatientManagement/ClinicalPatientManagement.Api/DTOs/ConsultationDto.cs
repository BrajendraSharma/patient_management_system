namespace ClinicalPatientManagement.Api.DTOs;

/// <summary>
/// Data Transfer Object for Consultation entity
/// Step 9: Implement Consultation Creation
/// </summary>
public class ConsultationDto
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
/// Data Transfer Object for creating a new consultation
/// </summary>
public class CreateConsultationDto
{
    public int AppointmentId { get; set; }
    public decimal Temperature { get; set; }
    public string BloodPressure { get; set; } = string.Empty;
    public int Pulse { get; set; }
    public string Complaints { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for updating a consultation
/// </summary>
public class UpdateConsultationDto
{
    public decimal Temperature { get; set; }
    public string BloodPressure { get; set; } = string.Empty;
    public int Pulse { get; set; }
    public string Complaints { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
}
