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
}
