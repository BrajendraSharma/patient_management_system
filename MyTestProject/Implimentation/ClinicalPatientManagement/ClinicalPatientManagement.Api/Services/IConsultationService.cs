using ClinicalPatientManagement.Api.DTOs;

namespace ClinicalPatientManagement.Api.Services;

/// <summary>
/// Service interface for consultation management operations
/// Step 9: Implement Consultation Creation - Business Logic Layer
/// </summary>
public interface IConsultationService
{
    /// <summary>
    /// Get all consultations
    /// </summary>
    Task<IEnumerable<ConsultationDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get consultation by ID
    /// </summary>
    Task<ConsultationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create new consultation with validation
    /// </summary>
    Task<ConsultationDto> CreateAsync(CreateConsultationDto createDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing consultation with validation
    /// </summary>
    Task<ConsultationDto> UpdateAsync(int id, UpdateConsultationDto updateDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete consultation by ID
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get consultation by appointment ID
    /// </summary>
    Task<ConsultationDto?> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all consultations for a patient
    /// </summary>
    Task<IEnumerable<ConsultationDto>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if consultation exists
    /// </summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate consultation data
    /// </summary>
    bool ValidateConsultationData(CreateConsultationDto dto, out List<string> errors);

    /// <summary>
    /// Create consultation with prescription in a single atomic transaction
    /// Step 11: Persist consultations with transactions - ACID compliance
    /// Ensures consultation and prescription are either both persisted or both rolled back on failure
    /// </summary>
    /// <param name="consultationDto">Consultation data</param>
    /// <param name="prescriptionDto">Prescription data (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created consultation with prescription details</returns>
    /// <exception cref="InvalidOperationException">Thrown when validation fails or appointment not found</exception>
    Task<ConsultationDto> CreateConsultationWithPrescriptionAsync(
        CreateConsultationDto consultationDto,
        CreatePrescriptionDto? prescriptionDto = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get patient consultation history with optional date filtering
    /// Step 12: Implement Patient History - View past visits with date filtering
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="startDate">Start date for filtering (inclusive), null for no lower bound</param>
    /// <param name="endDate">End date for filtering (inclusive), null for no upper bound</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Filtered consultations ordered by creation date descending</returns>
    /// <exception cref="ArgumentException">Thrown when startDate > endDate</exception>
    Task<IEnumerable<ConsultationDto>> GetPatientHistoryAsync(
        int patientId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
}
