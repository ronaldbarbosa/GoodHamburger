using GoodHamburger.Core.Entities;
using GoodHamburger.Core.ValueObjects;

namespace GoodHamburger.Core.Interfaces.Services;

public interface IDiscountCalculator
{
    (Money discount, string rule) Calculate(Order order);
    Money CalculateSubtotal(Order order);
}