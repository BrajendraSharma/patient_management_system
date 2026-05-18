using ClinicalPatientManagement.Api.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests.Security;

/// <summary>
/// Tests for Phase 1 security hardening implementation.
/// Validates JWT key management, CORS configuration, and token handling.
/// </summary>
public class Phase1SecurityTests
{
    [Fact]
    public void JwtKeyProvider_WithValidKey_ReturnsSymmetricSecurityKey()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "this-is-a-valid-security-key-minimum-32-characters-long-!!!" }
            })
            .Build();

        var loggerMock = new Mock<ILogger<JwtKeyProvider>>();
        var provider = new JwtKeyProvider(config, loggerMock.Object);

        // Act
        var key = provider.GetSigningKey();

        // Assert
        Assert.NotNull(key);
        Assert.IsType<SymmetricSecurityKey>(key);
    }

    [Fact]
    public void JwtKeyProvider_WithMissingKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var loggerMock = new Mock<ILogger<JwtKeyProvider>>();
        var provider = new JwtKeyProvider(config, loggerMock.Object);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => provider.GetSigningKey());
        Assert.Contains("JWT key not configured", exception.Message);
    }

    [Fact]
    public void JwtKeyProvider_WithShortKey_LogsWarning()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "short-key-123" }
            })
            .Build();

        var loggerMock = new Mock<ILogger<JwtKeyProvider>>();
        var provider = new JwtKeyProvider(config, loggerMock.Object);

        // Act
        var key = provider.GetSigningKey();

        // Assert
        Assert.NotNull(key);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("less than 32 characters")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void JwtKeyProvider_CachesKey_AfterFirstRetrieval()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "this-is-a-valid-security-key-minimum-32-characters-long-!!!" }
            })
            .Build();

        var loggerMock = new Mock<ILogger<JwtKeyProvider>>();
        var provider = new JwtKeyProvider(config, loggerMock.Object);

        // Act
        var key1 = provider.GetSigningKey();
        var key2 = provider.GetSigningKey();

        // Assert
        Assert.Same(key1, key2); // Should be same cached instance
    }

    [Fact]
    public async Task JwtKeyProvider_GetSigningKeyAsync_ReturnsKey()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "this-is-a-valid-security-key-minimum-32-characters-long-!!!" }
            })
            .Build();

        var loggerMock = new Mock<ILogger<JwtKeyProvider>>();
        var provider = new JwtKeyProvider(config, loggerMock.Object);

        // Act
        var key = await provider.GetSigningKeyAsync();

        // Assert
        Assert.NotNull(key);
        Assert.IsType<SymmetricSecurityKey>(key);
    }

    [Fact]
    public void JwtKeyProvider_GeneratesCorrectKeyLength()
    {
        // Arrange
        const string testKey = "this-is-a-valid-security-key-minimum-32-characters-long-!!!";
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", testKey }
            })
            .Build();

        var loggerMock = new Mock<ILogger<JwtKeyProvider>>();
        var provider = new JwtKeyProvider(config, loggerMock.Object);

        // Act
        var key = provider.GetSigningKey();
        var expectedKeyBytes = Encoding.UTF8.GetBytes(testKey);

        // Assert
        Assert.NotNull(key);
        // SymmetricSecurityKey uses the byte representation
        Assert.Equal(expectedKeyBytes.Length, key.KeySize / 8); // KeySize is in bits, convert to bytes
    }

    [Fact]
    public void JwtKeyProvider_WithNullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        IConfiguration nullConfig = null!;
        var loggerMock = new Mock<ILogger<JwtKeyProvider>>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new JwtKeyProvider(nullConfig, loggerMock.Object));
        Assert.Equal("configuration", exception.ParamName);
    }

    [Fact]
    public void JwtKeyProvider_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();
        ILogger<JwtKeyProvider> nullLogger = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new JwtKeyProvider(config, nullLogger));
        Assert.Equal("logger", exception.ParamName);
    }
}
