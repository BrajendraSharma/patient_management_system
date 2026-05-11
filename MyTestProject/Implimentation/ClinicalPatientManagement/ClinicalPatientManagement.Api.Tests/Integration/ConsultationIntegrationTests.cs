using ClinicalPatientManagement.Api.Data;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests.Integration;

/// <summary>
/// Integration tests for Consultation API-to-DB operations using Testcontainers.
/// Tests consultation creation, vitals validation, and appointment linking.
/// </summary>
public class ConsultationIntegrationTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public ConsultationIntegrationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateConsultation_ValidData_PersistsToDatabase()
    {
        // Arrange: Create patient and appointment
        var patient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male"
        };

        var appointment = new Appointment
        {
            PatientId = 0, // Will be set after patient saved
            AppointmentDate = DateTime.Now.AddDays(5),
            Status = "Scheduled",
            Notes = "Routine checkup"
        };

        using var context = _fixture.CreateDbContext();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        appointment.PatientId = patient.Id;
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        // Act: Create consultation
        var consultation = new Consultation
        {
            AppointmentId = appointment.Id,
            Temperature = 37.2m, // Valid: 36.5-38.5°C
            BloodPressure = "120/80",
            Pulse = 72,
            Complaints = "Slight headache",
            Diagnosis = "Tension headache"
        };

        context.Consultations.Add(consultation);
        await context.SaveChangesAsync();
        var consultationId = consultation.Id;

        // Assert
        using var verifyContext = _fixture.CreateDbContext();
        var persisted = await verifyContext.Consultations.FindAsync(consultationId);

        Assert.NotNull(persisted);
        Assert.Equal("Tension headache", persisted.Diagnosis);
        Assert.Equal(37.2m, persisted.Temperature);
    }

    [Fact]
    public async Task CreateConsultation_LinkedToAppointment_PersistsRelationship()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(1995, 5, 15),
            Gender = "Female"
        };

        var appointment = new Appointment
        {
            PatientId = 0,
            AppointmentDate = DateTime.Now.AddDays(3),
            Status = "Scheduled",
            Notes = "Follow-up"
        };

        using var context = _fixture.CreateDbContext();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        appointment.PatientId = patient.Id;
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        // Act: Create and persist consultation
        var consultation = new Consultation
        {
            AppointmentId = appointment.Id,
            Temperature = 37.0m,
            BloodPressure = "120/80",
            Pulse = 72,
            Complaints = "Routine visit",
            Diagnosis = "Normal"
        };

        context.Consultations.Add(consultation);
        await context.SaveChangesAsync();

        // Assert: Verify relationship
        using var queryContext = _fixture.CreateDbContext();
        var linked = await queryContext.Consultations
            .Include(c => c.Appointment)
            .FirstOrDefaultAsync(c => c.AppointmentId == appointment.Id);

        Assert.NotNull(linked);
        Assert.NotNull(linked.Appointment);
        Assert.Equal(appointment.Id, linked.Appointment.Id);
    }

    [Fact]
    public async Task ConsultationVitals_ValidateTemperatureRange_ChecksRange()
    {
        // Arrange: Create patient, appointment, and consultations with different temperatures
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
            AppointmentDate = DateTime.Now.AddDays(2),
            Status = "Scheduled",
            Notes = "Consultation"
        };

        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        var consultation = new Consultation
        {
            AppointmentId = appointment.Id,
            Temperature = 37.5m, // Valid: within 36.5-38.5°C
            BloodPressure = "120/80",
            Pulse = 72,
            Complaints = "Checkup",
            Diagnosis = "Good"
        };

        context.Consultations.Add(consultation);
        await context.SaveChangesAsync();
        var consultationId = consultation.Id;

        // Act: Retrieve and validate temperature range
        using var queryContext = _fixture.CreateDbContext();
        var stored = await queryContext.Consultations.FindAsync(consultationId);

        // Assert: Temperature persisted and can be validated
        Assert.NotNull(stored);
        Assert.Equal(37.5m, stored.Temperature);
        Assert.True(stored.Temperature >= 36.5m && stored.Temperature <= 38.5m);
    }

    [Fact]
    public async Task ConsultationVitals_BloodPressureFormat_ValidatesFormat()
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
            AppointmentDate = DateTime.Now.AddDays(4),
            Status = "Scheduled",
            Notes = "Check vitals"
        };

        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        // Act: Create consultation with proper BP format
        var consultation = new Consultation
        {
            AppointmentId = appointment.Id,
            Temperature = 37.0m,
            BloodPressure = "120/80", // Proper format XX/XX
            Pulse = 72,
            Complaints = "Checkup",
            Diagnosis = "Pending"
        };

        context.Consultations.Add(consultation);
        await context.SaveChangesAsync();

        // Assert: Verify BP format persisted correctly
        using var verifyContext = _fixture.CreateDbContext();
        var stored = await verifyContext.Consultations
            .FirstOrDefaultAsync(c => c.AppointmentId == appointment.Id);

        Assert.NotNull(stored);
        Assert.Equal("120/80", stored.BloodPressure);
        Assert.Contains("/", stored.BloodPressure); // Basic format validation
    }

    [Fact]
    public async Task UpdateConsultation_ModifiesDiagnosis_PersistsChanges()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Michael",
            LastName = "Davis",
            Email = "michael@example.com",
            Phone = "6666666666",
            DateOfBirth = new DateTime(1988, 9, 1),
            Gender = "Male"
        };

        using var context = _fixture.CreateDbContext();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var appointment = new Appointment
        {
            PatientId = patient.Id,
            AppointmentDate = DateTime.Now.AddDays(6),
            Status = "Scheduled",
            Notes = "Initial visit"
        };

        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        var consultation = new Consultation
        {
            AppointmentId = appointment.Id,
            Temperature = 37.0m,
            BloodPressure = "120/80",
            Pulse = 70,
            Complaints = "Mild discomfort",
            Diagnosis = "Under evaluation"
        };

        context.Consultations.Add(consultation);
        await context.SaveChangesAsync();
        var consultationId = consultation.Id;

        // Act: Update consultation
        using var updateContext = _fixture.CreateDbContext();
        var toUpdate = await updateContext.Consultations.FindAsync(consultationId);
        Assert.NotNull(toUpdate);
        toUpdate.Diagnosis = "Common cold";
        toUpdate.Temperature = 37.8m;
        await updateContext.SaveChangesAsync();

        // Assert
        using var verifyContext = _fixture.CreateDbContext();
        var updated = await verifyContext.Consultations.FindAsync(consultationId);
        Assert.NotNull(updated);
        Assert.Equal("Common cold", updated.Diagnosis);
        Assert.Equal(37.8m, updated.Temperature);
    }

    [Fact]
    public async Task DeleteConsultation_RemovesFromDatabase_NoLongerRetrievable()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Lisa",
            LastName = "Anderson",
            Email = "lisa@example.com",
            Phone = "8888888888",
            DateOfBirth = new DateTime(1993, 11, 25),
            Gender = "Female"
        };

        using var context = _fixture.CreateDbContext();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var appointment = new Appointment
        {
            PatientId = patient.Id,
            AppointmentDate = DateTime.Now.AddDays(8),
            Status = "Scheduled",
            Notes = "Consultation"
        };

        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        var consultation = new Consultation
        {
            AppointmentId = appointment.Id,
            Temperature = 36.8m,
            BloodPressure = "118/76",
            Pulse = 68,
            Complaints = "Annual checkup",
            Diagnosis = "Healthy"
        };

        context.Consultations.Add(consultation);
        await context.SaveChangesAsync();
        var consultationId = consultation.Id;

        // Act: Delete consultation
        using var deleteContext = _fixture.CreateDbContext();
        var toDelete = await deleteContext.Consultations.FindAsync(consultationId);
        Assert.NotNull(toDelete);
        deleteContext.Consultations.Remove(toDelete);
        await deleteContext.SaveChangesAsync();

        // Assert: Deleted record no longer retrievable
        using var verifyContext = _fixture.CreateDbContext();
        var deleted = await verifyContext.Consultations.FirstOrDefaultAsync(c => c.Id == consultationId);
        Assert.Null(deleted);
    }
}
