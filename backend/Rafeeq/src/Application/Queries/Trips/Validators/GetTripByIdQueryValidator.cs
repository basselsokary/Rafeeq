using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Queries.Trips.Validators;

internal sealed class GetTripByIdQueryValidator : AbstractValidator<GetTripByIdQuery>
{
    public GetTripByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);
    }
}
