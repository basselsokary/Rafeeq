namespace Application.DTOs.Common;

/// <summary>
/// Paged result wrapper
/// </summary>
public record PagedResult<T>(
    List<T> Data,
    int TotalCount,
    int PageNumber,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}
