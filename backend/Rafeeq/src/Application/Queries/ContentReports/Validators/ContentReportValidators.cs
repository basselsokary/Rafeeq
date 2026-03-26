using FluentValidation;
using Application.Queries.Common.Validators;
using Domain.Common.Constants;
using Domain.Entities.ContentReportAggregate;

namespace Application.Queries.ContentReports.Validators;

internal class GetContentReportByIdQueryValidator : AbstractValidator<GetContentReportByIdQuery>
{
    public GetContentReportByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ContentReportErrors.IdRequired.Message);
    }
}

internal class GetContentReportByIdForAdminQueryValidator : AbstractValidator<GetContentReportByIdForAdminQuery>
{
    public GetContentReportByIdForAdminQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ContentReportErrors.IdRequired.Message);
    }
}

internal class GetContentReportsByHighPriorityQueryValidator : AbstractValidator<GetContentReportsByHighPriorityQuery>
{
    public GetContentReportsByHighPriorityQueryValidator()
    {
        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(DomainConstants.ContentReport.HighPriority);

        RuleFor(x => x.Paging)
            .NotNull()
            .SetValidator(new PagingParametersValidator());

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);

        RuleFor(x => x.Reason)
            .IsInEnum()
            .When(x => x.Reason.HasValue);
    }
}