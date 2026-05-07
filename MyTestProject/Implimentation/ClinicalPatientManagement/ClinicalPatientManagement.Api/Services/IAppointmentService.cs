using ClinicalPatientManagement.Api.DTOs;

namespace ClinicalPatientManagement.Api.Services;

/// <summary>
/// Service interface for appointment management operations
/// Step 7: Appointment Scheduling - Business Logic Layer
/// </summary>
public interface IAppointmentService
{
    /// <summary>
    /// Get all appointments
    /// </summary>
    Task<IEnumerable<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointment by ID
    /// </summary>
    Task<AppointmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create new appointment with validation
    /// </summary>
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto createDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing appointment with validation
    /// </summary>
    Task<AppointmentDto> UpdateAsync(int id, UpdateAppointmentDto updateDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete appointment by ID
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointments for a specific patient
    /// </summary>
    Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointments by date range for a patient
    /// </summary>
    Task<IEnumerable<AppointmentDto>> GetByPatientIdAndDateRangeAsync(int patientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointments scheduled for a specific date
    /// </summary>
    Task<IEnumerable<AppointmentDto>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointments with specific status on a given date
    /// </summary>
    Task<IEnumerable<AppointmentDto>> GetByDateAndStatusAsync(DateTime date, string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update appointment status
    /// </summary>
    Task<AppointmentDto> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get upcoming appointments for a patient
    /// </summary>
    Task<IEnumerable<AppointmentDto>> GetUpcomingAppointmentsAsync(int patientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if patient has appointment conflict
    /// </summary>
    Task<bool> HasConflictAsync(int patientId, DateTime appointmentDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if appointment exists
    /// </summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate appointment data
    /// </summary>
    bool ValidateAppointmentData(CreateAppointmentDto dto, out List<string> errors);
}
