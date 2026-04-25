using FluentValidation;
using GoodHamburger.Api.Models.Requests;

namespace GoodHamburger.Api.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("O pedido deve conter ao menos um item.");

        RuleForEach(x => x.Items).SetValidator(new OrderItemCreateOrderRequestValidator());
    }
}

public class OrderItemCreateOrderRequestValidator : AbstractValidator<OrderItemCreateOrderRequest>
{
    public OrderItemCreateOrderRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("O ID do produto deve ser maior que zero.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("A quantidade deve ser maior que zero.");
    }
}
