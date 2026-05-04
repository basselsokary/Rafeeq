using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Entities.SponsorAggregate;
using Domain.ValueObjects;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Sponsor;

namespace Application.Commands.Sponsors.LocalizedContents.Validators;

internal sealed class AddSponsorLocalizedContentCommandValidator : AbstractValidator<AddSponsorLocalizedContentsCommand>
{
    public AddSponsorLocalizedContentCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
        
        RuleFor(x => x.LocalizedContents)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);

        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new AddSponsorLocalizedContentsDtoCommandValidator(errors));
    }
}

internal sealed class AddSponsorLocalizedContentsDtoCommandValidator : AbstractValidator<AddSponsorLocalizedContentsDtoCommand>
{
    public AddSponsorLocalizedContentsDtoCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Language.ToString()));

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.TitleRequired.Code])
            .MaximumLength(MaxTitleLength)
            .WithMessage(errors.Format(SponsorErrors.ExceededTitleLength.Code, MaxTitleLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.DescriptionRequired.Code])
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(errors.Format(SponsorErrors.ExceededDescriptionLength.Code, MaxDescriptionLength));

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage(errors[AddressErrors.EmptyAddress.Code])
            .MaximumLength(DomainConstants.Address.MaxAddressLength)
            .WithMessage(errors.Format(AddressErrors.ExceededAddressLength.Code, DomainConstants.Address.MaxAddressLength));
    }
}
