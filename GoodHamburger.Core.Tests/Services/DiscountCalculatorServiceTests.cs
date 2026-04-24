using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Services;
using GoodHamburger.Core.ValueObjects;

namespace GoodHamburger.Core.Tests.Services;

public class DiscountCalculatorServiceTests
{
    private class TestableDiscountCalculatorService : DiscountCalculatorService { }

    private readonly TestableDiscountCalculatorService _calculatorService = new();

    private static Product CreateProduct(int id, string name, string categoryName, decimal price, int categoryId = 0)
    {
        return new Product
        {
            Id = id,
            Name = name,
            Price = new Money(price),
            Category = new ProductCategory { Id = categoryId, Name = categoryName }
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
            CreateProduct(1, "X-Burger", "Sanduíches", 15.00m, 1),
            CreateProduct(2, "Batata Frita", "Acompanhamentos", 8.00m, 2),
            CreateProduct(3, "Refrigerante", "Bebidas", 5.00m, 3)
        };
        var order = CreateOrder(products);

        var subtotal = _calculatorService.CalculateSubtotal(order);

        Assert.Equal(28.00m, subtotal.Amount);
    }

    [Fact]
    public void Calculate_FullDiscount_SandwichSideBeverage()
    {
        var products = new[]
        {
            CreateProduct(1, "X-Burger", "Sanduíches", 15.00m, 1),
            CreateProduct(2, "Batata Frita", "Acompanhamentos", 8.00m, 2),
            CreateProduct(3, "Refrigerante", "Bebidas", 5.00m, 3)
        };
        var order = CreateOrder(products);
        order.Subtotal = _calculatorService.CalculateSubtotal(order);

        var (discount, rule) = _calculatorService.Calculate(order);

        Assert.Equal(28.00m * 0.20m, discount.Amount);
        Assert.Equal("Sanduíche + Batata + Refrigerante (20%)", rule);
    }

    [Fact]
    public void Calculate_SandwichBeverageDiscount()
    {
        var products = new[]
        {
            CreateProduct(1, "X-Burger", "Sanduíches", 15.00m, 1),
            CreateProduct(2, "Refrigerante", "Bebidas", 5.00m, 3)
        };
        var order = CreateOrder(products);
        order.Subtotal = _calculatorService.CalculateSubtotal(order);

        var (discount, rule) = _calculatorService.Calculate(order);

        Assert.Equal(20.00m * 0.15m, discount.Amount);
        Assert.Equal("Sanduíche + Refrigerante (15%)", rule);
    }

    [Fact]
    public void Calculate_SandwichSideDiscount()
    {
        var products = new[]
        {
            CreateProduct(1, "X-Burger", "Sanduíches", 15.00m, 1),
            CreateProduct(2, "Batata Frita", "Acompanhamentos", 8.00m, 2)
        };
        var order = CreateOrder(products);
        order.Subtotal = _calculatorService.CalculateSubtotal(order);

        var (discount, rule) = _calculatorService.Calculate(order);

        Assert.Equal(23.00m * 0.10m, discount.Amount);
        Assert.Equal("Sanduíche + Batata (10%)", rule);
    }

    [Fact]
    public void Calculate_NoDiscount_SingleItem()
    {
        var products = new[]
        {
            CreateProduct(1, "X-Burger", "Sanduíches", 15.00m, 1)
        };
        var order = CreateOrder(products);
        order.Subtotal = _calculatorService.CalculateSubtotal(order);

        var (discount, rule) = _calculatorService.Calculate(order);

        Assert.Equal(0m, discount.Amount);
        Assert.Equal("Sem desconto", rule);
    }

    [Fact]
    public void Calculate_NoDiscount_NoValidCombination()
    {
        var products = new[]
        {
            CreateProduct(1, "Batata Frita", "Acompanhamentos", 8.00m, 2),
            CreateProduct(2, "Refrigerante", "Bebidas", 5.00m, 3)
        };
        var order = CreateOrder(products);
        order.Subtotal = _calculatorService.CalculateSubtotal(order);

        var (discount, rule) = _calculatorService.Calculate(order);

        Assert.Equal(0m, discount.Amount);
        Assert.Equal("Sem desconto", rule);
    }
}