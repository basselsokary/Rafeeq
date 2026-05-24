using Application.Common.Interfaces.Localization;
using FluentValidation;

namespace Application.Queries.Users.Validators;

internal sealed class GetUserStatisticsQueryValidator : AbstractValidator<GetUserStatisticsQuery>
{
    public GetUserStatisticsQueryValidator(IErrorLocalizer errors)
    {
    }
}
