namespace ClinicalPatientManagement.Client.Models;

/// <summary>
/// Base DTO for all client-side models
/// </summary>
public class BaseDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
