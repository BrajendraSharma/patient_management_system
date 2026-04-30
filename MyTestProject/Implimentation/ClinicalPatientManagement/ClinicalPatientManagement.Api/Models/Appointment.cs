using System.ComponentModel.DataAnnotations;

namespace ClinicalPatientManagement.Api.Models;

/// <summary>
/// Appointment entity representing a scheduled appointment
/// </summary>
public class Appointment : BaseEntity
{
    [Required]
    public int PatientId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled, No-Show

    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;

    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public Consultation? Consultation { get; set; }
}