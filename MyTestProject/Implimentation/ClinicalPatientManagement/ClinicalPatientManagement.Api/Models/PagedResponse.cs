namespace ClinicalPatientManagement.Api.Models;

/// <summary>
/// Generic paginated response wrapper for list operations
/// Phase 2: Architectural Improvements - Pagination Pattern
/// </summary>
/// <typeparam name="T">Type of items in the page</typeparam>
public class PagedResponse<T> where T : class
{
    /// <summary>
    /// Current page number (1-indexed)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;

    /// <summary>
    /// Items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}
