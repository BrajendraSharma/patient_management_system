using Xunit;
using Moq;
using ClinicalPatientManagement.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClinicalPatientManagement.Api.Tests;

/// <summary>
/// Unit tests for HealthController - verifies API startup and basic functionality
/// </summary>
public class HealthControllerTests
{
    [Fact]
    public void Get_ReturnsOkResult_WhenCalled()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HealthController>>();
        var controller = new HealthController(loggerMock.Object);

        // Act
        var result = controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void Get_ReturnsHealthyStatus()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HealthController>>();
        var controller = new HealthController(loggerMock.Object);

        // Act
        var result = controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<HealthResponse>(okResult.Value);
        Assert.Equal("Healthy", response.Status);
    }

    [Fact]
    public void Get_ReturnsValidVersion()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HealthController>>();
        var controller = new HealthController(loggerMock.Object);

        // Act
        var result = controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<HealthResponse>(okResult.Value);
        Assert.NotEmpty(response.Version);
    }

    [Fact]
    public void Get_ReturnsCurrentTimestamp()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HealthController>>();
        var controller = new HealthController(loggerMock.Object);
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = controller.Get();
        var afterCall = DateTime.UtcNow;

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<HealthResponse>(okResult.Value);
        Assert.True(response.Timestamp >= beforeCall && response.Timestamp <= afterCall);
    }
}
