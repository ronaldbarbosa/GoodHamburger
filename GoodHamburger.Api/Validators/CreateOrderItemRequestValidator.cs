using FluentValidation;
using GoodHamburger.Api.Models.Requests;

namespace GoodHamburger.Api.Validators;

public class CreateOrderItemRequestValidator : AbstractValidator<CreateOrderItemRequest>
{
    public CreateOrderItemRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("O ID do pedido deve ser maior que zero.");

        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("O ID do produto deve ser maior que zero.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("A quantidade deve ser maior que zero.");
    }
}
