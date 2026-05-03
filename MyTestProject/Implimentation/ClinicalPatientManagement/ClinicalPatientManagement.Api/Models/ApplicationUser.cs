using Microsoft.AspNetCore.Identity;

namespace ClinicalPatientManagement.Api.Models;

/// <summary>
/// Application user for authentication
/// </summary>
public class ApplicationUser : IdentityUser
{
    // Additional properties can be added here if needed
    // For single-user system, basic IdentityUser is sufficient
}