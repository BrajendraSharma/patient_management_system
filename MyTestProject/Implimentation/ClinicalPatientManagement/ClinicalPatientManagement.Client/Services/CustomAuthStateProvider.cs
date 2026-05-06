using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace ClinicalPatientManagement.Client.Services;

/// <summary>
/// Custom authentication state provider that manages user authentication state
/// based on JWT tokens stored in localStorage
/// </summary>
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private readonly HttpClient _httpClient;

    public CustomAuthStateProvider(IAuthService authService, HttpClient httpClient)
    {
        _authService = authService;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _authService.GetTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var principal = new ClaimsPrincipal(identity);

            // Add token to default request headers for API calls
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return new AuthenticationState(principal);
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task LoginAsync(string username, string password)
    {
        var response = await _authService.LoginAsync(username, password);
        if (response?.Token != null)
        {
            var token = response.Token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

    public async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        _httpClient.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static List<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();

        try
        {
            // JWT format: header.payload.signature
            var parts = jwt.Split('.');
            if (parts.Length != 3)
                return claims;

            // Decode payload (add padding if necessary)
            var payloadJson = parts[1];
            var padding = 4 - (payloadJson.Length % 4);
            if (padding != 4)
                payloadJson += new string('=', padding);

            var decodedBytes = Convert.FromBase64String(payloadJson);
            var payload = Encoding.UTF8.GetString(decodedBytes);

            // Parse JSON claims
            using (JsonDocument doc = JsonDocument.Parse(payload))
            {
                var root = doc.RootElement;

                foreach (var property in root.EnumerateObject())
                {
                    var claimType = property.Name;
                    var value = property.Value;

                    if (value.ValueKind == JsonValueKind.String)
                    {
                        claims.Add(new Claim(claimType, value.GetString() ?? ""));
                    }
                    else if (value.ValueKind == JsonValueKind.Number)
                    {
                        claims.Add(new Claim(claimType, value.GetRawText()));
                    }
                    else if (value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in value.EnumerateArray())
                        {
                            if (item.ValueKind == JsonValueKind.String)
                            {
                                claims.Add(new Claim(claimType, item.GetString() ?? ""));
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            // If parsing fails, return empty claims list
        }

        return claims;
    }
}
