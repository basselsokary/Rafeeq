using Application.DTOs;
using FluentValidation;
using static Domain.Constants.DomainConstants.Money;

namespace Application.Common.Validators;

public class MoneyDtoValidator : AbstractValidator<MoneyDto>
{
    public MoneyDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency cannot be empty.")
            .MaximumLength(MaxCurrencyLength)
            .WithMessage($"Currency cannot exceed {MaxCurrencyLength} characters.");
    }
}