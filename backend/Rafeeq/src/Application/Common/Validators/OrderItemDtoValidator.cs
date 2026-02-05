using Application.DTOs;
using FluentValidation;
using static Domain.Constants.DomainConstants.Item;

namespace Application.Common.Validators;

public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID cannot be empty.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(MaxItemQuantity)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.UnitPrice)
            .SetValidator(new MoneyDtoValidator())
            .WithMessage("Invalid price details.");
    }
    
}
