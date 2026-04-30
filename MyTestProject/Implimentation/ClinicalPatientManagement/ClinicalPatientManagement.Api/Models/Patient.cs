using System.ComponentModel.DataAnnotations;

namespace ClinicalPatientManagement.Api.Models;

/// <summary>
/// Patient entity representing a patient in the system
/// </summary>
public class Patient : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [StringLength(10)]
    public string Gender { get; set; } = string.Empty; // Male, Female, Other

    // Navigation properties
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}