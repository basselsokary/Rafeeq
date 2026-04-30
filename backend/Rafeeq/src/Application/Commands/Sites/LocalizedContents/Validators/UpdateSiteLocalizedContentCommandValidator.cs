using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Site;

namespace Application.Commands.Sites.LocalizedContents.Validators;

internal sealed class UpdateSiteLocalizedContentCommandValidator : AbstractValidator<UpdateSiteLocalizedContentCommand>
{
    public UpdateSiteLocalizedContentCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);

        RuleFor(x => x.LocalizedContents)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);
        
        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new UpdateSiteLocalizedContentsDtoValidator(errors));
    }
}

internal class UpdateSiteLocalizedContentsDtoValidator : AbstractValidator<UpdateSiteLocalizedContentsDto>
{
    public UpdateSiteLocalizedContentsDtoValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage(errors[ValidationErrors.InvalidEnumValue.Code]);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.NameRequired.Code])
            .MaximumLength(MaxNameLength)
            .WithMessage(errors.Format(SiteErrors.ExceededNameLength.Code, MaxNameLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.DescriptionRequired.Code])
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(errors.Format(SiteErrors.ExceededDescriptionLength.Code, MaxDescriptionLength));
        
        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage(errors[AddressErrors.EmptyAddress.Code])
            .MaximumLength(DomainConstants.Address.MaxAddressLength)
            .WithMessage(errors.Format(AddressErrors.ExceededAddressLength.Code, DomainConstants.Address.MaxAddressLength));
    }
}