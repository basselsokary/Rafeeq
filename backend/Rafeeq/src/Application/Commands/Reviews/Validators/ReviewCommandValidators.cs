using Domain.Entities.ReviewAggregate;
using Domain.Enums;
using FluentValidation;

namespace Application.Commands.Reviews.Validators;

internal class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(ReviewErrors.SiteIdRequired.Message);
        
        RuleFor(x => x.Rating)
            .NotNull();
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ReviewErrors.TitleRequired.Message);
        
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage(ReviewErrors.ContentRequired.Message);
    }
}

internal class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ReviewErrors.IdRequired.Message);
    }
}

internal class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ReviewErrors.IdRequired.Message);
        
        RuleFor(x => x.Rating)
            .NotNull();
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ReviewErrors.TitleRequired.Message);
        
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage(ReviewErrors.ContentRequired.Message);
    }
}

internal class SetReviewStatusCommandValidator : AbstractValidator<SetReviewStatusCommand>
{
    public SetReviewStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ReviewErrors.IdRequired.Message);
        
        RuleFor(x => x.Status)
            .IsInEnum();
        
        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .When(x => x.Status == ReviewStatus.Rejected)
            .WithMessage(ReviewErrors.RejectionReasonRequired.Message);
    }
}

internal class HelpfulReviewCommandValidator : AbstractValidator<HelpfulReviewCommand>
{
    public HelpfulReviewCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ReviewErrors.IdRequired.Message);
    }
}