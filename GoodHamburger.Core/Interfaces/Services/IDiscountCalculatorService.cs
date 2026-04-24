using GoodHamburger.Core.Entities;
using GoodHamburger.Core.ValueObjects;

namespace GoodHamburger.Core.Interfaces.Services;

public interface IDiscountCalculatorService
{
    (Money discount, string rule) Calculate(Order order);
    Money CalculateSubtotal(Order order);
}