using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can only contain letters and spaces")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can only contain letters and spaces")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(20, MinimumLength = 7, ErrorMessage = "Phone number must be between 7 and 20 characters")]
    [RegularExpression(@"^[\d\+\-\(\)\s]+$", ErrorMessage = "Phone number contains invalid characters")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email format is invalid")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-30);

    [Required(ErrorMessage = "Gender is required")]
    [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
    public string Gender { get; set; } = "Male";
}

/// <summary>
/// DTO for updating a patient
/// </summary>
public class UpdatePatientModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can only contain letters and spaces")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can only contain letters and spaces")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(20, MinimumLength = 7, ErrorMessage = "Phone number must be between 7 and 20 characters")]
    [RegularExpression(@"^[\d\+\-\(\)\s]+$", ErrorMessage = "Phone number contains invalid characters")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email format is invalid")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
    public string Gender { get; set; } = "Male";
}
