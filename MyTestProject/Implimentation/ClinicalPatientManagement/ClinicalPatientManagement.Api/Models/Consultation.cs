using System.ComponentModel.DataAnnotations;

namespace ClinicalPatientManagement.Api.Models;

/// <summary>
/// Consultation entity representing a patient consultation
/// </summary>
public class Consultation : BaseEntity
{
    [Required]
    public int AppointmentId { get; set; }

    [Required]
    [Range(30, 45)] // Reasonable temperature range in Celsius
    public decimal Temperature { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^\d{2,3}/\d{2,3}$")] // Format like 120/80
    public string BloodPressure { get; set; } = string.Empty;

    [Required]
    [Range(40, 200)] // Reasonable pulse range
    public int Pulse { get; set; }

    [Required]
    [StringLength(1000)]
    public string Complaints { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Diagnosis { get; set; } = string.Empty;

    // Navigation properties
    public Appointment Appointment { get; set; } = null!;
    public Prescription? Prescription { get; set; }
}