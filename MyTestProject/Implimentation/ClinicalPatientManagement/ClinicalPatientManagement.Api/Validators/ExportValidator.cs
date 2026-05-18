namespace ClinicalPatientManagement.Api.Validators;

/// <summary>
/// Validator for export format and parameters
/// Phase 2: Architectural Improvements - Export Service Enhancement
/// </summary>
public static class ExportValidator
{
    public enum ExportFormat
    {
        Csv,
        Json,
        Excel,
        Pdf
    }

    private static readonly HashSet<string> ValidFormats = new(StringComparer.OrdinalIgnoreCase)
    {
        "csv",
        "json",
        "excel",
        "xlsx",
        "pdf"
    };

    /// <summary>
    /// Validate export format
    /// </summary>
    public static bool IsValidFormat(string? format)
    {
        if (string.IsNullOrWhiteSpace(format))
            return false;

        return ValidFormats.Contains(format.Trim());
    }

    /// <summary>
    /// Parse string format to enum
    /// </summary>
    public static ExportFormat ParseFormat(string format)
    {
        var normalized = format.ToLowerInvariant().Trim();

        return normalized switch
        {
            "csv" => ExportFormat.Csv,
            "json" => ExportFormat.Json,
            "excel" or "xlsx" => ExportFormat.Excel,
            "pdf" => ExportFormat.Pdf,
            _ => throw new ArgumentException($"Unsupported export format: {format}")
        };
    }

    /// <summary>
    /// Get file extension for format
    /// </summary>
    public static string GetFileExtension(ExportFormat format)
    {
        return format switch
        {
            ExportFormat.Csv => ".csv",
            ExportFormat.Json => ".json",
            ExportFormat.Excel => ".xlsx",
            ExportFormat.Pdf => ".pdf",
            _ => throw new ArgumentException($"Unknown format: {format}")
        };
    }

    /// <summary>
    /// Get MIME type for format
    /// </summary>
    public static string GetMimeType(ExportFormat format)
    {
        return format switch
        {
            ExportFormat.Csv => "text/csv",
            ExportFormat.Json => "application/json",
            ExportFormat.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ExportFormat.Pdf => "application/pdf",
            _ => "application/octet-stream"
        };
    }
}
