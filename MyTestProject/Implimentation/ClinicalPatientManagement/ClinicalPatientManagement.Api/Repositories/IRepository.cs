namespace ClinicalPatientManagement.Api.Repositories;

/// <summary>
/// Generic repository interface for data access operations
/// </summary>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get all entities
    /// </summary>
    IQueryable<T> GetAll();

    /// <summary>
    /// Get entity by ID
    /// </summary>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add entity
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update entity
    /// </summary>
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete entity by ID
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if entity exists
    /// </summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of all entities
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
