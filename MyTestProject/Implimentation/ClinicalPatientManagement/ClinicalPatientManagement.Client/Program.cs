using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using ClinicalPatientManagement.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client for API communication
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

// Register HTTP client factory
builder.Services.AddHttpClient();

// Register API HTTP client
builder.Services.AddHttpClient("ClinicalApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? "https://localhost:5001");
});

await builder.Build().RunAsync();
