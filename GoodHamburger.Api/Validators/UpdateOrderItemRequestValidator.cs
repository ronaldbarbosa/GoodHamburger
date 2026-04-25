using FluentValidation;
using GoodHamburger.Api.Models.Requests;

namespace GoodHamburger.Api.Validators;

public class UpdateOrderItemRequestValidator : AbstractValidator<UpdateOrderItemRequest>
{
    public UpdateOrderItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("O ID do produto deve ser maior que zero.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("A quantidade deve ser maior que zero.");
    }
}
