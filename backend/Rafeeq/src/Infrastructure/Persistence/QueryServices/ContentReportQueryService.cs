using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.ContentReports;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.QueryServices;

internal class ContentReportQueryService(ApplicationDbContext context)
    : IContentReportQueryService
{
    public async Task<ContentReportDetailDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var dto = await context.ContentReports
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(cr => new ContentReportDetailDto(
                cr.Id,
                cr.ReportedBy,
                cr.ReportedEntityId,
                cr.Reason,
                cr.Description,
                cr.Status,
                cr.ReportedAt,
                cr.ReviewedBy,
                cr.ReviewedAt,
                cr.ReviewNotes,
                cr.ActionTaken
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return dto;
    }

    public async Task<PagedResult<ContentReportListDto>> GetFilteredByPriorityAsync(
        int priority,
        PagingParameters paging,
        ReportReason? reason = null,
        ReportStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.ContentReports.AsNoTracking().AsQueryable();

        if (reason is not null)
            query = query.Where(cr => cr.Reason == reason);
        
        if (status is not null)
            query = query.Where(cr => cr.Status == status);

        var queryPaging = query
            .Select(cr => new ContentReportListDto(
                cr.Id,
                cr.ReportedBy,
                cr.ReportedEntityId,
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
}