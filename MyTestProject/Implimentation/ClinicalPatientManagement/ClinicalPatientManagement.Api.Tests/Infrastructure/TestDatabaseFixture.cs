using ClinicalPatientManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests.Infrastructure;

/// <summary>
/// Fixture for managing SQL Server container and test database lifecycle.
/// Implements IAsyncLifetime to handle async initialization and cleanup.
/// </summary>
public class TestDatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container;
    private string _connectionString = null!;

    public TestDatabaseFixture()
    {
        // Create SQL Server container with custom password
        _container = new MsSqlBuilder()
            .WithPassword("MyTest@123!")
            .Build();
    }

    public async Task InitializeAsync()
    {
        // Start the container
        await _container.StartAsync();

        // Get the connection string from the container
        _connectionString = _container.GetConnectionString();

        // Create and apply migrations
        using var context = CreateDbContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        // Stop and clean up the container
        await _container.StopAsync();
    }

    /// <summary>
    /// Creates a fresh DbContext connected to the test database.
    /// Each test should get its own context instance for isolation.
    /// </summary>
    public ClinicalDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ClinicalDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        return new ClinicalDbContext(options);
    }

    /// <summary>
    /// Seeds test data into the database.
    /// </summary>
    public async Task SeedTestDataAsync(Func<ClinicalDbContext, Task> seedAction)
    {
        using var context = CreateDbContext();
        await seedAction(context);
        await context.SaveChangesAsync();
    }
}
