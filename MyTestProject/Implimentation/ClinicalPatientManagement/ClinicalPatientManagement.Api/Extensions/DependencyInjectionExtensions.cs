using ClinicalPatientManagement.Api.Data;
using ClinicalPatientManagement.Api.Mappings;
using ClinicalPatientManagement.Api.Repositories;
using ClinicalPatientManagement.Api.Services;
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

        // Step 6: Register Patient Repository and Service
        services.AddScoped<PatientRepository>();
        services.AddScoped<IPatientRepository>(sp => sp.GetRequiredService<PatientRepository>());
        services.AddScoped<IPatientService, PatientService>();

        // Step 7: Register Appointment Repository and Service
        services.AddScoped<AppointmentRepository>();
        services.AddScoped<IAppointmentRepository>(sp => sp.GetRequiredService<AppointmentRepository>());
        services.AddScoped<IAppointmentService, AppointmentService>();

        // Step 9: Register Consultation Repository and Service
        services.AddScoped<ConsultationRepository>();
        services.AddScoped<IConsultationRepository>(sp => sp.GetRequiredService<ConsultationRepository>());
        services.AddScoped<IConsultationService, ConsultationService>();

        // Step 10: Register Prescription Repository and Service
        services.AddScoped<PrescriptionRepository>();
        services.AddScoped<IPrescriptionRepository>(sp => sp.GetRequiredService<PrescriptionRepository>());
        services.AddScoped<IPrescriptionService, PrescriptionService>();

        // Additional repositories and services will be added in future steps

        return services;
    }

    /// <summary>
    /// Register API infrastructure services
    /// </summary>
    public static IServiceCollection AddApiInfrastructure(this IServiceCollection services)
    {
        // Step 11: Register Unit of Work for transaction management - ACID compliance
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
