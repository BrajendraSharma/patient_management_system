using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ClinicalPatientManagement.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client for API communication
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

// Register API HTTP client
builder.Services.AddHttpClient("ClinicalApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? "https://localhost:5001");
});

await builder.Build().RunAsync();
