using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "Patient is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid patient")]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Appointment date and time is required")]
    [DataType(DataType.DateTime)]
    [FutureDate(ErrorMessage = "Appointment date must be in the future")]
    [MaxFutureDate(365, ErrorMessage = "Appointment cannot be scheduled more than 1 year in advance")]
    public DateTime AppointmentDate { get; set; } = DateTime.Now.AddDays(1);

    [Required(ErrorMessage = "Status is required")]
    [RegularExpression(@"^(Scheduled|Completed|Cancelled|No-Show)$", ErrorMessage = "Status must be Scheduled, Completed, Cancelled, or No-Show")]
    public string Status { get; set; } = "Scheduled";

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating an appointment
/// </summary>
public class UpdateAppointmentModel
{
    [Required(ErrorMessage = "Patient is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid patient")]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Appointment date and time is required")]
    [DataType(DataType.DateTime)]
    [FutureDate(ErrorMessage = "Appointment date must be in the future")]
    [MaxFutureDate(365, ErrorMessage = "Appointment cannot be scheduled more than 1 year in advance")]
    public DateTime AppointmentDate { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [RegularExpression(@"^(Scheduled|Completed|Cancelled|No-Show)$", ErrorMessage = "Status must be Scheduled, Completed, Cancelled, or No-Show")]
    public string Status { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Custom validation attribute for future dates
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime < DateTime.Now)
            {
                return new ValidationResult(ErrorMessage ?? "Date must be in the future");
            }
        }
        return ValidationResult.Success;
    }
}

/// <summary>
/// Custom validation attribute for max future date (e.g., max 1 year)
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MaxFutureDateAttribute : ValidationAttribute
{
    private readonly int _maxDays;

    public MaxFutureDateAttribute(int maxDays)
    {
        _maxDays = maxDays;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime > DateTime.Now.AddDays(_maxDays))
            {
                return new ValidationResult(ErrorMessage ?? $"Date cannot be more than {_maxDays} days in the future");
            }
        }
        return ValidationResult.Success;
    }
}
