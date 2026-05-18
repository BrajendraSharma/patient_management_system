using AutoMapper;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text;
using ILogger = Serilog.ILogger;

namespace ClinicalPatientManagement.Api.Services;

/// <summary>
/// Service implementation for data export operations
/// Step 13: Add data export functionality for Excel and PDF formats
/// </summary>
public class ExportService : IExportService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IConsultationRepository _consultationRepository;
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public ExportService(
        IPatientRepository patientRepository,
        IConsultationRepository consultationRepository,
        IPrescriptionRepository prescriptionRepository,
        IMapper mapper)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _consultationRepository = consultationRepository ?? throw new ArgumentNullException(nameof(consultationRepository));
        _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = Log.ForContext<ExportService>();
    }

    /// <summary>
    /// Export patient/visit data to Excel or PDF format
    /// </summary>
    public async Task<ExportResponse> ExportDataAsync(ExportRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            _logger.Information("Starting export: Format={Format}, DataType={DataType}, PatientId={PatientId}", 
                request.Format, request.DataType, request.PatientId);

            // Validate format and data type early
            if (string.IsNullOrEmpty(request.Format) || (request.Format?.ToLower() != "excel" && request.Format?.ToLower() != "pdf"))
                throw new InvalidOperationException($"Unknown Format: {request.Format}");

            if (string.IsNullOrEmpty(request.DataType))
                throw new InvalidOperationException($"DataType is required");

            List<object> dataToExport = new();
            string fileName = string.Empty;

            // Fetch data based on DataType
            switch (request.DataType?.ToLower())
            {
                case "patientdata":
                    dataToExport = (await GetPatientsForExportAsync(cancellationToken)).Cast<object>().ToList();
                    fileName = $"PatientData_{DateTime.Now:yyyyMMdd_HHmmss}";
                    break;

                case "visithistory":
                    if (!request.PatientId.HasValue)
                        throw new InvalidOperationException("PatientId is required for VisitHistory export");
                    
                    dataToExport = (await GetPatientVisitsForExportAsync(
                        request.PatientId.Value, 
                        request.StartDate, 
                        request.EndDate, 
                        cancellationToken)).Cast<object>().ToList();
                    fileName = $"VisitHistory_Patient{request.PatientId}_{DateTime.Now:yyyyMMdd_HHmmss}";
                    break;

                case "prescriptiondata":
                    dataToExport = (await GetPrescriptionsForExportAsync(request.PatientId, cancellationToken)).Cast<object>().ToList();
                    fileName = $"PrescriptionData_{DateTime.Now:yyyyMMdd_HHmmss}";
                    break;

                default:
                    throw new InvalidOperationException($"Unknown DataType: {request.DataType}");
            }

            if (dataToExport.Count == 0)
            {
                _logger.Warning("No data found for export: DataType={DataType}", request.DataType);
                return new ExportResponse
                {
                    Status = "Completed",
                    FileName = fileName,
                    FileContent = string.Empty,
                    MimeType = GetMimeType(request.Format),
                    RecordCount = 0
                };
            }

            // Generate file based on Format
            string fileContent = request.Format?.ToLower() switch
            {
                "excel" => GenerateExcelContent(dataToExport),
                "pdf" => GeneratePdfContent(dataToExport, request.DataType),
                _ => string.Empty // This should not happen due to early validation
            };

            string mimeType = GetMimeType(request.Format);
            // Note: Using CSV for Excel (plain text CSV, not binary .xlsx)
            // and plain text for PDF (not binary PDF). To support true Excel/PDF,
            // consider adding EPPlus or iTextSharp libraries.
            string fileExtension = request.Format?.ToLower() == "pdf" ? ".txt" : ".csv";

            _logger.Information("Export completed: FileName={FileName}, RecordCount={RecordCount}", 
                fileName, dataToExport.Count);

            return new ExportResponse
            {
                Status = "Completed",
                FileName = $"{fileName}{fileExtension}",
                FileContent = fileContent,
                MimeType = mimeType,
                RecordCount = dataToExport.Count
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during data export");
            throw;
        }
    }

    /// <summary>
    /// Get patient history data for export with DD-MM-YYYY date formatting
    /// </summary>
    public async Task<List<VisitExportRow>> GetPatientVisitsForExportAsync(
        int patientId, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching visits for export: PatientId={PatientId}, StartDate={StartDate}, EndDate={EndDate}",
                patientId, startDate?.ToString("yyyy-MM-dd"), endDate?.ToString("yyyy-MM-dd"));

            var consultations = await _consultationRepository.GetByPatientIdAsync(patientId, cancellationToken);

            if (consultations == null)
                return new List<VisitExportRow>();

            var filtered = consultations.AsEnumerable();

            if (startDate.HasValue)
                filtered = filtered.Where(c => c.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
            {
                var endDateOnly = endDate.Value.Date.AddDays(1); // Include entire day
                filtered = filtered.Where(c => c.CreatedAt < endDateOnly);
            }

            var visits = filtered
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new VisitExportRow
                {
                    ConsultationId = c.Id,
                    PatientName = c.Appointment?.Patient != null ? 
                        $"{c.Appointment.Patient.FirstName} {c.Appointment.Patient.LastName}" : "Unknown",
                    ConsultationDate = c.CreatedAt.ToString("dd-MM-yyyy"), // DD-MM-YYYY format
                    Temperature = c.Temperature,
                    BloodPressure = c.BloodPressure,
                    Pulse = c.Pulse,
                    Complaints = c.Complaints,
                    Diagnosis = c.Diagnosis,
                    Medications = string.Empty // Will be populated from prescriptions
                })
                .ToList();

            // Add medication information
            foreach (var visit in visits)
            {
                var prescription = await _prescriptionRepository.GetByConsultationIdAsync(visit.ConsultationId);
                if (prescription?.Medications != null && prescription.Medications.Count > 0)
                {
                    visit.Medications = string.Join(", ", 
                        prescription.Medications.Select(m => $"{m.Name} ({m.Dosage})"));
                }
            }

            return visits;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching visits for export");
            throw;
        }
    }

    /// <summary>
    /// Get all patients data for export with DD-MM-YYYY date formatting
    /// </summary>
    public async Task<List<PatientExportRow>> GetPatientsForExportAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching all patients for export");

            var patients = _patientRepository.GetAll().ToList();

            var patientRows = patients
                .Select(p => new PatientExportRow
                {
                    PatientId = p.Id,
                    FullName = $"{p.FirstName} {p.LastName}",
                    PhoneNumber = p.Phone,
                    Email = p.Email,
                    Age = CalculateAge(p.DateOfBirth),
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth.ToString("dd-MM-yyyy") // DD-MM-YYYY format
                })
                .OrderBy(p => p.FullName)
                .ToList();

            return await Task.FromResult(patientRows);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching patients for export");
            throw;
        }
    }

    /// <summary>
    /// Get prescription data for export with DD-MM-YYYY date formatting
    /// </summary>
    public async Task<List<PrescriptionExportRow>> GetPrescriptionsForExportAsync(int? patientId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Information("Fetching prescriptions for export: PatientId={PatientId}", patientId);

            var prescriptions = _prescriptionRepository.GetAll().ToList();

            if (patientId.HasValue)
            {
                prescriptions = prescriptions
                    .Where(p => p.Consultation?.Appointment?.PatientId == patientId.Value)
                    .ToList();
            }

            var prescriptionRows = new List<PrescriptionExportRow>();

            foreach (var prescription in prescriptions)
            {
                if (prescription.Medications == null || prescription.Medications.Count == 0)
                    continue;

                var patient = prescription.Consultation?.Appointment?.Patient;
                var patientName = patient != null ? 
                    $"{patient.FirstName} {patient.LastName}" : "Unknown";

                foreach (var medication in prescription.Medications)
                {
                    prescriptionRows.Add(new PrescriptionExportRow
                    {
                        PrescriptionId = prescription.Id,
                        PatientName = patientName,
                        PrescriptionDate = prescription.PrescriptionDate.ToString("dd-MM-yyyy"), // DD-MM-YYYY format
                        MedicationName = medication.Name,
                        Dosage = medication.Dosage,
                        Frequency = medication.Frequency,
                        Duration = medication.Duration,
                        Instructions = medication.Instructions
                    });
                }
            }

            return await Task.FromResult(prescriptionRows.OrderByDescending(p => p.PrescriptionDate).ToList());
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error fetching prescriptions for export");
            throw;
        }
    }

    /// <summary>
    /// Generate Excel file content as CSV for compatibility
    /// Note: For full Excel support, consider using EPPlus or ClosedXML libraries
    /// </summary>
    private string GenerateExcelContent(List<object> data)
    {
        if (data.Count == 0)
            return string.Empty;

        try
        {
            var sb = new StringBuilder();
            var firstItem = data.First();
            var properties = firstItem.GetType().GetProperties();

            // Write header row
            sb.AppendLine(string.Join(",", properties.Select(p => QuoteCsv(p.Name))));

            // Write data rows
            foreach (var item in data)
            {
                var values = properties.Select(p => QuoteCsv(p.GetValue(item)?.ToString() ?? ""));
                sb.AppendLine(string.Join(",", values));
            }

            // Convert to CSV as base64 (Excel can open CSV files)
            var csvBytes = Encoding.UTF8.GetBytes(sb.ToString());
            return Convert.ToBase64String(csvBytes);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating Excel content");
            throw;
        }
    }

    /// <summary>
    /// Generate PDF content as formatted text
    /// Note: For full PDF support, consider using iTextSharp or SelectPdf libraries
    /// </summary>
    private string GeneratePdfContent(List<object> data, string dataType)
    {
        if (data.Count == 0)
            return string.Empty;

        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("================================================================================");
            sb.AppendLine($"Clinical Patient Management System - {dataType} Report");
            sb.AppendLine($"Generated: {DateTime.Now:dd-MM-yyyy HH:mm:ss}");
            sb.AppendLine("================================================================================");
            sb.AppendLine();

            var firstItem = data.First();
            var properties = firstItem.GetType().GetProperties();

            // Write header
            foreach (var prop in properties)
            {
                sb.Append($"{prop.Name,-30}");
            }
            sb.AppendLine();
            sb.AppendLine(new string('-', 150));

            // Write data rows
            foreach (var item in data)
            {
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(item)?.ToString() ?? "";
                    sb.Append($"{value,-30}");
                }
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine($"Total Records: {data.Count}");
            sb.AppendLine("================================================================================");

            // Convert to base64 for transmission
            var pdfBytes = Encoding.UTF8.GetBytes(sb.ToString());
            return Convert.ToBase64String(pdfBytes);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating PDF content");
            throw;
        }
    }

    /// <summary>
    /// Quote CSV values to handle commas and quotes
    /// </summary>
    private string QuoteCsv(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }

    /// <summary>
    /// Get MIME type based on format
    /// Note: Using text/csv for Excel (since we generate CSV, not binary Excel)
    /// and text/plain for PDF (since we generate plain text, not binary PDF)
    /// For true Excel/PDF support, add EPPlus or iTextSharp libraries
    /// </summary>
    private string GetMimeType(string format)
    {
        return format?.ToLower() switch
        {
            "excel" => "text/csv", // Excel format generates CSV text
            "pdf" => "text/plain", // PDF format generates plain text
            _ => "text/plain"
        };
    }

    /// <summary>
    /// Calculate age from date of birth
    /// </summary>
    private int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age))
            age--;
        return age;
    }
}
