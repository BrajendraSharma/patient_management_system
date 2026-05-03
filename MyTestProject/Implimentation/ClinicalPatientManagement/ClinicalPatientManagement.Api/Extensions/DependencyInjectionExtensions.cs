using ClinicalPatientManagement.Api.Data;
using ClinicalPatientManagement.Api.Mappings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClinicalPatientManagement.Api.Models;

namespace ClinicalPatientManagement.Api.Extensions;

/// <summary>
/// Dependency Injection extension methods for setting up services
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Register application services and repositories
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ClinicalDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Add Identity
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ClinicalDbContext>()
            .AddDefaultTokenProviders();

        // Add AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Repository and Service registrations will be added here in Steps 3+
        // Example pattern:
        // services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        // services.AddScoped<IPatientService, PatientService>();
        // services.AddScoped<IAppointmentService, AppointmentService>();

        return services;
    }

    /// <summary>
    /// Register API infrastructure services
    /// </summary>
    public static IServiceCollection AddApiInfrastructure(this IServiceCollection services)
    {
        // Add any infrastructure services here
        // Example: services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
