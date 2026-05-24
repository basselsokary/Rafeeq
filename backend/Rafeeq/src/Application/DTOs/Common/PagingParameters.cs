namespace Application.DTOs.Common;

public record PagingParameters(
    int Page = 1,
    int PageSize = 20)
{
    public int Skip => (Page - 1) * PageSize;
    public int Take => PageSize;
}
