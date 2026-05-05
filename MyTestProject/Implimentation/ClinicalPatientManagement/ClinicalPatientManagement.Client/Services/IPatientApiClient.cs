using ClinicalPatientManagement.Client.Models;

namespace ClinicalPatientManagement.Client.Services;

/// <summary>
/// API client interface for patient operations
/// Step 6: Patient Management - Client API communication
/// </summary>
public interface IPatientApiClient
{
    /// <summary>
    /// Get all patients
    /// </summary>
    Task<IEnumerable<PatientModel>> GetAllAsync();

    /// <summary>
    /// Get patient by ID
    /// </summary>
    Task<PatientModel?> GetByIdAsync(int id);

    /// <summary>
    /// Create new patient
    /// </summary>
    Task<PatientModel> CreateAsync(CreatePatientModel createModel);

    /// <summary>
    /// Update patient
    /// </summary>
    Task<PatientModel> UpdateAsync(int id, UpdatePatientModel updateModel);

    /// <summary>
    /// Delete patient
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Search patients by name or phone
    /// </summary>
    Task<IEnumerable<PatientModel>> SearchAsync(string searchTerm);
}
