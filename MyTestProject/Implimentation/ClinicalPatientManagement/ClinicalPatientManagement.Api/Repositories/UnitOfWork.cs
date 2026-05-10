using ClinicalPatientManagement.Api.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ClinicalPatientManagement.Api.Repositories;

/// <summary>
/// Unit of Work pattern implementation for managing transactions across repositories
/// Step 11: Persist consultations with transactions - ACID compliance
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ClinicalDbContext _dbContext;
    private IDbContextTransaction? _transaction;
    private readonly ILogger _logger;

    public UnitOfWork(ClinicalDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = Log.ForContext<UnitOfWork>();
    }

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Debug("Saving changes to database");
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.Debug("Successfully saved {ChangeCount} changes to database", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving changes to database");
            throw;
        }
    }

    /// <summary>
    /// Begin a transaction with isolation level Read Committed
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // Only begin a transaction if one is not already active
        if (_transaction != null)
        {
            _logger.Warning("Transaction already active, skipping BeginTransactionAsync");
            return;
        }

        try
        {
            _logger.Information("Beginning database transaction");
            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            _logger.Debug("Transaction started with ID: {TransactionId}", _transaction.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error beginning transaction");
            throw;
        }
    }

    /// <summary>
    /// Commit the current transaction
    /// Ensures all changes are persisted atomically
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            _logger.Warning("No active transaction to commit");
            return;
        }

        try
        {
            _logger.Information("Committing database transaction");
            
            // Save changes before commit
            await SaveChangesAsync(cancellationToken);
            
            // Commit the transaction
            await _transaction.CommitAsync(cancellationToken);
            
            _logger.Information("Transaction committed successfully");
            _transaction = null;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error committing transaction");
            // Attempt rollback on commit failure
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Rollback the current transaction
    /// Reverts all pending changes
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            _logger.Warning("No active transaction to rollback");
            return;
        }

        try
        {
            _logger.Information("Rolling back database transaction");
            await _transaction.RollbackAsync(cancellationToken);
            _logger.Information("Transaction rolled back successfully");
            _transaction = null;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error rolling back transaction");
            throw;
        }
    }

    /// <summary>
    /// Dispose pattern: Clean up transaction resources
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            _logger.Warning("Disposing UnitOfWork with active transaction, rolling back");
            await RollbackTransactionAsync();
        }

        await _dbContext.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
