using Serilog;
using ClinicalPatientManagement.Api.Extensions;
using ClinicalPatientManagement.Api.Configuration;
using ClinicalPatientManagement.Api.Middleware;
using ClinicalPatientManagement.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ClinicalPatientManagement.Api.Models;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.Extensions.Caching.StackExchangeRedis;

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
    
    // Phase 2.4: Add API Versioning support
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    });
    Log.Information("API versioning configured");
    
    builder.Services.AddSwaggerGen();

    // Phase 1.1: Fix CORS Configuration - Use whitelisted origins from appsettings
    var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" };
    
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowClientsOnly", policy =>
        {
            policy.WithOrigins(corsOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
            
            Log.Information("CORS configured with allowed origins: {Origins}", string.Join(", ", corsOrigins));
        });
    });

    // Add application services and infrastructure
    builder.Services.AddApplicationServices(builder.Configuration);
    builder.Services.AddApiInfrastructure();

    // Phase 3: Add Redis distributed cache and caching service for performance optimization
    var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
    });
    builder.Services.AddSingleton<ICacheService, RedisCacheService>();
    Log.Information("Redis distributed cache and ICacheService registered");

    // Phase 1.3: Add JWT Key Provider for secure key management
    builder.Services.AddScoped<IJwtKeyProvider, JwtKeyProvider>();
    Log.Information("JWT Key Provider registered");
    
    // Phase 2: Register UnitOfWork for transaction management
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    Log.Information("Unit of Work registered for transaction management");

    // Add Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                }));

        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsJsonAsync(
                new { error = "Rate limit exceeded. Maximum 100 requests per minute." },
                token);
            Log.Warning("Rate limit exceeded for {ClientIp}", context.HttpContext.Connection.RemoteIpAddress);
        };
    });

    // Phase 1.3: Add JWT Authentication with secure key provider
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Get JWT key from secure provider
        using var scope = builder.Services.BuildServiceProvider().CreateScope();
        var keyProvider = scope.ServiceProvider.GetRequiredService<IJwtKeyProvider>();
        var signingKey = keyProvider.GetSigningKey();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = signingKey
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

    
    // Phase 2.3: Register global exception handling middleware (before other middleware)
    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    Log.Information("Global exception handling middleware registered");
    
    // ✅ Redirect root URL to Swagger
    app.MapGet("/", () => Results.Redirect("/swagger"));

    app.UseHttpsRedirection();
    app.UseRateLimiter();
    app.UseCors("AllowClientsOnly");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Phase 1.2: Seed default user only in development with environment-specific credentials
    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await SeedDefaultUser(userManager, builder.Environment);
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

/// <summary>
/// Phase 1.2: Seeds default user only in development environment.
/// In production, users must be created through proper admin interfaces.
/// </summary>
static async Task SeedDefaultUser(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
{
    // Only seed in development environment
    if (!environment.IsDevelopment())
    {
        Log.Information("Skipping default user seeding in {Environment} environment", environment.EnvironmentName);
        return;
    }

    const string defaultUsername = "doctor";
    
    var user = await userManager.FindByNameAsync(defaultUsername);
    if (user == null)
    {
        // In development, read from environment or configuration
        // This prevents hardcoded credentials in source code
        var defaultPassword = Environment.GetEnvironmentVariable("DEFAULT_USER_PASSWORD") ?? "DevPassword123!";
        var defaultEmail = Environment.GetEnvironmentVariable("DEFAULT_USER_EMAIL") ?? "doctor@clinic.local";

        user = new ApplicationUser { UserName = defaultUsername, Email = defaultEmail };
        var result = await userManager.CreateAsync(user, defaultPassword);
        if (!result.Succeeded)
        {
            Log.Error("Failed to create default user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        else
        {
            Log.Information("Default development user created successfully");
        }
    }
    else
    {
        Log.Information("Default user already exists, skipping creation");
    }
}
