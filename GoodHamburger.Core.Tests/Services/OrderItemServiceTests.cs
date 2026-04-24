using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Core.Services;
using GoodHamburger.Core.ValueObjects;
using Moq;

namespace GoodHamburger.Core.Tests.Services;

public class OrderItemServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly OrderItemService _orderItemService;

    public OrderItemServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _orderItemRepositoryMock = new Mock<IOrderItemRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var orderService = new OrderService(
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object,
            new TestableDiscountCalculatorService(),
            _unitOfWorkMock.Object);

        _orderItemService = new OrderItemService(
            orderService,
            _orderItemRepositoryMock.Object,
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);
    }

    private class TestableDiscountCalculatorService : DiscountCalculatorService { }

    private static Product CreateProduct(int id, string name, decimal price, int categoryId, string categoryName) =>
        new() { Id = id, Name = name, Price = new Money(price), CategoryId = categoryId, Category = new ProductCategory { Id = categoryId, Name = categoryName }, IsActive = true };

    [Fact]
    public async Task CreateAsync_DuplicateCategoryInExistingOrder_ThrowsDuplicateItemException()
    {
        var existingSandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");
        var existingOrder = new Order
        {
            Id = 1,
            Items = [new OrderItem { Id = 1, ProductId = 1, Product = existingSandwich, Quantity = 1 }]
        };

        var newSandwich = CreateProduct(2, "X-Egg", 17.00m, 1, "Sanduíches");
        var newItem = new OrderItem { ProductId = 2, OrderId = 1, Quantity = 1 };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _productRepositoryMock.Setup(p => p.GetByIdAsync(2)).ReturnsAsync(newSandwich);

        await Assert.ThrowsAsync<DuplicateItemException>(() => _orderItemService.CreateAsync(newItem));
    }

    [Fact]
    public async Task CreateAsync_QuantityGreaterThanOne_ThrowsInvalidItemQuantityException()
    {
        var existingOrder = new Order { Id = 1, Items = [] };
        var sandwich = CreateProduct(1, "X-Burger", 15.00m, 1, "Sanduíches");
        var newItem = new OrderItem { ProductId = 1, OrderId = 1, Quantity = 2 };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingOrder);
        _productRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(sandwich);

        await Assert.ThrowsAsync<InvalidItemQuantityException>(() => _orderItemService.CreateAsync(newItem));
    }
}
