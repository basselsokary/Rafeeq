using Application.Commands.Sites.Facilities;
using Domain.Entities.SiteAggregate;
using FluentValidation;

namespace Application.Commands.Sites.Validators;

internal class AddSiteFacilityCommandValidator : AbstractValidator<AddSiteFacilityCommand>
{
    public AddSiteFacilityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
    
        RuleFor(x => x.FacilityName)
            .NotEmpty()
            .WithMessage(SiteErrors.NameRequired.Message);
        
        RuleFor(x => x.FacilityDescription)
            .NotEmpty()
            .WithMessage(SiteErrors.DescriptionRequired.Message);
    }
}

internal class RemoveSiteFacilityCommandValidator : AbstractValidator<RemoveSiteFacilityCommand>
{
    public RemoveSiteFacilityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
        
        RuleFor(x => x.FacilityId)
            .NotEmpty()
            .WithMessage(SiteErrors.FacilityIdRequired.Message);
    }
}
