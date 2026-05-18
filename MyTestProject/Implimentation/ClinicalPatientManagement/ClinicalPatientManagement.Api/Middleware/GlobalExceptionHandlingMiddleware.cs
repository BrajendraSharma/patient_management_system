using System.Net;
using ClinicalPatientManagement.Api.DTOs;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ClinicalPatientManagement.Api.Middleware;

/// <summary>
/// Global exception handling middleware to catch and standardize all API errors
/// Phase 2: Architectural Improvements - Consistent Error Responses
/// Catches unhandled exceptions and returns standardized ErrorResponse
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = Log.ForContext<GlobalExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// Process the HTTP request and catch any exceptions
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unhandled exception in request to {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handle exception and write error response
    /// </summary>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            StatusCode = context.Response.StatusCode,
            Message = GetUserFriendlyMessage(exception),
            Path = context.Request.Path,
            TraceId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        // Only include detailed information in development
        if (!IsProduction(context))
        {
            response.Details = exception.ToString();
        }

        // Map exception types to HTTP status codes
        context.Response.StatusCode = exception switch
        {
            ArgumentNullException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        response.StatusCode = context.Response.StatusCode;

        return context.Response.WriteAsJsonAsync(response);
    }

    /// <summary>
    /// Get user-friendly error message based on exception type
    /// </summary>
    private static string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => "Invalid request: required parameter is missing",
            ArgumentException => "Invalid request: invalid parameter value",
            InvalidOperationException => "Invalid operation: operation cannot be performed",
            KeyNotFoundException => "Resource not found",
            UnauthorizedAccessException => "Unauthorized: you do not have permission to access this resource",
            _ => "An unexpected error occurred. Please try again later or contact support."
        };
    }

    /// <summary>
    /// Check if running in production environment
    /// </summary>
    private static bool IsProduction(HttpContext context)
    {
        var environment = context.RequestServices.GetService<IWebHostEnvironment>();
        return environment?.IsProduction() ?? false;
    }
}
