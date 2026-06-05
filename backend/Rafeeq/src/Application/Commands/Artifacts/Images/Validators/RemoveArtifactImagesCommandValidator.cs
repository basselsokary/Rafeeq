using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Commands.Artifacts.Images.Validators;

internal sealed class RemoveArtifactImagesCommandValidator : AbstractValidator<RemoveArtifactImagesCommand>
{
    public RemoveArtifactImagesCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);

        RuleForEach(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(errors[ImageErrors.ImageIdRequired.Code]);
    }
}
