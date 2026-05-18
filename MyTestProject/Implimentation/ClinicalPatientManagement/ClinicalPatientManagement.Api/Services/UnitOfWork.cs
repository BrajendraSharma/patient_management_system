using ClinicalPatientManagement.Api.Data;
using ClinicalPatientManagement.Api.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ClinicalPatientManagement.Api.Services;

/// <summary>
/// Unit of Work implementation for managing transactions across repositories
/// Phase 2: Architectural Improvements - Transaction Management
/// Ensures all repository operations are coordinated under a single transaction
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ClinicalDbContext _context;
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IConsultationRepository _consultationRepository;
    private IDbContextTransaction? _transaction;
    private readonly ILogger _logger;

    public IPatientRepository Patients => _patientRepository;
    public IAppointmentRepository Appointments => _appointmentRepository;
    public IConsultationRepository Consultations => _consultationRepository;

    public UnitOfWork(
        ClinicalDbContext context,
        IPatientRepository patientRepository,
        IAppointmentRepository appointmentRepository,
        IConsultationRepository consultationRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        _consultationRepository = consultationRepository ?? throw new ArgumentNullException(nameof(consultationRepository));
        _logger = Log.ForContext<UnitOfWork>();
    }

    /// <summary>
    /// Save all pending changes in a single transaction
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Saving changes to database");
            var affectedRows = await _context.SaveChangesAsync(cancellationToken);
            _logger.Information("Changes saved successfully, affected rows: {AffectedRows}", affectedRows);
            return affectedRows;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving changes to database");
            throw;
        }
    }

    /// <summary>
    /// Begin a new transaction
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            _logger.Warning("Transaction already active, consider committing or rolling back first");
            return;
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _logger.Information("Transaction started");
    }

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction == null)
            {
                _logger.Warning("No active transaction to commit");
                return;
            }

            await _transaction.CommitAsync(cancellationToken);
            _logger.Information("Transaction committed successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error committing transaction");
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction == null)
            {
                _logger.Warning("No active transaction to rollback");
                return;
            }

            await _transaction.RollbackAsync(cancellationToken);
            _logger.Information("Transaction rolled back successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error rolling back transaction");
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }

        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
