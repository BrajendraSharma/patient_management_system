namespace ClinicalPatientManagement.Api.DTOs;

/// <summary>
/// DTO for token refresh request
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// The expired JWT token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The refresh token (for future enhancement)
    /// </summary>
    public string? RefreshToken { get; set; }
}

/// <summary>
/// DTO for token refresh response
/// </summary>
public class RefreshTokenResponse
{
    /// <summary>
    /// New JWT token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time (Unix timestamp)
    /// </summary>
    public long ExpiresIn { get; set; }

    /// <summary>
    /// Token expiration date/time
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Token type (Bearer)
    /// </summary>
    public string TokenType { get; set; } = "Bearer";
}

/// <summary>
/// Extended login response with token expiration details
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in seconds
    /// </summary>
    public long ExpiresIn { get; set; }

    /// <summary>
    /// Token expiration date/time
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Token type (Bearer)
    /// </summary>
    public string TokenType { get; set; } = "Bearer";
}
