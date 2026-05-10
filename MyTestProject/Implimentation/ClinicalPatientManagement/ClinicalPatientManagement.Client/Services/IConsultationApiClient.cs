using ClinicalPatientManagement.Client.Models;

namespace ClinicalPatientManagement.Client.Services;

/// <summary>
/// API client interface for consultation operations
/// Step 9: Implement Consultation Creation - Client API communication
/// Step 12: Implement Patient History - Get consultation history with date filtering
/// </summary>
public interface IConsultationApiClient
{
    /// <summary>
    /// Get all consultations
    /// </summary>
    Task<IEnumerable<ConsultationModel>> GetAllAsync();

    /// <summary>
    /// Get consultation by ID
    /// </summary>
    Task<ConsultationModel?> GetByIdAsync(int id);

    /// <summary>
    /// Create new consultation
    /// </summary>
    Task<ConsultationModel> CreateAsync(CreateConsultationModel createModel);

    /// <summary>
    /// Update consultation
    /// </summary>
    Task<ConsultationModel> UpdateAsync(int id, UpdateConsultationModel updateModel);

    /// <summary>
    /// Delete consultation
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Get consultation by appointment ID
    /// </summary>
    Task<ConsultationModel?> GetByAppointmentIdAsync(int appointmentId);

    /// <summary>
    /// Get all consultations for a patient
    /// </summary>
    Task<IEnumerable<ConsultationModel>> GetByPatientIdAsync(int patientId);

    /// <summary>
    /// Get patient consultation history with optional date filtering
    /// Step 12: Implement Patient History - View past visits with date filtering
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="startDate">Start date for filtering (optional)</param>
    /// <param name="endDate">End date for filtering (optional)</param>
    /// <returns>Filtered consultations ordered by date descending</returns>
    Task<IEnumerable<ConsultationModel>?> GetPatientHistoryAsync(
        int patientId,
        DateTime? startDate = null,
        DateTime? endDate = null);
}
