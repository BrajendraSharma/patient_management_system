using ClinicalPatientManagement.Client.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClinicalPatientManagement.Client.Services;

/// <summary>
/// API client implementation for consultation operations
/// Step 9: Implement Consultation Creation - Client API communication
/// Step 12: Implement Patient History - Get consultation history with date filtering
/// </summary>
public class ConsultationApiClient : IConsultationApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUri = "api/consultations";

    public ConsultationApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Get all consultations
    /// </summary>
    public async Task<IEnumerable<ConsultationModel>> GetAllAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ConsultationModel>>(_baseUri) 
                ?? new List<ConsultationModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching consultations: {ex.Message}");
            return new List<ConsultationModel>();
        }
    }

    /// <summary>
    /// Get consultation by ID
    /// </summary>
    public async Task<ConsultationModel?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ConsultationModel>($"{_baseUri}/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching consultation {id}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Create new consultation
    /// </summary>
    public async Task<ConsultationModel> CreateAsync(CreateConsultationModel createModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUri, createModel);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ConsultationModel>(json)
                ?? throw new InvalidOperationException("Failed to create consultation");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating consultation: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Update consultation
    /// </summary>
    public async Task<ConsultationModel> UpdateAsync(int id, UpdateConsultationModel updateModel)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUri}/{id}", updateModel);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ConsultationModel>(json)
                ?? throw new InvalidOperationException("Failed to update consultation");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating consultation {id}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Delete consultation
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
            Console.WriteLine($"Error deleting consultation {id}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get consultation by appointment ID
    /// </summary>
    public async Task<ConsultationModel?> GetByAppointmentIdAsync(int appointmentId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ConsultationModel>(
                $"{_baseUri}/appointment/{appointmentId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching consultation for appointment {appointmentId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get all consultations for a patient
    /// </summary>
    public async Task<IEnumerable<ConsultationModel>> GetByPatientIdAsync(int patientId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ConsultationModel>>(
                $"{_baseUri}/patient/{patientId}") 
                ?? new List<ConsultationModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching consultations for patient {patientId}: {ex.Message}");
            return new List<ConsultationModel>();
        }
    }

    /// <summary>
    /// Get patient consultation history with optional date filtering
    /// Step 12: Implement Patient History - View past visits with date filtering
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="startDate">Start date for filtering (optional)</param>
    /// <param name="endDate">End date for filtering (optional)</param>
    /// <returns>Filtered consultations ordered by date descending</returns>
    public async Task<IEnumerable<ConsultationModel>?> GetPatientHistoryAsync(
        int patientId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate:yyyy-MM-dd}");
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate:yyyy-MM-dd}");

            var query = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : "";
            var url = $"{_baseUri}/history/{patientId}{query}";

            return await _httpClient.GetFromJsonAsync<IEnumerable<ConsultationModel>>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching consultation history for patient {patientId}: {ex.Message}");
            return null;
        }
    }
}
