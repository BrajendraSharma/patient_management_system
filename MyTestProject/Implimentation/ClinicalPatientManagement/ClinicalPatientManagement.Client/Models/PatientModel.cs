namespace ClinicalPatientManagement.Client.Models;

/// <summary>
/// Patient model for Blazor client
/// Step 6: Patient Management - Client-side model
/// </summary>
public class PatientModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = "Male";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
}

/// <summary>
/// DTO for creating a new patient
/// </summary>
public class CreatePatientModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-30);
    public string Gender { get; set; } = "Male";
}

/// <summary>
/// DTO for updating a patient
/// </summary>
public class UpdatePatientModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = "Male";
}
