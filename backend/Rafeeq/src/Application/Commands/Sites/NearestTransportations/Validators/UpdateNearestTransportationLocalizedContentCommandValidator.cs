using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Site;

namespace Application.Commands.Sites.NearestTransportations.Validators;

internal sealed class UpdateNearestTransportationLocalizedContentCommandValidator : AbstractValidator<UpdateNearestTransportationLocalizedContentCommand>
{
    public UpdateNearestTransportationLocalizedContentCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.ContentId)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.LocalizedIdRequired.Code]);

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
            .NotEmpty()
            .WithMessage(errors[AddressErrors.EmptyAddress.Code])
            .MaximumLength(DomainConstants.Address.MaxAddressLength)
            .WithMessage(errors.Format(AddressErrors.ExceededAddressLength.Code, DomainConstants.Address.MaxAddressLength));
    }
}
