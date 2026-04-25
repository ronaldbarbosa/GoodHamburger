using FluentValidation;
using GoodHamburger.Api.Models.Requests;
using GoodHamburger.Core.Entities;

namespace GoodHamburger.Api.Validators;

public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("O status é obrigatório.")
            .Must(s => Enum.TryParse<OrderStatus>(s, ignoreCase: true, out _))
            .WithMessage("Status inválido. Valores aceitos: Confirmed, Preparing, Ready, Completed.");
    }
}
