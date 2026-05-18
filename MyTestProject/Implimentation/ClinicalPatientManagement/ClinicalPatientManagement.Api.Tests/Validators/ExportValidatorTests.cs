using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Validators;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests.Validators;

/// <summary>
/// Tests for ExportValidator
/// Phase 2: Architectural Improvements - Export Service Validation Tests
/// Verifies export format validation and conversion logic
/// </summary>
public class ExportValidatorTests
{
    [Theory]
    [InlineData("csv")]
    [InlineData("CSV")]
    [InlineData("json")]
    [InlineData("JSON")]
    [InlineData("excel")]
    [InlineData("xlsx")]
    [InlineData("pdf")]
    public void IsValidFormat_WithValidFormats_ReturnsTrue(string format)
    {
        // Act & Assert
        Assert.True(ExportValidator.IsValidFormat(format));
    }

    [Theory]
    [InlineData("xml")]
    [InlineData("doc")]
    [InlineData("txt")]
    [InlineData("invalid")]
    public void IsValidFormat_WithInvalidFormats_ReturnsFalse(string format)
    {
        // Act & Assert
        Assert.False(ExportValidator.IsValidFormat(format));
    }

    [Fact]
    public void IsValidFormat_WithNull_ReturnsFalse()
    {
        // Act & Assert
        Assert.False(ExportValidator.IsValidFormat(null));
    }

    [Fact]
    public void IsValidFormat_WithEmptyString_ReturnsFalse()
    {
        // Act & Assert
        Assert.False(ExportValidator.IsValidFormat(string.Empty));
    }

    [Theory]
    [InlineData("csv", ExportValidator.ExportFormat.Csv)]
    [InlineData("CSV", ExportValidator.ExportFormat.Csv)]
    [InlineData("json", ExportValidator.ExportFormat.Json)]
    [InlineData("excel", ExportValidator.ExportFormat.Excel)]
    [InlineData("xlsx", ExportValidator.ExportFormat.Excel)]
    [InlineData("pdf", ExportValidator.ExportFormat.Pdf)]
    public void ParseFormat_WithValidFormats_ReturnsCorrectEnum(string format, ExportValidator.ExportFormat expected)
    {
        // Act
        var result = ExportValidator.ParseFormat(format);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ParseFormat_WithInvalidFormat_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => ExportValidator.ParseFormat("invalid"));
    }

    [Theory]
    [InlineData(ExportValidator.ExportFormat.Csv, ".csv")]
    [InlineData(ExportValidator.ExportFormat.Json, ".json")]
    [InlineData(ExportValidator.ExportFormat.Excel, ".xlsx")]
    [InlineData(ExportValidator.ExportFormat.Pdf, ".pdf")]
    public void GetFileExtension_ReturnsCorrectExtension(ExportValidator.ExportFormat format, string expected)
    {
        // Act
        var result = ExportValidator.GetFileExtension(format);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(ExportValidator.ExportFormat.Csv, "text/csv")]
    [InlineData(ExportValidator.ExportFormat.Json, "application/json")]
    [InlineData(ExportValidator.ExportFormat.Excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData(ExportValidator.ExportFormat.Pdf, "application/pdf")]
    public void GetMimeType_ReturnsCorrectMimeType(ExportValidator.ExportFormat format, string expected)
    {
        // Act
        var result = ExportValidator.GetMimeType(format);

        // Assert
        Assert.Equal(expected, result);
    }
}
