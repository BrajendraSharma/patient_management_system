namespace ClinicalPatientManagement.Client.Models;

/// <summary>
/// Appointment model for Blazor client
/// Step 7: Appointment Scheduling - Client-side model
/// </summary>
public class AppointmentModel
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = "Scheduled";
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Helper properties
    public string FormattedDate => AppointmentDate.ToString("dd/MM/yyyy");
    public string FormattedTime => AppointmentDate.ToString("HH:mm");
    public string FormattedDateTime => AppointmentDate.ToString("dd/MM/yyyy HH:mm");
    
    public string StatusBadgeClass => Status switch
    {
        "Scheduled" => "badge bg-primary",
        "Completed" => "badge bg-success",
        "Cancelled" => "badge bg-danger",
        "No-Show" => "badge bg-warning",
        _ => "badge bg-secondary"
    };
}

/// <summary>
/// DTO for creating a new appointment
/// </summary>
public class CreateAppointmentModel
{
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; } = DateTime.Now.AddDays(1);
    public string Status { get; set; } = "Scheduled";
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating an appointment
/// </summary>
public class UpdateAppointmentModel
{
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
