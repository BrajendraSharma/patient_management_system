using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;

namespace ClinicalPatientManagement.Client.Services;

/// <summary>
/// Service for authentication operations including token management
/// </summary>
public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(string username, string password);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task<string?> GetUsernameAsync();
    Task SetUsernameAsync(string username);
    Task ClearAuthenticationAsync();
    bool IsAuthenticated { get; }
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private const string TokenStorageKey = "authToken";
    private const string UsernameStorageKey = "username";

    public bool IsAuthenticated => !string.IsNullOrEmpty(GetTokenAsync().Result);

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        var loginRequest = new { username, password };
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result?.Token != null)
            {
                await SetTokenAsync(result.Token);
                await SetUsernameAsync(result.Username ?? username);
            }
            return result;
        }

        return null;
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _httpClient.PostAsync("/api/auth/logout", null);
        }
        finally
        {
            await ClearAuthenticationAsync();
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        return await LocalStorageHelper.GetItemAsync(TokenStorageKey);
    }

    public async Task SetTokenAsync(string token)
    {
        await LocalStorageHelper.SetItemAsync(TokenStorageKey, token);
    }

    public async Task<string?> GetUsernameAsync()
    {
        return await LocalStorageHelper.GetItemAsync(UsernameStorageKey);
    }

    public async Task SetUsernameAsync(string username)
    {
        await LocalStorageHelper.SetItemAsync(UsernameStorageKey, username);
    }

    public async Task ClearAuthenticationAsync()
    {
        await LocalStorageHelper.RemoveItemAsync(TokenStorageKey);
        await LocalStorageHelper.RemoveItemAsync(UsernameStorageKey);
    }
}

public class LoginResponse
{
    public string? Token { get; set; }
    public string? Username { get; set; }
}

/// <summary>
/// Helper class for localStorage operations using JavaScript interop
/// </summary>
public static class LocalStorageHelper
{
    private static IJSRuntime? _jsRuntime;

    public static void Initialize(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public static async Task<string?> GetItemAsync(string key)
    {
        if (_jsRuntime == null) return null;

        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
        }
        catch
        {
            return null;
        }
    }

    public static async Task SetItemAsync(string key, string value)
    {
        if (_jsRuntime == null) return;

        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }
        catch { }
    }

    public static async Task RemoveItemAsync(string key)
    {
        if (_jsRuntime == null) return;

        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch { }
    }

    public static async Task ClearAsync()
    {
        if (_jsRuntime == null) return;

        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.clear");
        }
        catch { }
    }
}
