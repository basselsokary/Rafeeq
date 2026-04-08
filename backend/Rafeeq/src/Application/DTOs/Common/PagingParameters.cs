namespace Application.DTOs.Common;

public record PagingParameters(
    int PageNumber = 1,
    int PageSize = 20)
{
    public int Skip => (PageNumber - 1) * PageSize;
    public int Take => PageSize;
}
