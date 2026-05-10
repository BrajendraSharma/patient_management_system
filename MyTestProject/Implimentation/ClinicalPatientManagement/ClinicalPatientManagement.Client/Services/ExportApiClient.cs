using System.Text.Json;
using System.Net.Http.Json;

namespace ClinicalPatientManagement.Client.Services;

/// <summary>
/// Client implementation for API export operations
/// Step 13: Add data export
/// </summary>
public class ExportApiClient : IExportApiClient
{
    private readonly HttpClient _httpClient;

    public ExportApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Export data to Excel or PDF format
    /// </summary>
    public async Task<byte[]> ExportDataAsync(
        string format,
        string dataType,
        int? patientId = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        try
        {
            var request = new
            {
                Format = format,
                DataType = dataType,
                PatientId = patientId,
                StartDate = startDate,
                EndDate = endDate
            };

            var response = await _httpClient.PostAsJsonAsync("/api/export", request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Export failed with status {response.StatusCode}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting data: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Get supported export formats
    /// </summary>
    public async Task<IEnumerable<string>> GetSupportedFormatsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<string>>("/api/export/formats") 
                ?? Enumerable.Empty<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting export formats: {ex.Message}");
            return Enumerable.Empty<string>();
        }
    }

    /// <summary>
    /// Get supported export data types
    /// </summary>
    public async Task<IEnumerable<string>> GetSupportedDataTypesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<string>>("/api/export/data-types")
                ?? Enumerable.Empty<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting export data types: {ex.Message}");
            return Enumerable.Empty<string>();
        }
    }

    /// <summary>
    /// Get patient visits for export
    /// </summary>
    public async Task<IEnumerable<VisitExportRow>> GetPatientVisitsAsync(
        int patientId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        try
        {
            var query = $"/api/export/patient/{patientId}/visits";
            if (startDate.HasValue || endDate.HasValue)
            {
                var queryParams = new List<string>();
                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate:yyyy-MM-dd}");
                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate:yyyy-MM-dd}");

                if (queryParams.Count > 0)
                    query += "?" + string.Join("&", queryParams);
            }

            return await _httpClient.GetFromJsonAsync<IEnumerable<VisitExportRow>>(
                query,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? Enumerable.Empty<VisitExportRow>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting patient visits: {ex.Message}");
            return Enumerable.Empty<VisitExportRow>();
        }
    }

    /// <summary>
    /// Get all patients for export
    /// </summary>
    public async Task<IEnumerable<PatientExportRow>> GetPatientsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<PatientExportRow>>(
                "/api/export/patients",
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? Enumerable.Empty<PatientExportRow>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting patients: {ex.Message}");
            return Enumerable.Empty<PatientExportRow>();
        }
    }

    /// <summary>
    /// Get prescriptions for export
    /// </summary>
    public async Task<IEnumerable<PrescriptionExportRow>> GetPrescriptionsAsync(int? patientId = null)
    {
        try
        {
            var query = "/api/export/prescriptions";
            if (patientId.HasValue)
                query += $"?patientId={patientId}";

            return await _httpClient.GetFromJsonAsync<IEnumerable<PrescriptionExportRow>>(
                query,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? Enumerable.Empty<PrescriptionExportRow>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting prescriptions: {ex.Message}");
            return Enumerable.Empty<PrescriptionExportRow>();
        }
    }
}
