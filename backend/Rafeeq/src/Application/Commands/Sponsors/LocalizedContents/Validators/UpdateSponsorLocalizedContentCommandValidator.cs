using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.SponsorAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Sponsor;

namespace Application.Commands.Sponsors.LocalizedContents.Validators;

internal sealed class UpdateSponsorLocalizedContentCommandValidator : AbstractValidator<UpdateSponsorLocalizedContentCommand>
{
    public UpdateSponsorLocalizedContentCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);

        RuleFor(x => x.LocalizedContents)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);
        
        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new UpdateSponsorLocalizedContentsDtoValidator(errors));
    }
}

internal class UpdateSponsorLocalizedContentsDtoValidator : AbstractValidator<UpdateSponsorLocalizedContentsDto>
{
    public UpdateSponsorLocalizedContentsDtoValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage(errors[ValidationErrors.InvalidEnumValue.Code]);

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
    }
}