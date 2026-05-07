using ClinicalPatientManagement.Client.Models;
using System.Net.Http.Json;

namespace ClinicalPatientManagement.Client.Services;

/// <summary>
/// API client implementation for appointment operations
/// Step 7: Appointment Scheduling - Client API communication
/// </summary>
public class AppointmentApiClient : IAppointmentApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUri = "api/appointments";

    public AppointmentApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Get all appointments
    /// </summary>
    public async Task<IEnumerable<AppointmentModel>> GetAllAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AppointmentModel>>(_baseUri) ?? new List<AppointmentModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching appointments: {ex.Message}");
            return new List<AppointmentModel>();
        }
    }

    /// <summary>
    /// Get appointment by ID
    /// </summary>
    public async Task<AppointmentModel?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AppointmentModel>($"{_baseUri}/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching appointment {id}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Create new appointment
    /// </summary>
    public async Task<AppointmentModel> CreateAsync(CreateAppointmentModel createModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUri, createModel);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppointmentModel>() ?? new AppointmentModel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating appointment: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Update appointment
    /// </summary>
    public async Task<AppointmentModel> UpdateAsync(int id, UpdateAppointmentModel updateModel)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUri}/{id}", updateModel);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppointmentModel>() ?? new AppointmentModel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating appointment: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Delete appointment
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
            Console.WriteLine($"Error deleting appointment: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Get appointments for a patient
    /// </summary>
    public async Task<IEnumerable<AppointmentModel>> GetByPatientIdAsync(int patientId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AppointmentModel>>($"{_baseUri}/patient/{patientId}") ?? new List<AppointmentModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching appointments for patient {patientId}: {ex.Message}");
            return new List<AppointmentModel>();
        }
    }

    /// <summary>
    /// Get appointments by date range
    /// </summary>
    public async Task<IEnumerable<AppointmentModel>> GetByPatientIdAndDateRangeAsync(int patientId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var url = $"{_baseUri}/patient/{patientId}/range?startDate={startDate:O}&endDate={endDate:O}";
            return await _httpClient.GetFromJsonAsync<IEnumerable<AppointmentModel>>(url) ?? new List<AppointmentModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching appointments for date range: {ex.Message}");
            return new List<AppointmentModel>();
        }
    }

    /// <summary>
    /// Get appointments by date
    /// </summary>
    public async Task<IEnumerable<AppointmentModel>> GetByDateAsync(DateTime date)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AppointmentModel>>($"{_baseUri}/date/{date:O}") ?? new List<AppointmentModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching appointments for date: {ex.Message}");
            return new List<AppointmentModel>();
        }
    }

    /// <summary>
    /// Get upcoming appointments for a patient
    /// </summary>
    public async Task<IEnumerable<AppointmentModel>> GetUpcomingAppointmentsAsync(int patientId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AppointmentModel>>($"{_baseUri}/upcoming/{patientId}") ?? new List<AppointmentModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching upcoming appointments: {ex.Message}");
            return new List<AppointmentModel>();
        }
    }

    /// <summary>
    /// Update appointment status
    /// </summary>
    public async Task<AppointmentModel> UpdateStatusAsync(int id, string status)
    {
        try
        {
            var response = await _httpClient.PatchAsJsonAsync($"{_baseUri}/{id}/status", new { status });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AppointmentModel>() ?? new AppointmentModel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating appointment status: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Check if patient has appointment conflict
    /// </summary>
    public async Task<ConflictCheckResult> CheckConflictAsync(int patientId, DateTime appointmentDate)
    {
        try
        {
            var url = $"{_baseUri}/conflict?patientId={patientId}&appointmentDate={appointmentDate:O}";
            return await _httpClient.GetFromJsonAsync<ConflictCheckResult>(url) ?? new ConflictCheckResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking appointment conflict: {ex.Message}");
            return new ConflictCheckResult { HasConflict = false };
        }
    }
}
