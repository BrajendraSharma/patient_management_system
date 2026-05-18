using System.ComponentModel.DataAnnotations;

namespace ClinicalPatientManagement.Api.DTOs;

/// <summary>
/// DTO for user login with input validation (Phase 1 security)
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Username is required and must be between 3-50 characters
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password is required and must be between 6-100 characters
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    public string Password { get; set; } = string.Empty;
}