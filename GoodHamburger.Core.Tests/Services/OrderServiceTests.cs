using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Core.Services;
using GoodHamburger.Core.ValueObjects;
using Moq;

namespace GoodHamburger.Core.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _orderService = new OrderService(
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object,
            new TestableDiscountCalculatorService(),
            _unitOfWorkMock.Object);
    }

    private class TestableDiscountCalculatorService : DiscountCalculatorService { }

    private static ProductCategory CreateCategory(int id, string name, bool isActive = true) =>
        new() { Id = id, Name = name, IsActive = isActive };

    private static Product CreateProduct(int id, string name, decimal price, int categoryId, string categoryName, bool isActive = true) =>
        new() { Id = id, Name = name, Price = new Money(price), CategoryId = categoryId, Category = CreateCategory(categoryId, categoryName, isActive), IsActive = isActive };

    private void SetupAddAsync() =>
        _orderRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns<Order, CancellationToken>((o, _) => Task.FromResult(o));

    // ----- CreateAsync -----

    [Fact]
    public async Task CreateAsync_AllThreeCategories_Applies20PercentDiscount()
    {
        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");
        var side = CreateProduct(2, "Batata Frita", 8.00m, 2, "Acompanhamentos");
        var beverage = CreateProduct(3, "Refrigerante", 5.00m, 3, "Bebidas");

        var order = new Order
        {
            Items =
            [
                new() { Id = 1, ProductId = 1 },
                new() { Id = 2, ProductId = 2 },
                new() { Id = 3, ProductId = 3 }
            ]
        };

        _productRepositoryMock.SetupSequence(p => p.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(sandwich)
            .ReturnsAsync(side)
            .ReturnsAsync(beverage);
        SetupAddAsync();

        var result = await _orderService.CreateAsync(order);

        Assert.Equal(28.00m, result.Subtotal.Amount);
        Assert.Equal(28.00m * 0.20m, result.Discount.Amount);
        Assert.Equal(28.00m * 0.80m, result.Total.Amount);
    }

    [Fact]
    public async Task CreateAsync_SandwichAndBeverage_Applies15PercentDiscount()
    {
        var sandwich = CreateProduct(1, "X-Burger", 20.00m, 1, "Sanduíches");
        var beverage = CreateProduct(2, "Refrigerante", 5.00m, 3, "Bebidas");

        var order = new Order
        {
            Items =
            [
                new() { Id = 1, ProductId = 1 },
                new() { Id = 2, ProductId = 2 }
            ]
        };

        _productRepositoryMock.SetupSequence(p => p.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(sandwich)
            .ReturnsAsync(beverage);
        SetupAddAsync();

        var result = await _orderService.CreateAsync(order);

        Assert.Equal(25.00m, result.Subtotal.Amount);
        Assert.Equal(25.00m * 0.15m, result.Discount.Amount);
        Assert.Equal(25.00m * 0.85m, result.Total.Amount);
    }

    [Fact]
    public async Task CreateAsync_SandwichAndSide_Applies10PercentDiscount()
    {
        var sandwich = CreateProduct(1, "X-Burger", 20.00m, 1, "Sanduíches");
        var side = CreateProduct(2, "Batata Frita", 8.00m, 2, "Acompanhamentos");

        var order = new Order
        {
            Items =
            [
                new() { Id = 1, ProductId = 1 },
                new() { Id = 2, ProductId = 2 }
            ]
        };

        _productRepositoryMock.SetupSequence(p => p.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(sandwich)
            .ReturnsAsync(side);
        SetupAddAsync();

        var result = await _orderService.CreateAsync(order);

        Assert.Equal(28.00m, result.Subtotal.Amount);
        Assert.Equal(28.00m * 0.10m, result.Discount.Amount);
        Assert.Equal(28.00m * 0.90m, result.Total.Amount);
    }

    [Fact]
    public async Task CreateAsync_OnlySandwich_AppliesNoDiscount()
    {
        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");

        var order = new Order
        {
            Items = [new() { Id = 1, ProductId = 1 }]
        };

        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(sandwich);
        SetupAddAsync();

        var result = await _orderService.CreateAsync(order);

        Assert.Equal(15.00m, result.Subtotal.Amount);
        Assert.Equal(0m, result.Discount.Amount);
        Assert.Equal(15.00m, result.Total.Amount);
    }

    [Fact]
    public async Task CreateAsync_ValidOrder_SetsUnitPriceFromProduct()
    {
        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");
        var item = new OrderItem { Id = 1, ProductId = 1 };
        var order = new Order { Items = [item] };

        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(sandwich);
        SetupAddAsync();

        await _orderService.CreateAsync(order);

        Assert.Equal(15.00m, item.UnitPrice.Amount);
    }

    [Fact]
    public async Task CreateAsync_ValidOrder_CallsSaveChanges()
    {
        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");
        var order = new Order { Items = [new() { Id = 1, ProductId = 1 }] };

        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(sandwich);
        SetupAddAsync();

        await _orderService.CreateAsync(order);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ProductNotFound_ThrowsEntityNotFoundException()
    {
        var order = new Order
        {
            Items = [new() { Id = 1, ProductId = 1 }]
        };

        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _orderService.CreateAsync(order));
    }

    [Fact]
    public async Task CreateAsync_ProductInactive_ThrowsBusinessRuleViolationException()
    {
        var product = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches", isActive: false);

        var order = new Order
        {
            Items = [new() { Id = 1, ProductId = 1 }]
        };

        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(product);

        await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _orderService.CreateAsync(order));
    }

    [Fact]
    public async Task CreateAsync_QuantityGreaterThanOne_ThrowsInvalidItemQuantityException()
    {
        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");

        var order = new Order
        {
            Items = [new() { Id = 1, ProductId = 1, Quantity = 2 }]
        };

        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(sandwich);

        await Assert.ThrowsAsync<InvalidItemQuantityException>(() => _orderService.CreateAsync(order));
    }

    [Fact]
    public async Task CreateAsync_DuplicateCategories_ThrowsDuplicateItemException()
    {
        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");
        var sandwich2 = CreateProduct(2, "X-Salada", 12.00m, 1, "Sanduíches");

        var order = new Order
        {
            Items =
            [
                new() { Id = 1, ProductId = 1 },
                new() { Id = 2, ProductId = 2 }
            ]
        };

        _productRepositoryMock.SetupSequence(p => p.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(sandwich)
            .ReturnsAsync(sandwich2);

        await Assert.ThrowsAsync<DuplicateItemException>(() => _orderService.CreateAsync(order));
    }

    // ----- UpdateAsync -----

    [Fact]
    public async Task UpdateAsync_ValidOrder_RecalculatesTotals()
    {
        var existingOrder = new Order { Id = 1, Items = [] };
        var sandwich = CreateProduct(1, "X-Burger", 20.00m, 1, "Sanduíches");
        var beverage = CreateProduct(2, "Refrigerante", 5.00m, 3, "Bebidas");

        var order = new Order
        {
            Id = 1,
            Items =
            [
                new() { Id = 1, ProductId = 1 },
                new() { Id = 2, ProductId = 2 }
            ]
        };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _productRepositoryMock.SetupSequence(p => p.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(sandwich)
            .ReturnsAsync(beverage);
        _orderRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _orderService.UpdateAsync(order);

        Assert.Equal(25.00m, result.Subtotal.Amount);
        Assert.Equal(25.00m * 0.15m, result.Discount.Amount);
        Assert.Equal(25.00m * 0.85m, result.Total.Amount);
    }

    [Fact]
    public async Task UpdateAsync_ValidOrder_SetsItemOrderIdAndUnitPrice()
    {
        var existingOrder = new Order { Id = 1, Items = [] };
        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");
        var item = new OrderItem { Id = 1, ProductId = 1 };

        var order = new Order { Id = 1, Items = [item] };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(sandwich);
        _orderRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _orderService.UpdateAsync(order);

        Assert.Equal(1, item.OrderId);
        Assert.Equal(15.00m, item.UnitPrice.Amount);
    }

    [Fact]
    public async Task UpdateAsync_ValidOrder_CallsSaveChanges()
    {
        var existingOrder = new Order { Id = 1, Items = [] };
        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");

        var order = new Order { Id = 1, Items = [new() { Id = 1, ProductId = 1 }] };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(sandwich);
        _orderRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _orderService.UpdateAsync(order);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_OrderNotFound_ThrowsEntityNotFoundException()
    {
        var order = new Order { Id = 1 };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Order?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _orderService.UpdateAsync(order));
    }

    [Fact]
    public async Task UpdateAsync_ProductNotFound_ThrowsEntityNotFoundException()
    {
        var existingOrder = new Order { Id = 1, Items = [] };
        var order = new Order { Id = 1, Items = [new() { Id = 1, ProductId = 99 }] };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _productRepositoryMock.Setup(p => p.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _orderService.UpdateAsync(order));
    }

    [Fact]
    public async Task UpdateAsync_ProductInactive_ThrowsBusinessRuleViolationException()
    {
        var existingOrder = new Order { Id = 1, Items = [] };
        var inactiveProduct = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches", isActive: false);

        var order = new Order { Id = 1, Items = [new() { Id = 1, ProductId = 1 }] };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(inactiveProduct);

        await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _orderService.UpdateAsync(order));
    }

    [Fact]
    public async Task UpdateAsync_DuplicateCategories_ThrowsDuplicateItemException()
    {
        var existingOrder = new Order { Id = 1, Items = [] };
        var sandwich1 = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");
        var sandwich2 = CreateProduct(2, "X-Salada", 12.00m, 1, "Sanduíches");

        var order = new Order
        {
            Id = 1,
            Items =
            [
                new() { Id = 1, ProductId = 1 },
                new() { Id = 2, ProductId = 2 }
            ]
        };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _productRepositoryMock.SetupSequence(p => p.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(sandwich1)
            .ReturnsAsync(sandwich2);

        await Assert.ThrowsAsync<DuplicateItemException>(() => _orderService.UpdateAsync(order));
    }

    [Fact]
    public async Task UpdateAsync_QuantityGreaterThanOne_ThrowsInvalidItemQuantityException()
    {
        var existingOrder = new Order { Id = 1, Items = [] };
        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");

        var order = new Order
        {
            Id = 1,
            Items = [new() { Id = 1, ProductId = 1, Quantity = 3 }]
        };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(sandwich);

        await Assert.ThrowsAsync<InvalidItemQuantityException>(() => _orderService.UpdateAsync(order));
    }

    // ----- DeleteAsync -----

    [Fact]
    public async Task DeleteAsync_OrderNotFound_ThrowsEntityNotFoundException()
    {
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Order?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _orderService.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_ValidId_CallsRepository()
    {
        var order = new Order { Id = 1 };
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
        _orderRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _orderService.DeleteAsync(1);

        _orderRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidId_CallsSaveChanges()
    {
        var order = new Order { Id = 1 };
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
        _orderRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _orderService.DeleteAsync(1);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
