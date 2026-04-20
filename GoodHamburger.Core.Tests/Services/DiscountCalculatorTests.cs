using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Services;
using GoodHamburger.Core.ValueObjects;

namespace GoodHamburger.Core.Tests.Services;

public class DiscountCalculatorTests
{
    private class TestableDiscountCalculator : DiscountCalculator { }

    private readonly TestableDiscountCalculator _calculator = new();

    private static Product CreateProduct(int id, string name, string categoryName, decimal price)
    {
        return new Product
        {
            Id = id,
            Name = name,
            Price = new Money(price),
            Category = new ProductCategory { Id = 1, Name = categoryName }
        };
    }

    private static Order CreateOrder(params Product[] products)
    {
        return new Order
        {
            Id = 1,
            Items = products.Select((p, i) => new OrderItem { Id = i + 1, ProductId = p.Id, Product = p }).ToList()
        };
    }

    [Fact]
    public void CalculateSubtotal_SumItemsCorrectly()
    {
        var products = new[]
        {
            CreateProduct(1, "X-Burger", "Sandwich", 15.00m),
            CreateProduct(2, "Batata", "Side", 8.00m),
            CreateProduct(3, "Refrigerante", "Beverage", 5.00m)
        };
        var order = CreateOrder(products);

        var subtotal = _calculator.CalculateSubtotal(order);

        Assert.Equal(28.00m, subtotal.Amount);
    }

    [Fact]
    public void Calculate_FullDiscount_SandwichSideBeverage()
    {
        var products = new[]
        {
            CreateProduct(1, "X-Burger", "Sandwich", 15.00m),
            CreateProduct(2, "Batata", "Side", 8.00m),
            CreateProduct(3, "Refrigerante", "Beverage", 5.00m)
        };
        var order = CreateOrder(products);
        order.Subtotal = _calculator.CalculateSubtotal(order);

        var (discount, rule) = _calculator.Calculate(order);

        Assert.Equal(28.00m * 0.20m, discount.Amount);
        Assert.Equal("Sanduíche + Batata + Refrigerante (20%)", rule);
    }

    [Fact]
    public void Calculate_SandwichBeverageDiscount()
    {
        var products = new[]
        {
            CreateProduct(1, "X-Burger", "Sandwich", 15.00m),
            CreateProduct(2, "Refrigerante", "Beverage", 5.00m)
        };
        var order = CreateOrder(products);
        order.Subtotal = _calculator.CalculateSubtotal(order);

        var (discount, rule) = _calculator.Calculate(order);

        Assert.Equal(20.00m * 0.15m, discount.Amount);
        Assert.Equal("Sanduíche + Refrigerante (15%)", rule);
    }

    [Fact]
    public void Calculate_SandwichSideDiscount()
    {
        var products = new[]
        {
            CreateProduct(1, "X-Burger", "Sandwich", 15.00m),
            CreateProduct(2, "Batata", "Side", 8.00m)
        };
        var order = CreateOrder(products);
        order.Subtotal = _calculator.CalculateSubtotal(order);

        var (discount, rule) = _calculator.Calculate(order);

        Assert.Equal(23.00m * 0.10m, discount.Amount);
        Assert.Equal("Sanduíche + Batata (10%)", rule);
    }

    [Fact]
    public void Calculate_NoDiscount_SingleItem()
    {
        var products = new[]
        {
            CreateProduct(1, "X-Burger", "Sandwich", 15.00m)
        };
        var order = CreateOrder(products);
        order.Subtotal = _calculator.CalculateSubtotal(order);

        var (discount, rule) = _calculator.Calculate(order);

        Assert.Equal(0m, discount.Amount);
        Assert.Equal("Sem desconto", rule);
    }

    [Fact]
    public void Calculate_NoDiscount_NoValidCombination()
    {
        var products = new[]
        {
            CreateProduct(1, "Batata", "Side", 8.00m),
            CreateProduct(2, "Refrigerante", "Beverage", 5.00m)
        };
        var order = CreateOrder(products);
        order.Subtotal = _calculator.CalculateSubtotal(order);

        var (discount, rule) = _calculator.Calculate(order);

        Assert.Equal(0m, discount.Amount);
        Assert.Equal("Sem desconto", rule);
    }
}