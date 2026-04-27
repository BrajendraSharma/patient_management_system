using Xunit;
using ClinicalPatientManagement.Api.Models;

namespace ClinicalPatientManagement.Api.Tests;

/// <summary>
/// Unit tests for BaseEntity model
/// </summary>
public class BaseEntityTests
{
    [Fact]
    public void BaseEntity_HasDefaultCreatedAtAsUtcNow()
    {
        // Arrange & Act
        var entity = new TestEntity();
        var now = DateTime.UtcNow;

        // Assert
        Assert.True(entity.CreatedAt <= now);
        Assert.Equal(DateTimeKind.Utc, entity.CreatedAt.Kind);
    }

    [Fact]
    public void BaseEntity_UpdatedAtIsNullByDefault()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        Assert.Null(entity.UpdatedAt);
    }

    [Fact]
    public void BaseEntity_CanSetId()
    {
        // Arrange
        var entity = new TestEntity();

        // Act
        entity.Id = 123;

        // Assert
        Assert.Equal(123, entity.Id);
    }

    [Fact]
    public void BaseEntity_CanSetUpdatedAt()
    {
        // Arrange
        var entity = new TestEntity();
        var updateTime = DateTime.UtcNow;

        // Act
        entity.UpdatedAt = updateTime;

        // Assert
        Assert.Equal(updateTime, entity.UpdatedAt);
    }

    /// <summary>
    /// Concrete implementation of BaseEntity for testing
    /// </summary>
    private class TestEntity : BaseEntity
    {
    }
}
