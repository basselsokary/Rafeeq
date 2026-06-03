using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Application.DTOs.Common;
using Application.DTOs.ContentReports;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

internal class CachedContentReportQueryService(IContentReportQueryService inner, IMemoryCache cache)
    : BaseCache("content-report", cache), IContentReportQueryService
{
    public async Task<ContentReportDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:detail:{id}";
        return await GetOrCreateNullableAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetByIdAsync(id, cancellationToken));
    }

    public async Task<ContentReportAdminDetailDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:admin:{id}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetByIdForAdminAsync(id, cancellationToken));
    }

    public async Task<PagedResult<ContentReportListDto>> GetAsync(
        PagingParameters paging,
        int? priority = null,
        ReportReason? reason = null,
        ReportStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:list:priority={priority?.ToString() ?? "all"}:reason={reason?.ToString() ?? "all"}:status={status?.ToString() ?? "all"}:{FormatPaging(paging)}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetAsync(paging, priority, reason, status, cancellationToken));
    }
}