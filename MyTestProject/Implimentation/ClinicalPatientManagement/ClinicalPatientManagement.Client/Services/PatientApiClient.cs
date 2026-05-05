using ClinicalPatientManagement.Client.Models;
using System.Net.Http.Json;

namespace ClinicalPatientManagement.Client.Services;

/// <summary>
/// API client implementation for patient operations
/// Step 6: Patient Management - Client API communication
/// </summary>
public class PatientApiClient : IPatientApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUri = "api/patients";

    public PatientApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Get all patients
    /// </summary>
    public async Task<IEnumerable<PatientModel>> GetAllAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PatientModel>>(_baseUri) ?? new List<PatientModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching patients: {ex.Message}");
            return new List<PatientModel>();
        }
    }

    /// <summary>
    /// Get patient by ID
    /// </summary>
    public async Task<PatientModel?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PatientModel>($"{_baseUri}/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching patient {id}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Create new patient
    /// </summary>
    public async Task<PatientModel> CreateAsync(CreatePatientModel createModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUri, createModel);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PatientModel>() ?? new PatientModel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating patient: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Update patient
    /// </summary>
    public async Task<PatientModel> UpdateAsync(int id, UpdatePatientModel updateModel)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUri}/{id}", updateModel);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PatientModel>() ?? new PatientModel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating patient {id}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Delete patient
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUri}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting patient {id}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Search patients by name or phone
    /// </summary>
    public async Task<IEnumerable<PatientModel>> SearchAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            return await _httpClient.GetFromJsonAsync<IEnumerable<PatientModel>>($"{_baseUri}/search/{Uri.EscapeDataString(searchTerm)}") ?? new List<PatientModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching patients: {ex.Message}");
            return new List<PatientModel>();
        }
    }
}
