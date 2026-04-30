using FluentValidation;
using Domain.Entities.ContentReportAggregate;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.ContentReports.Validators;

internal sealed class GetContentReportByIdForAdminQueryValidator : AbstractValidator<GetContentReportByIdForAdminQuery>
{
    public GetContentReportByIdForAdminQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ContentReportErrors.IdRequired.Code]);
    }
}
