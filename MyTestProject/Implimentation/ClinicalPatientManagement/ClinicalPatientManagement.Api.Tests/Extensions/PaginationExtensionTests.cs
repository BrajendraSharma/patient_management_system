using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Extensions;
using ClinicalPatientManagement.Api.Models;
using Xunit;

namespace ClinicalPatientManagement.Api.Tests.Extensions;

/// <summary>
/// Tests for PaginationExtensions
/// Phase 2: Architectural Improvements - Pagination Tests
/// Verifies pagination logic for both async and sync operations
/// </summary>
public class PaginationExtensionTests
{
    [Fact]
    public void ToPaginated_WithValidRequest_ReturnsPaginatedResponse()
    {
        // Arrange
        var items = Enumerable.Range(1, 25).Select(i => $"item{i}").ToList();
        var request = new PaginationRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = items.ToPaginated(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(10, result.Items.Count());
        Assert.True(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public void ToPaginated_WithLastPage_ReturnsCorrectPageInfo()
    {
        // Arrange
        var items = Enumerable.Range(1, 25).Select(i => $"item{i}").ToList();
        var request = new PaginationRequest { PageNumber = 3, PageSize = 10 };

        // Act
        var result = items.ToPaginated(request);

        // Assert
        Assert.Equal(3, result.PageNumber);
        Assert.Equal(5, result.Items.Count());
        Assert.False(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
    }

    [Fact]
    public void ToPaginated_WithMiddlePage_ReturnsCorrectPageInfo()
    {
        // Arrange
        var items = Enumerable.Range(1, 25).Select(i => $"item{i}").ToList();
        var request = new PaginationRequest { PageNumber = 2, PageSize = 10 };

        // Act
        var result = items.ToPaginated(request);

        // Assert
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.Items.Count());
        Assert.True(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
    }

    [Fact]
    public void ToPaginated_WithPageSizeExceedingTotal_ReturnsAllItems()
    {
        // Arrange
        var items = Enumerable.Range(1, 5).Select(i => $"item{i}").ToList();
        var request = new PaginationRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = items.ToPaginated(request);

        // Assert
        Assert.Equal(5, result.Items.Count());
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void ToPaginated_WithEmptyCollection_ReturnsEmptyResponse()
    {
        // Arrange
        var items = new List<string>();
        var request = new PaginationRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = items.ToPaginated(request);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public void ToPaginated_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var items = Enumerable.Range(1, 10).Select(i => $"item{i}").ToList();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => items.ToPaginated(null!));
    }

    [Fact]
    public void PaginationRequest_WithPageSizeExceedingMax_ClampedToMax()
    {
        // Arrange & Act
        var request = new PaginationRequest { PageSize = 500 };

        // Assert
        Assert.Equal(100, request.PageSize); // Max is 100
    }

    [Fact]
    public void PaginationRequest_WithPageNumberBelowMin_ClampedToMin()
    {
        // Arrange & Act
        var request = new PaginationRequest { PageNumber = -5 };

        // Assert
        Assert.Equal(1, request.PageNumber); // Min is 1
    }

    [Fact]
    public void PagedResponse_CalculatesTotalPages_Correctly()
    {
        // Arrange
        var response = new PagedResponse<string>
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 25,
            Items = new List<string>()
        };

        // Act & Assert
        Assert.Equal(3, response.TotalPages); // (25 + 10 - 1) / 10 = 3
    }

    [Fact]
    public void PagedResponse_CalculatesTotalPages_WithExactDivision()
    {
        // Arrange
        var response = new PagedResponse<string>
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 30,
            Items = new List<string>()
        };

        // Act & Assert
        Assert.Equal(3, response.TotalPages); // 30 / 10 = 3
    }
}
