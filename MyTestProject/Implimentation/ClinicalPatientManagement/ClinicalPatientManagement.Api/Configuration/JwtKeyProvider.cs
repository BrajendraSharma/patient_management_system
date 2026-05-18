using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ClinicalPatientManagement.Api.Configuration;

/// <summary>
/// Provides secure JWT key management with support for Azure Key Vault in production
/// and user secrets/configuration in development.
/// </summary>
public interface IJwtKeyProvider
{
    SymmetricSecurityKey GetSigningKey();
    Task<SymmetricSecurityKey> GetSigningKeyAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of JWT key provider that retrieves keys from configuration or Key Vault.
/// </summary>
public class JwtKeyProvider : IJwtKeyProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtKeyProvider> _logger;
    private SymmetricSecurityKey? _cachedKey;

    public JwtKeyProvider(IConfiguration configuration, ILogger<JwtKeyProvider> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the JWT signing key from configuration.
    /// For production, this should be replaced with Azure Key Vault retrieval.
    /// </summary>
    public SymmetricSecurityKey GetSigningKey()
    {
        if (_cachedKey != null)
        {
            return _cachedKey;
        }

        var keyString = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(keyString))
        {
            throw new InvalidOperationException("JWT key not configured. Ensure Jwt:Key is set in appsettings or user secrets.");
        }

        if (keyString.Length < 32)
        {
            _logger.LogWarning("JWT key is less than 32 characters. This may not meet security requirements for HS256.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        _cachedKey = key;
        _logger.LogInformation("JWT signing key loaded successfully");
        return key;
    }

    /// <summary>
    /// Async version for future Key Vault integration.
    /// Currently delegates to synchronous version but allows for async Key Vault calls.
    /// </summary>
    public Task<SymmetricSecurityKey> GetSigningKeyAsync(CancellationToken cancellationToken = default)
    {
        // For now, just return the cached/configured key
        // In production, this would fetch from Azure Key Vault:
        // var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        // var secret = await client.GetSecretAsync("jwtSigningKey", cancellationToken: cancellationToken);
        // return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.Value.Value));

        return Task.FromResult(GetSigningKey());
    }
}
