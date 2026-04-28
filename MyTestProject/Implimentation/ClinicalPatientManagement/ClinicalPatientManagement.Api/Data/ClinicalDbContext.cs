using Microsoft.EntityFrameworkCore;

namespace ClinicalPatientManagement.Api.Data;

/// <summary>
/// Application DbContext for Clinical Patient Management
/// Placeholder for Step 3: will be populated with entity configurations
/// </summary>
public class ClinicalDbContext : DbContext
{
    public ClinicalDbContext(DbContextOptions<ClinicalDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Entity configurations will be added in Step 3
        // - Patient DbSet and mapping
        // - Appointment DbSet and mapping
        // - Consultation DbSet and mapping
        // - Prescription DbSet and mapping
        // - Medication DbSet and mapping
    }

    // DbSets will be added in Step 3:
    // public DbSet<Patient> Patients { get; set; }
    // public DbSet<Appointment> Appointments { get; set; }
    // public DbSet<Consultation> Consultations { get; set; }
    // public DbSet<Prescription> Prescriptions { get; set; }
    // public DbSet<Medication> Medications { get; set; }
}
