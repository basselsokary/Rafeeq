using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.ContentReports;
using Domain.Entities.ContentReportAggregate;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.QueryServices;

internal sealed class ContentReportQueryService(
    ApplicationDbContext context) : IContentReportQueryService
{
    private IQueryable<ContentReport> ContentReports => context.ContentReports.AsNoTracking();
    
    public async Task<ContentReportDetailDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await ContentReports
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
        int? priority = null,
        ReportReason? reason = null,
        ReportStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = ContentReports;

        if (priority.HasValue)
            query = query.Where(cr => cr.Priority == priority);
        
        if (reason is not null)
            query = query.Where(cr => cr.Reason == reason);
        
        if (status is not null)
            query = query.Where(cr => cr.Status == status);

        query = query.OrderByDescending(cr => cr.Priority);

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
        return await ContentReports
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