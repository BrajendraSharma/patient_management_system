using ClinicalPatientManagement.Api.Data;
using ClinicalPatientManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicalPatientManagement.Api.Repositories;

/// <summary>
/// Repository implementation for Appointment entity
/// Step 7: Appointment Scheduling - Data Access Layer
/// </summary>
public class AppointmentRepository : IAppointmentRepository
{
    private readonly ClinicalDbContext _context;

    public AppointmentRepository(ClinicalDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Get all appointments ordered by date
    /// </summary>
    public IQueryable<Appointment> GetAll()
    {
        return _context.Appointments
            .Include(a => a.Patient)
            .OrderByDescending(a => a.AppointmentDate);
    }

    /// <summary>
    /// Get appointment by ID with patient details
    /// </summary>
    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    /// <summary>
    /// Add new appointment
    /// </summary>
    public async Task<Appointment> AddAsync(Appointment entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Validate that patient exists
        var patientExists = await _context.Patients.AnyAsync(p => p.Id == entity.PatientId, cancellationToken);
        if (!patientExists)
            throw new InvalidOperationException($"Patient with ID {entity.PatientId} not found");

        entity.CreatedAt = DateTime.UtcNow;
        _context.Appointments.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <summary>
    /// Update existing appointment
    /// </summary>
    public async Task<Appointment> UpdateAsync(Appointment entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var existingAppointment = await _context.Appointments.FindAsync(new object[] { entity.Id }, cancellationToken: cancellationToken);
        if (existingAppointment == null)
            throw new InvalidOperationException($"Appointment with ID {entity.Id} not found");

        existingAppointment.PatientId = entity.PatientId;
        existingAppointment.AppointmentDate = entity.AppointmentDate;
        existingAppointment.Status = entity.Status;
        existingAppointment.Notes = entity.Notes;
        existingAppointment.UpdatedAt = DateTime.UtcNow;

        _context.Appointments.Update(existingAppointment);
        await _context.SaveChangesAsync(cancellationToken);
        return existingAppointment;
    }

    /// <summary>
    /// Delete appointment by ID
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (appointment == null)
            return false;

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Check if appointment exists
    /// </summary>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments.AnyAsync(a => a.Id == id, cancellationToken);
    }

    /// <summary>
    /// Get count of all appointments
    /// </summary>
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Appointments.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Get all appointments for a specific patient
    /// </summary>
    public IQueryable<Appointment> GetByPatientId(int patientId)
    {
        return _context.Appointments
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate);
    }

    /// <summary>
    /// Get appointments by date range for a specific patient
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetByPatientIdAndDateRangeAsync(int patientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.PatientId == patientId
                && a.AppointmentDate >= startDate
                && a.AppointmentDate <= endDate)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get appointments scheduled for a specific date
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        return await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.AppointmentDate >= startOfDay && a.AppointmentDate <= endOfDay)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get all appointments for a specific date with status filter
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetByDateAndStatusAsync(DateTime date, string status, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        return await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.AppointmentDate >= startOfDay
                && a.AppointmentDate <= endOfDay
                && a.Status == status)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Update appointment status
    /// </summary>
    public async Task<Appointment> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (appointment == null)
            throw new InvalidOperationException($"Appointment with ID {id} not found");

        var validStatuses = new[] { "Scheduled", "Completed", "Cancelled", "No-Show" };
        if (!validStatuses.Contains(status))
            throw new ArgumentException($"Invalid status: {status}. Valid statuses are: {string.Join(", ", validStatuses)}");

        appointment.Status = status;
        appointment.UpdatedAt = DateTime.UtcNow;

        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync(cancellationToken);
        return appointment;
    }

    /// <summary>
    /// Get upcoming appointments for a patient (future dates only)
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.PatientId == patientId && a.AppointmentDate > DateTime.UtcNow)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Check if patient has appointment conflict at specified date and time
    /// </summary>
    public async Task<bool> HasConflictAsync(int patientId, DateTime appointmentDate, CancellationToken cancellationToken = default)
    {
        // Allow 30-minute buffer between appointments
        var bufferMinutes = 30;
        var startBuffer = appointmentDate.AddMinutes(-bufferMinutes);
        var endBuffer = appointmentDate.AddMinutes(bufferMinutes);

        return await _context.Appointments
            .AnyAsync(a => a.PatientId == patientId
                && a.AppointmentDate >= startBuffer
                && a.AppointmentDate <= endBuffer
                && (a.Status == "Scheduled" || a.Status == "Completed"),
                cancellationToken);
    }
}
