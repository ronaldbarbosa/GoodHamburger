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
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

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

    [Fact]
    public async Task CreateAsync_ValidOrder_ReturnsOrder()
    {
        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");
        var side = CreateProduct(2, "Batata Frita", 8.00m, 2, "Acompanhamentos");
        var beverage = CreateProduct(3, "Refrigerante", 5.00m, 3, "Bebidas");

        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new() { Id = 1, ProductId = 1 },
                new() { Id = 2, ProductId = 2 },
                new() { Id = 3, ProductId = 3 }
            }
        };

        _productRepositoryMock.SetupSequence(p => p.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(sandwich)
            .ReturnsAsync(side)
            .ReturnsAsync(beverage);

        _orderRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

        var result = await _orderService.CreateAsync(order);

        Assert.NotNull(result);
        Assert.Equal(28.00m, result.Subtotal.Amount);
        Assert.Equal(28.00m * 0.20m, result.Discount.Amount);
    }

    [Fact]
    public async Task CreateAsync_ProductNotFound_ThrowsEntityNotFoundException()
    {
        var order = new Order
        {
            Items = new List<OrderItem> { new() { Id = 1, ProductId = 1 } }
        };

        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _orderService.CreateAsync(order));
    }

    [Fact]
    public async Task CreateAsync_ProductInactive_ThrowsInvalidOperationException()
    {
        var product = CreateProduct(1, "X-Burger", 15.00m, 1, "Sandwich", isActive: false);

        var order = new Order
        {
            Items = new List<OrderItem> { new() { Id = 1, ProductId = 1 } }
        };

        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(product);

        await Assert.ThrowsAsync<Core.Exceptions.BusinessRuleViolationException>(() => _orderService.CreateAsync(order));
    }

    [Fact]
    public async Task CreateAsync_QuantityGreaterThanOne_ThrowsInvalidItemQuantityException()
    {
        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");

        var order = new Order
        {
            Items = new List<OrderItem> { new() { Id = 1, ProductId = 1, Quantity = 2 } }
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
            Items = new List<OrderItem>
            {
                new() { Id = 1, ProductId = 1 },
                new() { Id = 2, ProductId = 2 }
            }
        };

        _productRepositoryMock.SetupSequence(p => p.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(sandwich)
            .ReturnsAsync(sandwich2);

        await Assert.ThrowsAsync<DuplicateItemException>(() => _orderService.CreateAsync(order));
    }

    [Fact]
    public async Task UpdateAsync_ValidOrder_ReturnsUpdatedOrder()
    {
        var existingOrder = new Order
        {
            Id = 1,
            Items = new List<OrderItem>()
        };

        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");

        var order = new Order
        {
            Id = 1,
            Items = new List<OrderItem> { new() { Id = 1, ProductId = 1 } }
        };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(sandwich);
        _orderRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

        var result = await _orderService.UpdateAsync(order);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_OrderNotFound_ThrowsEntityNotFoundException()
    {
        var order = new Order { Id = 1 };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Order?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _orderService.UpdateAsync(order));
    }

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
        _orderRepositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _orderService.DeleteAsync(1);

        _orderRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}