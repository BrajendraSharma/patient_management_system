using System.ComponentModel.DataAnnotations;

namespace ClinicalPatientManagement.Api.Models;

/// <summary>
/// Prescription entity representing a prescription for a consultation
/// </summary>
public class Prescription : BaseEntity
{
    [Required]
    public int ConsultationId { get; set; }

    [Required]
    public DateTime PrescriptionDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Consultation Consultation { get; set; } = null!;
    public ICollection<Medication> Medications { get; set; } = new List<Medication>();
}