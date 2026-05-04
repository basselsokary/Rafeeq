using Domain.Entities.ContentReportAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal sealed class ContentReportRepository(ApplicationDbContext context)
    : BaseRepository<ContentReport>(context), IContentReportRepository
{
}
