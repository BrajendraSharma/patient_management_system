using ClinicalPatientManagement.Api.Data;
using ClinicalPatientManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicalPatientManagement.Api.Repositories;

/// <summary>
/// Repository implementation for Patient entity with search functionality
/// Step 6: Patient Management - Data Access Layer
/// </summary>
public class PatientRepository : IPatientRepository
{
    private readonly ClinicalDbContext _context;

    public PatientRepository(ClinicalDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Get all patients with sorted order and eager-loaded appointments (prevents N+1 queries)
    /// Phase 3: Performance Optimization - N+1 Query Fix
    /// </summary>
    public IQueryable<Patient> GetAll()
    {
        return _context.Patients
            .Include(p => p.Appointments)
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.LastName);
    }

    /// <summary>
    /// Get patient by ID with eager-loaded appointments (prevents N+1 queries)
    /// Phase 3: Performance Optimization - N+1 Query Fix
    /// </summary>
    public async Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <summary>
    /// Add new patient (NOTE: Changes are not persisted until UnitOfWork.SaveChangesAsync() is called)
    /// Phase 2: Architectural Improvements - Repository Pattern Fix
    /// </summary>
    public async Task<Patient> AddAsync(Patient entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        entity.CreatedAt = DateTime.UtcNow;
        _context.Patients.Add(entity);
        // NOTE: SaveChangesAsync is NOT called here - handled by UnitOfWork
        await Task.CompletedTask; // Ensure this is still async-compatible
        return entity;
    }

    /// <summary>
    /// Update existing patient (NOTE: Changes are not persisted until UnitOfWork.SaveChangesAsync() is called)
    /// Phase 2: Architectural Improvements - Repository Pattern Fix
    /// </summary>
    public async Task<Patient> UpdateAsync(Patient entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var existingPatient = await _context.Patients.FindAsync(new object[] { entity.Id }, cancellationToken: cancellationToken);
        if (existingPatient == null)
            throw new InvalidOperationException($"Patient with ID {entity.Id} not found");

        existingPatient.FirstName = entity.FirstName;
        existingPatient.LastName = entity.LastName;
        existingPatient.Phone = entity.Phone;
        existingPatient.Email = entity.Email;
        existingPatient.DateOfBirth = entity.DateOfBirth;
        existingPatient.Gender = entity.Gender;
        existingPatient.UpdatedAt = DateTime.UtcNow;

        _context.Patients.Update(existingPatient);
        // NOTE: SaveChangesAsync is NOT called here - handled by UnitOfWork
        return existingPatient;
    }

    /// <summary>
    /// Delete patient by ID (NOTE: Changes are not persisted until UnitOfWork.SaveChangesAsync() is called)
    /// Phase 2: Architectural Improvements - Repository Pattern Fix
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (patient == null)
            return false;

        _context.Patients.Remove(patient);
        // NOTE: SaveChangesAsync is NOT called here - handled by UnitOfWork
        return true;
    }

    /// <summary>
    /// Check if patient exists by ID
    /// </summary>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AnyAsync(p => p.Id == id, cancellationToken);
    }

    /// <summary>
    /// Get total count of patients
    /// </summary>
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Patients.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Search patients by name or phone (case-insensitive, partial match)
    /// Results ordered by CreatedAt DESC (most recent first)
    /// Requirement Step 8: Enhance patient search with recent-first ordering
    /// </summary>
    public async Task<IList<Patient>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAll()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);

        var lowerSearch = searchTerm.ToLower();
        return await _context.Patients
            .Where(p => p.FirstName.ToLower().Contains(lowerSearch) ||
                        p.LastName.ToLower().Contains(lowerSearch) ||
                        p.Phone.ToLower().Contains(lowerSearch)) // Case-insensitive for phone
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
