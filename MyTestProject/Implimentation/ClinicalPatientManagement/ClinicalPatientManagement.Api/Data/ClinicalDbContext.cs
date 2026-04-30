using Microsoft.EntityFrameworkCore;
using ClinicalPatientManagement.Api.Models;

namespace ClinicalPatientManagement.Api.Data;

/// <summary>
/// Application DbContext for Clinical Patient Management
/// </summary>
public class ClinicalDbContext : DbContext
{
    public ClinicalDbContext(DbContextOptions<ClinicalDbContext> options) 
        : base(options)
    {
    }

    // DbSets
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Consultation> Consultations { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Medication> Medications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Patient configuration
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Phone).IsRequired().HasMaxLength(20);
            entity.Property(p => p.Email).HasMaxLength(255);
            entity.Property(p => p.Gender).IsRequired().HasMaxLength(10);
            
            // Indexes for search performance
            entity.HasIndex(p => p.FirstName);
            entity.HasIndex(p => p.LastName);
            entity.HasIndex(p => p.Phone).IsUnique();
            entity.HasIndex(p => new { p.FirstName, p.LastName });
        });

        // Appointment configuration
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Status).IsRequired().HasMaxLength(20);
            entity.Property(a => a.Notes).HasMaxLength(500);
            
            // Foreign key
            entity.HasOne(a => a.Patient)
                  .WithMany(p => p.Appointments)
                  .HasForeignKey(a => a.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            entity.HasIndex(a => a.PatientId);
            entity.HasIndex(a => a.AppointmentDate);
            entity.HasIndex(a => a.Status);
        });

        // Consultation configuration
        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.BloodPressure).IsRequired().HasMaxLength(20);
            entity.Property(c => c.Complaints).IsRequired().HasMaxLength(1000);
            entity.Property(c => c.Diagnosis).IsRequired().HasMaxLength(1000);
            
            // Foreign key
            entity.HasOne(c => c.Appointment)
                  .WithOne(a => a.Consultation)
                  .HasForeignKey<Consultation>(c => c.AppointmentId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            entity.HasIndex(c => c.AppointmentId).IsUnique();
        });

        // Prescription configuration
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            // Foreign key
            entity.HasOne(p => p.Consultation)
                  .WithOne(c => c.Prescription)
                  .HasForeignKey<Prescription>(p => p.ConsultationId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            entity.HasIndex(p => p.ConsultationId).IsUnique();
            entity.HasIndex(p => p.PrescriptionDate);
        });

        // Medication configuration
        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Name).IsRequired().HasMaxLength(255);
            entity.Property(m => m.Dosage).IsRequired().HasMaxLength(100);
            entity.Property(m => m.Frequency).IsRequired().HasMaxLength(100);
            entity.Property(m => m.Instructions).HasMaxLength(500);
            
            // Foreign key
            entity.HasOne(m => m.Prescription)
                  .WithMany(p => p.Medications)
                  .HasForeignKey(m => m.PrescriptionId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            entity.HasIndex(m => m.PrescriptionId);
            entity.HasIndex(m => m.Name);
        });
    }
}
