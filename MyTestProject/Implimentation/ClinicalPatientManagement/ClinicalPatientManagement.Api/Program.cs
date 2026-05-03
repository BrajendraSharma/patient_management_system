using Serilog;
using ClinicalPatientManagement.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ClinicalPatientManagement.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/clinical-api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Clinical Patient Management API");

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Configure CORS for Blazor client
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowBlazor", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Add application services and infrastructure
    builder.Services.AddApplicationServices(builder.Configuration);
    builder.Services.AddApiInfrastructure();

    // Add JWT Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        
        app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                options.RoutePrefix = "swagger"; // swagger URL
            });

    }

    
    // ✅ Redirect root URL to Swagger
    app.MapGet("/", () => Results.Redirect("/swagger"));


    app.UseHttpsRedirection();
    app.UseCors("AllowBlazor");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Seed default user
    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await SeedDefaultUser(userManager);
    }

    Log.Information("Clinical Patient Management API configured successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static async Task SeedDefaultUser(UserManager<ApplicationUser> userManager)
{
    const string defaultUsername = "doctor";
    const string defaultPassword = "Password123!";

    var user = await userManager.FindByNameAsync(defaultUsername);
    if (user == null)
    {
        user = new ApplicationUser { UserName = defaultUsername, Email = "doctor@clinic.com" };
        var result = await userManager.CreateAsync(user, defaultPassword);
        if (!result.Succeeded)
        {
            Log.Error("Failed to create default user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        else
        {
            Log.Information("Default user created successfully");
        }
    }
}
