using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;

namespace ClinicalPatientManagement.Api.Controllers;

/// <summary>
/// Authentication controller for user login
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid login attempt with invalid model state for username: {Username}", loginDto.Username);
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByNameAsync(loginDto.Username);
        if (user == null)
        {
            _logger.LogWarning("Failed login attempt for non-existent user: {Username}", loginDto.Username);
            return Unauthorized("Invalid username or password");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Failed login attempt for user: {Username} - invalid password", loginDto.Username);
            return Unauthorized("Invalid username or password");
        }

        var (token, expiresAt) = GenerateJwtToken(user);
        var expiresIn = (long)(expiresAt - DateTime.UtcNow).TotalSeconds;
        
        _logger.LogInformation("Successful login for user: {Username}", loginDto.Username);
        return Ok(new LoginResponse
        {
            Token = token,
            Username = user.UserName ?? string.Empty,
            ExpiresIn = expiresIn,
            ExpiresAt = expiresAt,
            TokenType = "Bearer"
        });
    }

    /// <summary>
    /// Refresh token endpoint - generates a new token before expiration
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            _logger.LogWarning("Refresh token request with empty token");
            return BadRequest("Token is required");
        }

        try
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            var username = principal.FindFirst(ClaimTypes.Name)?.Value ?? principal.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("Refresh token request with invalid token - no user claim");
                return Unauthorized("Invalid token");
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("Refresh token request for non-existent user: {Username}", username);
                return Unauthorized("Invalid token");
            }

            var (newToken, expiresAt) = GenerateJwtToken(user);
            var expiresIn = (long)(expiresAt - DateTime.UtcNow).TotalSeconds;

            _logger.LogInformation("Token refreshed for user: {Username}", username);
            return Ok(new RefreshTokenResponse
            {
                Token = newToken,
                ExpiresIn = expiresIn,
                ExpiresAt = expiresAt,
                TokenType = "Bearer"
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Refresh token failed: {Exception}", ex.Message);
            return Unauthorized("Invalid token");
        }
    }

    /// <summary>
    /// Logout endpoint - clears token on client side
    /// </summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        _logger.LogInformation("User logout requested");
        return Ok(new { Message = "Logout successful" });
    }

    private (string token, DateTime expiresAt) GenerateJwtToken(ApplicationUser user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
        {
            throw new InvalidOperationException("JWT Key is not properly configured (minimum 32 characters required)");
        }

        var expirationMinutes = double.TryParse(_configuration["Jwt:ExpirationMinutes"], out var minutes) ? minutes : 60;
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new[]
        {
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expiresAt);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException("JWT Key is not configured");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = false // Don't validate lifetime for refresh
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}