using ClinicalPatientManagement.Api.Data;
using ClinicalPatientManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicalPatientManagement.Api.Repositories;

/// <summary>
/// Repository implementation for Consultation entity with consultation-specific queries
/// Step 9: Implement Consultation Creation - Data Access Layer
/// </summary>
public class ConsultationRepository : IConsultationRepository
{
    private readonly ClinicalDbContext _context;

    public ConsultationRepository(ClinicalDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Get all consultations ordered by date (recent first)
    /// </summary>
    public IQueryable<Consultation> GetAll()
    {
        return _context.Consultations
            .Include(c => c.Appointment)
            .OrderByDescending(c => c.CreatedAt);
    }

    /// <summary>
    /// Get consultation by ID
    /// </summary>
    public async Task<Consultation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Consultations
            .Include(c => c.Appointment)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>
    /// Add new consultation (NOTE: Changes are not persisted until UnitOfWork.SaveChangesAsync() is called)
    /// Phase 2: Architectural Improvements - Repository Pattern Fix
    /// </summary>
    public async Task<Consultation> AddAsync(Consultation entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        entity.CreatedAt = DateTime.UtcNow;
        _context.Consultations.Add(entity);
        // NOTE: SaveChangesAsync is NOT called here - handled by UnitOfWork
        await Task.CompletedTask; // Ensure this is still async-compatible
        return entity;
    }

    /// <summary>
    /// Update existing consultation (NOTE: Changes are not persisted until UnitOfWork.SaveChangesAsync() is called)
    /// Phase 2: Architectural Improvements - Repository Pattern Fix
    /// </summary>
    public async Task<Consultation> UpdateAsync(Consultation entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var existingConsultation = await _context.Consultations.FindAsync(new object[] { entity.Id }, cancellationToken: cancellationToken);
        if (existingConsultation == null)
            throw new InvalidOperationException($"Consultation with ID {entity.Id} not found");

        existingConsultation.Temperature = entity.Temperature;
        existingConsultation.BloodPressure = entity.BloodPressure;
        existingConsultation.Pulse = entity.Pulse;
        existingConsultation.Complaints = entity.Complaints;
        existingConsultation.Diagnosis = entity.Diagnosis;
        existingConsultation.UpdatedAt = DateTime.UtcNow;

        _context.Consultations.Update(existingConsultation);
        // NOTE: SaveChangesAsync is NOT called here - handled by UnitOfWork
        return existingConsultation;
    }

    /// <summary>
    /// Delete consultation by ID (NOTE: Changes are not persisted until UnitOfWork.SaveChangesAsync() is called)
    /// Phase 2: Architectural Improvements - Repository Pattern Fix
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var consultation = await _context.Consultations.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (consultation == null)
            return false;

        _context.Consultations.Remove(consultation);
        // NOTE: SaveChangesAsync is NOT called here - handled by UnitOfWork
        return true;
    }

    /// <summary>
    /// Check if consultation exists by ID
    /// </summary>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Consultations.AnyAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>
    /// Get total count of consultations
    /// </summary>
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Consultations.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Get consultation by appointment ID
    /// </summary>
    public async Task<Consultation?> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Consultations
            .FirstOrDefaultAsync(c => c.AppointmentId == appointmentId, cancellationToken);
    }

    /// <summary>
    /// Get all consultations for a specific patient by looking up their appointments
    /// </summary>
    public async Task<IEnumerable<Consultation>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Consultations
            .Where(c => c.Appointment.PatientId == patientId)
            .Include(c => c.Appointment)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Check if consultation exists for appointment
    /// </summary>
    public async Task<bool> ExistsByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Consultations.AnyAsync(c => c.AppointmentId == appointmentId, cancellationToken);
    }
}
