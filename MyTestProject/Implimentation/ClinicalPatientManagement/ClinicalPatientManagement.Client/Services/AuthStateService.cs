using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ClinicalPatientManagement.Client.Services;

/// <summary>
/// Centralized service for managing authentication state across the application
/// Ensures consistent auth state visibility and proper UI updates
/// </summary>
public interface IAuthStateService
{
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetUsernameAsync();
    Task<AuthenticationState> GetAuthenticationStateAsync();
    event Action? OnAuthStateChanged;
    void NotifyAuthStateChanged();
}

public class AuthStateService : IAuthStateService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IAuthService _authService;

    public event Action? OnAuthStateChanged;

    public AuthStateService(AuthenticationStateProvider authStateProvider, IAuthService authService)
    {
        _authStateProvider = authStateProvider;
        _authService = authService;
    }

    /// <summary>
    /// Checks if the user is currently authenticated
    /// </summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var authState = await GetAuthenticationStateAsync();
        return authState.User?.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// Gets the current authenticated username
    /// </summary>
    public async Task<string?> GetUsernameAsync()
    {
        var isAuthenticated = await IsAuthenticatedAsync();
        if (!isAuthenticated)
            return null;

        // First try to get from token claims
        var authState = await GetAuthenticationStateAsync();
        var username = authState.User?.FindFirst(c => c.Type == "sub")?.Value;

        // Fallback to localStorage if claim not found
        if (string.IsNullOrEmpty(username))
        {
            username = await _authService.GetUsernameAsync();
        }

        return username;
    }

    /// <summary>
    /// Gets the current authentication state
    /// </summary>
    public async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return await _authStateProvider.GetAuthenticationStateAsync();
    }

    /// <summary>
    /// Notifies all subscribers that the authentication state has changed
    /// </summary>
    public void NotifyAuthStateChanged()
    {
        OnAuthStateChanged?.Invoke();
    }
}
