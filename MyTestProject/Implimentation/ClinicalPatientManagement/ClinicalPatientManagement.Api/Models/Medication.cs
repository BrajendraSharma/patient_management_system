using System.ComponentModel.DataAnnotations;

namespace ClinicalPatientManagement.Api.Models;

/// <summary>
/// Medication entity representing a medication in a prescription
/// </summary>
public class Medication : BaseEntity
{
    [Required]
    public int PrescriptionId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Dosage { get; set; } = string.Empty; // e.g., 500mg

    [Required]
    [StringLength(100)]
    public string Frequency { get; set; } = string.Empty; // e.g., Twice daily

    [Required]
    [Range(1, 365)] // Duration in days
    public int Duration { get; set; }

    [StringLength(500)]
    public string Instructions { get; set; } = string.Empty;

    // Navigation properties
    public Prescription Prescription { get; set; } = null!;
}