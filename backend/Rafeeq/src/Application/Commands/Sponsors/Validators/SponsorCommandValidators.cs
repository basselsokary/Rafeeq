using Domain.Entities.SponsorAggregate;
using FluentValidation;

namespace Application.Commands.Sponsors.Validators;

internal class CreateSponsorCommandValidator : AbstractValidator<CreateSponsorCommand>
{
    public CreateSponsorCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(SponsorErrors.TitleRequired.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(SponsorErrors.DescriptionRequired.Message);

        RuleFor(x => x.Location)
            .NotNull();
            
        RuleFor(x => x.Address)
            .NotNull();

        RuleFor(x => x.Type)
            .IsInEnum();
        
        RuleFor(x => x.Tier)
            .IsInEnum();
    }
}

internal class DeleteSponsorCommandValidator : AbstractValidator<DeleteSponsorCommand>
{
    public DeleteSponsorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
    }
}

internal class UpdateSponsorCommandValidator : AbstractValidator<UpdateSponsorCommand>
{
    public UpdateSponsorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(SponsorErrors.TitleRequired.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(SponsorErrors.DescriptionRequired.Message);

        RuleFor(x => x.Location)
            .NotNull();
            
        RuleFor(x => x.Address)
            .NotNull();

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Tier)
            .IsInEnum();
    }
}

internal class ActivateSponsorCommandValidator : AbstractValidator<ActivateSponsorCommand>
{
    public ActivateSponsorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
    }
}

internal class SetSponsorContactInfoCommandValidator : AbstractValidator<SetSponsorContactInfoCommand>
{
    public SetSponsorContactInfoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
        
        RuleFor(x => x.Phone)
            .NotNull();
        
        RuleFor(x => x.Email)
            .NotNull();

        RuleFor(x => x.WebsiteUrl)
            .NotEmpty()
            .When(x => x.WebsiteUrl != null);
    }
}

internal class ExtendSponsorContractCommandValidator : AbstractValidator<ExtendSponsorContractCommand>
{
    public ExtendSponsorContractCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
    }
}

