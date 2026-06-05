using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Commands.Artifacts.Images.Validators;

internal sealed class SetMainArtifactImageCommandValidator : AbstractValidator<SetMainArtifactImageCommand>
{
    public SetMainArtifactImageCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.ImageId)
            .NotEmpty()
            .WithMessage(errors[ImageErrors.ImageIdRequired.Code]);
    }
}
