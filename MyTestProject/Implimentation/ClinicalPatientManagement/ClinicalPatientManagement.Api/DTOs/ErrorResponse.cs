namespace ClinicalPatientManagement.Api.DTOs;

/// <summary>
/// Standard error response format for all API errors
/// Phase 2: Architectural Improvements - Global Exception Handling
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description (not shown in production)
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Request path that caused the error
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Timestamp of the error
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Request correlation ID for tracking
    /// </summary>
    public string? TraceId { get; set; }
}
