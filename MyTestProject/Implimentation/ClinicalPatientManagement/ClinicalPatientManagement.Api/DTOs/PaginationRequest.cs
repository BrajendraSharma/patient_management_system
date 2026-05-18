using System.ComponentModel.DataAnnotations;

namespace ClinicalPatientManagement.Api.DTOs;

/// <summary>
/// Query parameters for paginated requests
/// Phase 2: Architectural Improvements - Pagination Pattern
/// </summary>
public class PaginationRequest
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    private const int MaxPageSize = 100;
    private const int MinPageNumber = 1;

    /// <summary>
    /// Page number (1-indexed, default 1)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = Math.Max(MinPageNumber, value);
    }

    /// <summary>
    /// Number of records per page (default 10, max 100)
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }

    /// <summary>
    /// Number of records to skip
    /// </summary>
    public int SkipCount => (PageNumber - 1) * PageSize;
}
