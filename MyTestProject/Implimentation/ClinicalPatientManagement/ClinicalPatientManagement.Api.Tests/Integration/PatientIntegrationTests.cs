using ClinicalPatientManagement.Api.Data;
using ClinicalPatientManagement.Api.Models;
using ClinicalPatientManagement.Api.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests.Integration;

/// <summary>
/// Integration tests for database operations using Testcontainers with SQL Server.
/// These tests verify that entity persistence, queries, and transactions work correctly
/// against a real database container, validating API-to-DB flows.
/// </summary>
public class PatientIntegrationTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public PatientIntegrationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreatePatient_ValidData_PersistsToDatabase()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male"
        };

        // Act
        using var context = _fixture.CreateDbContext();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        var patientId = patient.Id;

        // Assert: Verify persistence with new context
        using var verifyContext = _fixture.CreateDbContext();
        var persisted = await verifyContext.Patients.FindAsync(patientId);

        Assert.NotNull(persisted);
        Assert.Equal("John", persisted.FirstName);
        Assert.Equal("1234567890", persisted.Phone);
    }

    [Fact]
    public async Task CreatePatient_DuplicatePhone_CanBeDetected()
    {
        // Arrange: Create first patient
        var patient1 = new Patient
        {
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            Phone = "9876543210",
            DateOfBirth = new DateTime(1995, 5, 15),
            Gender = "Female"
        };

        using var context = _fixture.CreateDbContext();
        context.Patients.Add(patient1);
        await context.SaveChangesAsync();

        // Act: Check if duplicate phone exists (this pattern requires real async queryable - key benefit of Testcontainers)
        var duplicate = await context.Patients
            .FirstOrDefaultAsync(p => p.Phone == "9876543210" && p.Id != patient1.Id);

        // Assert: This is the pattern that Testcontainers enables (real async queryable)
        Assert.NotNull(duplicate);
        Assert.Equal("Alice", duplicate.FirstName);
    }

    [Fact]
    public async Task UpdatePatient_ModifiesExistingRecord_PersistsChanges()
    {
        // Arrange: Create patient
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
        var patientId = patient.Id;

        // Act: Update patient
        using var updateContext = _fixture.CreateDbContext();
        var toUpdate = await updateContext.Patients.FindAsync(patientId);
        Assert.NotNull(toUpdate);
        toUpdate.FirstName = "Robert";
        toUpdate.Email = "robert@example.com";
        await updateContext.SaveChangesAsync();

        // Assert
        using var verifyContext = _fixture.CreateDbContext();
        var updated = await verifyContext.Patients.FindAsync(patientId);
        Assert.NotNull(updated);
        Assert.Equal("Robert", updated.FirstName);
        Assert.Equal("robert@example.com", updated.Email);
    }

    [Fact]
    public async Task DeletePatient_RemovesFromDatabase_NoLongerRetrievable()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Charlie",
            LastName = "Brown",
            Email = "charlie@example.com",
            Phone = "7777777777",
            DateOfBirth = new DateTime(1990, 3, 10),
            Gender = "Male"
        };

        using var context = _fixture.CreateDbContext();
        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        var patientId = patient.Id;

        // Act: Delete patient
        using var deleteContext = _fixture.CreateDbContext();
        var toDelete = await deleteContext.Patients.FindAsync(patientId);
        Assert.NotNull(toDelete);
        deleteContext.Patients.Remove(toDelete);
        await deleteContext.SaveChangesAsync();

        // Assert: Deleted record no longer retrievable
        using var verifyContext = _fixture.CreateDbContext();
        var deleted = await verifyContext.Patients.FirstOrDefaultAsync(p => p.Id == patientId);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task SearchPatients_PartialName_ReturnsMatches()
    {
        // Arrange: Seed multiple patients
        var patients = new List<Patient>
        {
            new() { FirstName = "Robert", LastName = "Young", Email = "robert@test.com", Phone = "1111111111", DateOfBirth = DateTime.Now.AddYears(-30), Gender = "Male" },
            new() { FirstName = "Robin", LastName = "Green", Email = "robin@test.com", Phone = "2222222222", DateOfBirth = DateTime.Now.AddYears(-25), Gender = "Female" },
            new() { FirstName = "David", LastName = "Brown", Email = "david@test.com", Phone = "3333333333", DateOfBirth = DateTime.Now.AddYears(-28), Gender = "Male" }
        };

        using var seedContext = _fixture.CreateDbContext();
        seedContext.Patients.AddRange(patients);
        await seedContext.SaveChangesAsync();

        // Act: Search for names starting with "Ro"
        using var searchContext = _fixture.CreateDbContext();
        var results = await searchContext.Patients
            .Where(p => p.FirstName.Contains("Ro"))
            .ToListAsync();

        // Assert
        Assert.Equal(2, results.Count); // Robert and Robin
    }
}
