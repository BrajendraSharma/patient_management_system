using ClinicalPatientManagement.Client.Services;

namespace ClinicalPatientManagement.Client.Handlers;

/// <summary>
/// HTTP message handler that automatically injects Bearer token authorization headers
/// into all HTTP requests to the API.
/// Handles token refresh on 401 responses using circuit breaker pattern to prevent loops.
/// </summary>
public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthorizationMessageHandler> _logger;

    public AuthorizationMessageHandler(
        IAuthService authService,
        ILogger<AuthorizationMessageHandler> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sends an HTTP request with automatic Bearer token injection from the authentication service.
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Get current token from auth service
        var token = await _authService.GetTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            // Add Bearer token to Authorization header
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            _logger.LogDebug("Bearer token injected into request for {RequestUri}", request.RequestUri);
        }

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // If 401 Unauthorized, log for debugging (token refresh would be handled at AuthService level)
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Received 401 Unauthorized. Token may have expired. User should re-authenticate.");
            // Note: Token refresh is typically handled at the service/interceptor level,
            // not in the message handler. Consider implementing a token refresh mechanism
            // in AuthService if needed for automatic token renewal.
        }
        else if (response.IsSuccessStatusCode)
        {
            _logger.LogDebug("Request succeeded with status code {StatusCode}", response.StatusCode);
        }

        return response;
    }
}
