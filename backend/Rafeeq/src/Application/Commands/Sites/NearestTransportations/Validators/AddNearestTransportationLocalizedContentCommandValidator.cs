using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Site;

namespace Application.Commands.Sites.NearestTransportations.Validators;

internal sealed class AddNearestTransportationLocalizedContentCommandValidator : AbstractValidator<AddNearestTransportationLocalizedContentCommand>
{
    public AddNearestTransportationLocalizedContentCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage(errors[ValidationErrors.InvalidEnumValue.Code]);

        RuleFor(x => x.TransportationId)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.NameRequired.Code])
            .MaximumLength(MaxNearestTransportationStationLength)
            .WithMessage(errors.Format(ValidationErrors.MaximumLengthExceeded.Code, MaxNearestTransportationStationLength));

        RuleFor(x => x.Description)
            .MaximumLength(MaxDescriptionLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage(errors.Format(ValidationErrors.MaximumLengthExceeded.Code, MaxDescriptionLength));

        RuleFor(x => x.Address)
            .MaximumLength(DomainConstants.Address.MaxAddressLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage(errors.Format(AddressErrors.ExceededAddressLength.Code, DomainConstants.Address.MaxAddressLength));
    }
}
