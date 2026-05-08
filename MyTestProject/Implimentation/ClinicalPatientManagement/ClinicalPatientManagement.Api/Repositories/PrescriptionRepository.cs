using ClinicalPatientManagement.Api.Data;
using ClinicalPatientManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicalPatientManagement.Api.Repositories;

/// <summary>
/// Entity Framework Core implementation of IPrescriptionRepository
/// Step 10: Prescription Generation
/// </summary>
public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly ClinicalDbContext _context;

    public PrescriptionRepository(ClinicalDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Get all prescriptions
    /// </summary>
    public IQueryable<Prescription> GetAll()
    {
        return _context.Prescriptions
            .Include(p => p.Consultation)
            .Include(p => p.Medications)
            .OrderByDescending(p => p.CreatedAt);
    }

    /// <summary>
    /// Get prescription by ID
    /// </summary>
    public async Task<Prescription?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Prescriptions
            .Include(p => p.Consultation)
                .ThenInclude(c => c.Appointment)
                    .ThenInclude(a => a.Patient)
            .Include(p => p.Medications)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <summary>
    /// Add new prescription
    /// </summary>
    public async Task<Prescription> AddAsync(Prescription entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        entity.CreatedAt = DateTime.UtcNow;
        _context.Prescriptions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <summary>
    /// Update existing prescription
    /// </summary>
    public async Task<Prescription> UpdateAsync(Prescription entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var existingPrescription = await _context.Prescriptions
            .Include(p => p.Medications)
            .FirstOrDefaultAsync(p => p.Id == entity.Id, cancellationToken);
        
        if (existingPrescription == null)
            throw new InvalidOperationException($"Prescription with ID {entity.Id} not found");

        existingPrescription.Medications = entity.Medications;
        existingPrescription.UpdatedAt = DateTime.UtcNow;

        _context.Prescriptions.Update(existingPrescription);
        await _context.SaveChangesAsync(cancellationToken);
        return existingPrescription;
    }

    /// <summary>
    /// Delete prescription by ID
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var prescription = await _context.Prescriptions.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (prescription == null)
            return false;

        _context.Prescriptions.Remove(prescription);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Check if prescription exists
    /// </summary>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Prescriptions.AnyAsync(p => p.Id == id, cancellationToken);
    }

    /// <summary>
    /// Get count of all prescriptions
    /// </summary>
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Prescriptions.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Get prescription by consultation ID with related data
    /// </summary>
    public async Task<Prescription?> GetByConsultationIdAsync(int consultationId)
    {
        return await _context.Prescriptions
            .Include(p => p.Consultation)
                .ThenInclude(c => c.Appointment)
                    .ThenInclude(a => a.Patient)
            .Include(p => p.Medications)
            .FirstOrDefaultAsync(p => p.ConsultationId == consultationId);
    }

    /// <summary>
    /// Get all prescriptions for a patient by patient ID with medications
    /// </summary>
    public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId)
    {
        return await _context.Prescriptions
            .Where(p => p.Consultation.Appointment.PatientId == patientId)
            .Include(p => p.Consultation)
                .ThenInclude(c => c.Appointment)
            .Include(p => p.Medications)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Check if prescription exists for a consultation
    /// </summary>
    public async Task<bool> ExistsByConsultationIdAsync(int consultationId)
    {
        return await _context.Prescriptions
            .AnyAsync(p => p.ConsultationId == consultationId);
    }

    /// <summary>
    /// Create async - wrapper for AddAsync for backward compatibility with service layer
    /// </summary>
    public async Task<Prescription> CreateAsync(Prescription entity)
    {
        return await AddAsync(entity);
    }

    /// <summary>
    /// Save changes async wrapper
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
