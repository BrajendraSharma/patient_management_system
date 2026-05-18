using ClinicalPatientManagement.Api.Data;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests.Integration;

/// <summary>
/// Integration tests for Appointment API-to-DB operations using Testcontainers.
/// Tests appointment scheduling, conflict detection, and appointment status management.
/// </summary>
public class AppointmentIntegrationTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public AppointmentIntegrationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAppointment_ValidData_PersistsToDatabase()
    {
        // Arrange: Create a patient first
        var patient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male"
        };

        using var context = _fixture.CreateDbContext();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var appointment = new Appointment
        {
            PatientId = patient.Id,
            AppointmentDate = DateTime.Now.AddDays(7),
            Status = "Scheduled",
            Notes = "Regular checkup"
        };

        // Act
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();
        var appointmentId = appointment.Id;

        // Assert: Verify persistence with separate context
        using var verifyContext = _fixture.CreateDbContext();
        var persisted = await verifyContext.Appointments.FindAsync(appointmentId);

        Assert.NotNull(persisted);
        Assert.Equal("Scheduled", persisted.Status);
        Assert.Equal("Regular checkup", persisted.Notes);
    }

    [Fact]
    public async Task CreateAppointment_ConflictDetection_WithinThirtyMinutes()
    {
        // Arrange: Create patient and first appointment
        var patient = new Patient
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(1995, 5, 15),
            Gender = "Female"
        };

        using var context = _fixture.CreateDbContext();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var appointmentDate = DateTime.Now.AddDays(5).Date.AddHours(10);

        var firstAppointment = new Appointment
        {
            PatientId = patient.Id,
            AppointmentDate = appointmentDate,
            Status = "Scheduled",
            Notes = "First appointment"
        };

        context.Appointments.Add(firstAppointment);
        await context.SaveChangesAsync();

        // Act: Check for conflicting appointments (within 30 minutes)
        var conflictingTime = appointmentDate.AddMinutes(20); // 20 minutes after first
        var conflicts = await context.Appointments
            .Where(a => a.PatientId == patient.Id && 
                   Math.Abs((a.AppointmentDate - conflictingTime).TotalMinutes) < 30)
            .ToListAsync();

        // Assert: Conflict should be detected
        Assert.NotEmpty(conflicts);
        Assert.Single(conflicts);
    }

    [Fact]
    public async Task UpdateAppointment_ChangesStatus_PersistsChanges()
    {
        // Arrange: Create appointment
        var patient = new Patient
        {
            FirstName = "Bob",
            LastName = "Wilson",
            Email = "bob@example.com",
            Phone = "5555555555",
            DateOfBirth = new DateTime(1985, 7, 20),
            Gender = "Male"
        };

        using var context = _fixture.CreateDbContext();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var appointment = new Appointment
        {
            PatientId = patient.Id,
            AppointmentDate = DateTime.Now.AddDays(10),
            Status = "Scheduled",
            Notes = "Initial appointment"
        };

        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();
        var appointmentId = appointment.Id;

        // Act: Update status
        using var updateContext = _fixture.CreateDbContext();
        var toUpdate = await updateContext.Appointments.FindAsync(appointmentId);
        Assert.NotNull(toUpdate);
        toUpdate.Status = "Completed";
        toUpdate.Notes = "Patient examined, results normal";
        await updateContext.SaveChangesAsync();

        // Assert
        using var verifyContext = _fixture.CreateDbContext();
        var updated = await verifyContext.Appointments.FindAsync(appointmentId);
        Assert.NotNull(updated);
        Assert.Equal("Completed", updated.Status);
        Assert.Contains("results normal", updated.Notes);
    }

    [Fact]
    public async Task GetAppointment_WithPatientData_LoadsRelatedData()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Sarah",
            LastName = "Johnson",
            Email = "sarah@example.com",
            Phone = "4444444444",
            DateOfBirth = new DateTime(1992, 3, 14),
            Gender = "Female"
        };

        using var context = _fixture.CreateDbContext();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var appointment = new Appointment
        {
            PatientId = patient.Id,
            AppointmentDate = DateTime.Now.AddDays(15),
            Status = "Scheduled",
            Notes = "Routine checkup"
        };

        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();
        var appointmentId = appointment.Id;

        // Act: Retrieve appointment with related patient data
        using var queryContext = _fixture.CreateDbContext();
        var retrieved = await queryContext.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(patient.Id, retrieved.PatientId);
        Assert.NotNull(retrieved.Patient);
        Assert.Equal("Sarah", retrieved.Patient.FirstName);
    }

    [Fact]
    public async Task DeleteAppointment_RemovesFromDatabase_NoLongerRetrievable()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Michael",
            LastName = "Brown",
            Email = "michael@example.com",
            Phone = "7777777777",
            DateOfBirth = new DateTime(1988, 9, 1),
            Gender = "Male"
        };

        using var context = _fixture.CreateDbContext();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var appointment = new Appointment
        {
            PatientId = patient.Id,
            AppointmentDate = DateTime.Now.AddDays(20),
            Status = "Scheduled",
            Notes = "Scheduled visit"
        };

        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();
        var appointmentId = appointment.Id;

        // Act: Delete appointment
        using var deleteContext = _fixture.CreateDbContext();
        var toDelete = await deleteContext.Appointments.FindAsync(appointmentId);
        Assert.NotNull(toDelete);
        deleteContext.Appointments.Remove(toDelete);
        await deleteContext.SaveChangesAsync();

        // Assert: Deleted record no longer retrievable
        using var verifyContext = _fixture.CreateDbContext();
        var deleted = await verifyContext.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
        Assert.Null(deleted);
    }
}
