using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using ClinicalPatientManagement.Client;

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

await builder.Build().RunAsync();
