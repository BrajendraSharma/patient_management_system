using ClinicalPatientManagement.Api.Models;

namespace ClinicalPatientManagement.Api.Repositories;

/// <summary>
/// Repository interface for consultation data access operations
/// Step 9: Implement Consultation Creation - Data Access Layer
/// </summary>
public interface IConsultationRepository : IRepository<Consultation>
{
    /// <summary>
    /// Get consultation by appointment ID
    /// </summary>
    Task<Consultation?> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all consultations for a specific patient
    /// </summary>
    Task<IEnumerable<Consultation>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if consultation exists for appointment
    /// </summary>
    Task<bool> ExistsByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default);
}
