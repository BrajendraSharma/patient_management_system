namespace ClinicalPatientManagement.Api.DTOs;

/// <summary>
/// Data Transfer Object for Appointment entity
/// Placeholder - will be completed in Step 7
/// </summary>
public class AppointmentDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Data Transfer Object for creating an appointment
/// </summary>
public class CreateAppointmentDto
{
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = "Scheduled";
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for updating an appointment
/// </summary>
public class UpdateAppointmentDto
{
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
