using ClinicalPatientManagement.Api.DTOs;

namespace ClinicalPatientManagement.Api.Services;

/// <summary>
/// Service interface for patient management operations
/// Step 6: Patient Management - Business Logic Layer
/// </summary>
public interface IPatientService
{
    /// <summary>
    /// Get all patients
    /// </summary>
    Task<IEnumerable<PatientDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get patient by ID
    /// </summary>
    Task<PatientDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create new patient
    /// </summary>
    Task<PatientDto> CreateAsync(CreatePatientDto createDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing patient
    /// </summary>
    Task<PatientDto> UpdateAsync(int id, UpdatePatientDto updateDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete patient by ID
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search patients by name or phone
    /// Requirement Step 8: Enhance patient search
    /// </summary>
    Task<IEnumerable<PatientDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if patient exists
    /// </summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate patient data
    /// </summary>
    bool ValidatePatientData(CreatePatientDto dto, out List<string> errors);
}
