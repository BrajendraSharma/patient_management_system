using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using ClinicalPatientManagement.Client;
using ClinicalPatientManagement.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client for API communication
var apiUrl = builder.Configuration["ApiUrl"] ?? "https://localhost:7001";
var apiBaseAddress = apiUrl.EndsWith("/") ? apiUrl : apiUrl + "/";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseAddress)
});

// Register HTTP client factory for named API client
builder.Services.AddHttpClient("ClinicalApi", client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
});

// Step 4.5: Register Authentication Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();

// Step 6: Register Patient API Client
builder.Services.AddScoped<IPatientApiClient, PatientApiClient>();

var host = builder.Build();

// Initialize localStorage helper with JSRuntime
var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
LocalStorageHelper.Initialize(jsRuntime);

await host.RunAsync();
