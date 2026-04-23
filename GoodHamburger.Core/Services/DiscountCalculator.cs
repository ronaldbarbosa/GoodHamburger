using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Services;
using GoodHamburger.Core.ValueObjects;

namespace GoodHamburger.Core.Services;

public class DiscountCalculator :  IDiscountCalculator
{
    private const decimal FullDiscount = 0.20m;      // Sandwich + Side + Beverage
    private const decimal SandwichBeverageDiscount = 0.15m;  // Sandwich + Beverage
    private const decimal SandwichSideDiscount = 0.10m;   // Sandwich + Side

    public (Money discount, string rule) Calculate(Order order)
    {
        var categories = order.Items
            .Where(i => i.Product?.Category != null)
            .Select(i => i.Product!.Category!.Name.ToLowerInvariant())
            .ToHashSet();

        bool hasSandwich = categories.Contains("sanduíches");
        bool hasSide = categories.Contains("acompanhamentos");
        bool hasBeverage = categories.Contains("bebidas");

        decimal discountRate = 0m;
        string rule = "Sem desconto";

        if (hasSandwich && hasSide && hasBeverage)
        {
            discountRate = FullDiscount;
            rule = "Sanduíche + Batata + Refrigerante (20%)";
        }
        else if (hasSandwich && hasBeverage)
        {
            discountRate = SandwichBeverageDiscount;
            rule = "Sanduíche + Refrigerante (15%)";
        }
        else if (hasSandwich && hasSide)
        {
            discountRate = SandwichSideDiscount;
            rule = "Sanduíche + Batata (10%)";
        }

        var discount = new Money(order.Subtotal.Amount * discountRate);
        return (discount, rule);
    }

    public Money CalculateSubtotal(Order order)
    {
        var total = order.Items
            .Where(i => i.Product != null)
            .Sum(i => i.Product!.Price.Amount * i.Quantity);

        return new Money(total);
    }
}