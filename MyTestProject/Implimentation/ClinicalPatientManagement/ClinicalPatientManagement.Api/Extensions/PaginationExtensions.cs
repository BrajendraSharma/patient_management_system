using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicalPatientManagement.Api.Extensions;

/// <summary>
/// Extension methods for IQueryable pagination
/// Phase 2: Architectural Improvements - Pagination Pattern
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Apply pagination to a queryable sequence
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="query">IQueryable source</param>
    /// <param name="request">Pagination request parameters</param>
    /// <returns>PagedResponse with paginated items</returns>
    public static async Task<PagedResponse<T>> ToPaginatedAsync<T>(
        this IQueryable<T> query,
        PaginationRequest request,
        CancellationToken cancellationToken = default) where T : class
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));
        
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip(request.SkipCount)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<T>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    /// <summary>
    /// Create a PagedResponse without async database call (for in-memory collections)
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="items">Collection to paginate</param>
    /// <param name="request">Pagination parameters</param>
    /// <returns>PagedResponse with paginated items</returns>
    public static PagedResponse<T> ToPaginated<T>(
        this IEnumerable<T> items,
        PaginationRequest request) where T : class
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));
        
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var itemList = items.ToList();
        var totalCount = itemList.Count;

        var paginatedItems = itemList
            .Skip(request.SkipCount)
            .Take(request.PageSize)
            .ToList();

        return new PagedResponse<T>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Items = paginatedItems
        };
    }
}
