using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.ContentReports;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.QueryServices;

internal class ContentReportQueryService(
    ApplicationDbContext context) : IContentReportQueryService
{
    public async Task<ContentReportDetailDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.ContentReports
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(cr => new ContentReportDetailDto(
                cr.Id,
                cr.ReportedBy,
                cr.ContentId,
                cr.Reason,
                cr.Description,
                cr.Status,
                cr.ReportedAt,
                cr.ReviewedAt,
                cr.ReviewNotes,
                cr.ActionTaken
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<ContentReportListDto>> GetAsync(
        PagingParameters paging,
        int? priority,
        ReportReason? reason = null,
        ReportStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.ContentReports.AsNoTracking().AsQueryable();

        if (priority.HasValue)
            query = query.Where(cr => cr.Priority == priority);
        
        if (reason is not null)
            query = query.Where(cr => cr.Reason == reason);
        
        if (status is not null)
            query = query.Where(cr => cr.Status == status);

        var queryPaging = query
            .Select(cr => new ContentReportListDto(
                cr.Id,
                cr.ReportedBy,
                cr.ContentId,
                cr.Reason,
                cr.Description,
                cr.Status,
                cr.ReportedAt
            ));

        var totalCount = await queryPaging.CountAsync(cancellationToken);
        var items = await queryPaging
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<ContentReportListDto>(
            items,
            totalCount,
            paging.PageNumber,
            paging.PageSize);
    }

    public async Task<ContentReportAdminDetailDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ContentReports
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(cr => new ContentReportAdminDetailDto(
                cr.Id,
                cr.ReportedBy,
                cr.ContentId,
                cr.Reason,
                cr.Description,
                cr.Status,
                cr.Priority,
                cr.ReportedAt,
                cr.ReviewedBy,
                cr.ReviewedAt,
                cr.ReviewNotes,
                cr.ActionTaken,
                cr.CreatedAt,
                cr.LastModifiedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}