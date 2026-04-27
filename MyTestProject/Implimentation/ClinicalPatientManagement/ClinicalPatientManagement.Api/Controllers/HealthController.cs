using Microsoft.AspNetCore.Mvc;

namespace ClinicalPatientManagement.Api.Controllers;

/// <summary>
/// Health check endpoint for verifying API is running
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get API health status
    /// </summary>
    [HttpGet]
    [ProduceResponseType(StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> Get()
    {
        _logger.LogInformation("Health check endpoint called");

        var response = new HealthResponse
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        };

        return Ok(response);
    }
}

/// <summary>
/// Health check response model
/// </summary>
public class HealthResponse
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Version { get; set; } = string.Empty;
}
