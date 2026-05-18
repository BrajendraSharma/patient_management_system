using ClinicalPatientManagement.Api.Data;
using ClinicalPatientManagement.Api.Repositories;

namespace ClinicalPatientManagement.Api.Services;

/// <summary>
/// Unit of Work pattern for managing transactions and repository coordination
/// Phase 2: Architectural Improvements - Transaction Management
/// Ensures ACID compliance for multi-repository operations
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Patient repository
    /// </summary>
    IPatientRepository Patients { get; }

    /// <summary>
    /// Appointment repository
    /// </summary>
    IAppointmentRepository Appointments { get; }

    /// <summary>
    /// Consultation repository
    /// </summary>
    IConsultationRepository Consultations { get; }

    /// <summary>
    /// Save all pending changes as a single transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of entities affected</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin a new transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction object</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
