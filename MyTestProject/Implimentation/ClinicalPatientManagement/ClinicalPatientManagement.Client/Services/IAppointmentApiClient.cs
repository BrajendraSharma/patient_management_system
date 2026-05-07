using ClinicalPatientManagement.Client.Models;

namespace ClinicalPatientManagement.Client.Services;

/// <summary>
/// API client interface for appointment operations
/// Step 7: Appointment Scheduling - Client API communication
/// </summary>
public interface IAppointmentApiClient
{
    /// <summary>
    /// Get all appointments
    /// </summary>
    Task<IEnumerable<AppointmentModel>> GetAllAsync();

    /// <summary>
    /// Get appointment by ID
    /// </summary>
    Task<AppointmentModel?> GetByIdAsync(int id);

    /// <summary>
    /// Create new appointment
    /// </summary>
    Task<AppointmentModel> CreateAsync(CreateAppointmentModel createModel);

    /// <summary>
    /// Update appointment
    /// </summary>
    Task<AppointmentModel> UpdateAsync(int id, UpdateAppointmentModel updateModel);

    /// <summary>
    /// Delete appointment
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Get appointments for a patient
    /// </summary>
    Task<IEnumerable<AppointmentModel>> GetByPatientIdAsync(int patientId);

    /// <summary>
    /// Get appointments by date range
    /// </summary>
    Task<IEnumerable<AppointmentModel>> GetByPatientIdAndDateRangeAsync(int patientId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get appointments by date
    /// </summary>
    Task<IEnumerable<AppointmentModel>> GetByDateAsync(DateTime date);

    /// <summary>
    /// Get upcoming appointments for a patient
    /// </summary>
    Task<IEnumerable<AppointmentModel>> GetUpcomingAppointmentsAsync(int patientId);

    /// <summary>
    /// Update appointment status
    /// </summary>
    Task<AppointmentModel> UpdateStatusAsync(int id, string status);

    /// <summary>
    /// Check if patient has appointment conflict
    /// </summary>
    Task<ConflictCheckResult> CheckConflictAsync(int patientId, DateTime appointmentDate);
}

/// <summary>
/// DTO for conflict check result
/// </summary>
public class ConflictCheckResult
{
    public bool HasConflict { get; set; }
    public string? Message { get; set; }
}
