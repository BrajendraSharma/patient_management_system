using ClinicalPatientManagement.Api.Models;

namespace ClinicalPatientManagement.Api.Repositories;

/// <summary>
/// Repository interface for appointment data access operations
/// Step 7: Appointment Scheduling - Data Access Layer
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    /// <summary>
    /// Get all appointments for a specific patient
    /// </summary>
    IQueryable<Appointment> GetByPatientId(int patientId);

    /// <summary>
    /// Get appointments by date range for a specific patient
    /// </summary>
    Task<IEnumerable<Appointment>> GetByPatientIdAndDateRangeAsync(int patientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointments scheduled for a specific date
    /// </summary>
    Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all appointments for a specific date with status filter
    /// </summary>
    Task<IEnumerable<Appointment>> GetByDateAndStatusAsync(DateTime date, string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update appointment status
    /// </summary>
    Task<Appointment> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get upcoming appointments for a patient (future dates only)
    /// </summary>
    Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if patient has appointment at specified date and time
    /// </summary>
    Task<bool> HasConflictAsync(int patientId, DateTime appointmentDate, CancellationToken cancellationToken = default);
}
