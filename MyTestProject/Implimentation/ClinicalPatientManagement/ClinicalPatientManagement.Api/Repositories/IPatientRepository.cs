using ClinicalPatientManagement.Api.Models;

namespace ClinicalPatientManagement.Api.Repositories;

/// <summary>
/// Specialized repository interface for Patient entity with search capability
/// Step 6: Patient Management - Data Access Layer
/// </summary>
public interface IPatientRepository : IRepository<Patient>
{
    /// <summary>
    /// Search patients by name or phone (case-insensitive, partial match)
    /// </summary>
    Task<IList<Patient>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
