namespace ClinicalPatientManagement.Client.Services;

/// <summary>
/// Base service for HTTP communication with API
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Make GET request to API
    /// </summary>
    Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Make POST request to API
    /// </summary>
    Task<T?> PostAsync<T>(string endpoint, object? data = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Make PUT request to API
    /// </summary>
    Task<T?> PutAsync<T>(string endpoint, object? data = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Make DELETE request to API
    /// </summary>
    Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
}
