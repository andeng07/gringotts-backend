namespace Gringotts.Api.Shared.Pagination;

public record PaginatedResult<T>(int Page, int PageSize, long TotalRecords, IEnumerable<T> Items) where T : class;