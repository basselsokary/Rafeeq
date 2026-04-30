using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.CityAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.City;

namespace Application.Commands.Cities.LocalizedContents.Validators;

internal sealed class AddCityLocalizedContentsCommandValidator : AbstractValidator<AddCityLocalizedContentsCommand>
{
    public AddCityLocalizedContentsCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[CityErrors.IdRequired.Code]);
        
        RuleFor(x => x.LocalizedContents)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);

        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new AddCityLocalizedContentsDtoCommandValidator(errors));
    }
}

internal sealed class AddCityLocalizedContentsDtoCommandValidator : AbstractValidator<AddCityLocalizedContentsDtoCommand>
{
    public AddCityLocalizedContentsDtoCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Language.ToString()));

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(errors[CityErrors.NameRequired.Code])
            .MaximumLength(MaxNameLength)
            .WithMessage(errors.Format(CityErrors.ExceededNameLength.Code, MaxNameLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(errors[CityErrors.DescriptionRequired.Code])
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(errors.Format(CityErrors.ExceededDescriptionLength.Code, MaxDescriptionLength));
    }
}
