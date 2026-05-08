using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;

namespace ClinicalPatientManagement.Api.Services;

/// <summary>
/// Interface for Prescription business logic operations
/// Step 10: Prescription Generation
/// </summary>
public interface IPrescriptionService
{
    /// <summary>
    /// Get all prescriptions
    /// </summary>
    Task<IEnumerable<PrescriptionDto>> GetAllAsync();

    /// <summary>
    /// Get prescription by ID
    /// </summary>
    Task<PrescriptionDto?> GetByIdAsync(int id);

    /// <summary>
    /// Get prescription by consultation ID
    /// </summary>
    Task<PrescriptionDto?> GetByConsultationIdAsync(int consultationId);

    /// <summary>
    /// Get all prescriptions for a patient
    /// </summary>
    Task<IEnumerable<PrescriptionDto>> GetByPatientIdAsync(int patientId);

    /// <summary>
    /// Create a new prescription with medications
    /// </summary>
    Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto dto);

    /// <summary>
    /// Update prescription medications
    /// </summary>
    Task<PrescriptionDto> UpdateAsync(int id, UpdatePrescriptionDto dto);

    /// <summary>
    /// Delete a prescription
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Check if prescription exists for consultation
    /// </summary>
    Task<bool> ExistsAsync(int consultationId);

    /// <summary>
    /// Validate prescription data
    /// </summary>
    bool ValidatePrescriptionData(CreatePrescriptionDto dto, out List<string> errors);
}
