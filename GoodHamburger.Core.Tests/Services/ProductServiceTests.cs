using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Core.Services;
using GoodHamburger.Core.ValueObjects;
using Moq;

namespace GoodHamburger.Core.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IProductCategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<IProductCategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

        _productService = new ProductService(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    private static ProductCategory CreateCategory(int id, string name, bool isActive = true) =>
        new() { Id = id, Name = name, IsActive = isActive };

    [Fact]
    public async Task CreateAsync_ValidProduct_ReturnsProduct()
    {
        var category = CreateCategory(1, "Sandwich");
        var product = new Product
        {
            Name = "X-Burger",
            Price = new Money(15.00m),
            CategoryId = 1
        };

        _categoryRepositoryMock.Setup(c => c.GetByIdAsync(1)).ReturnsAsync(category);
        _productRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p);

        var result = await _productService.CreateAsync(product);

        Assert.NotNull(result);
        Assert.Equal("X-Burger", result.Name);
    }

    [Fact]
    public async Task CreateAsync_CategoryNotFound_ThrowsEntityNotFoundException()
    {
        var product = new Product
        {
            Name = "X-Burger",
            CategoryId = 1
        };

        _categoryRepositoryMock.Setup(c => c.GetByIdAsync(1)).ReturnsAsync((ProductCategory?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _productService.CreateAsync(product));
    }

    [Fact]
    public async Task CreateAsync_CategoryInactive_ThrowsInvalidOperationException()
    {
        var category = CreateCategory(1, "Sandwich", isActive: false);
        var product = new Product
        {
            Name = "X-Burger",
            CategoryId = 1
        };

        _categoryRepositoryMock.Setup(c => c.GetByIdAsync(1)).ReturnsAsync(category);

        await Assert.ThrowsAsync<Core.Exceptions.BusinessRuleViolationException>(() => _productService.CreateAsync(product));
    }

    [Fact]
    public async Task UpdateAsync_ValidProduct_ReturnsProduct()
    {
        var existingProduct = new Product
        {
            Id = 1,
            Name = "X-Burger",
            Price = new Money(15.00m),
            CategoryId = 1,
            Category = CreateCategory(1, "Sandwich")
        };

        var product = new Product
        {
            Id = 1,
            Name = "X-Burger Updated",
            CategoryId = 1
        };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

        var result = await _productService.UpdateAsync(product);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_ProductNotFound_ThrowsEntityNotFoundException()
    {
        var product = new Product { Id = 1 };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _productService.UpdateAsync(product));
    }

    [Fact]
    public async Task UpdateAsync_ChangeToInactiveCategory_ThrowsInvalidOperationException()
    {
        var existingProduct = new Product
        {
            Id = 1,
            CategoryId = 1,
            Category = CreateCategory(1, "Sandwich")
        };

        var inactiveCategory = CreateCategory(2, "Bebidas", isActive: false);
        var product = new Product { Id = 1, CategoryId = 2 };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingProduct);
        _categoryRepositoryMock.Setup(c => c.GetByIdAsync(2)).ReturnsAsync(inactiveCategory);

        await Assert.ThrowsAsync<Core.Exceptions.BusinessRuleViolationException>(() => _productService.UpdateAsync(product));
    }

    [Fact]
    public async Task DeleteAsync_ProductNotFound_ThrowsEntityNotFoundException()
    {
        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _productService.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_ValidId_CallsRepository()
    {
        var product = new Product { Id = 1 };
        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _productRepositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _productService.DeleteAsync(1);

        _productRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}