using ClinicalPatientManagement.Api.Models;

namespace ClinicalPatientManagement.Api.Repositories;

/// <summary>
/// Interface for Prescription data access operations
/// Step 10: Prescription Generation
/// </summary>
public interface IPrescriptionRepository : IRepository<Prescription>
{
    /// <summary>
    /// Get prescription by consultation ID
    /// </summary>
    Task<Prescription?> GetByConsultationIdAsync(int consultationId);

    /// <summary>
    /// Get all prescriptions for a patient by patient ID
    /// </summary>
    Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId);

    /// <summary>
    /// Check if a prescription exists for a consultation
    /// </summary>
    Task<bool> ExistsByConsultationIdAsync(int consultationId);

    /// <summary>
    /// Create async wrapper for backward compatibility
    /// </summary>
    Task<Prescription> CreateAsync(Prescription entity);

    /// <summary>
    /// Save changes to the database
    /// </summary>
    Task SaveChangesAsync();
}
