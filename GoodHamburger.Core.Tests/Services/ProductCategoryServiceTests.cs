using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Core.Services;
using Moq;

namespace GoodHamburger.Core.Tests.Services;

public class ProductCategoryServiceTests
{
    private readonly Mock<IProductCategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ProductCategoryService _categoryService;

    public ProductCategoryServiceTests()
    {
        _categoryRepositoryMock = new Mock<IProductCategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

        _categoryService = new ProductCategoryService(
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task UpdateAsync_ValidCategory_ReturnsCategory()
    {
        var existing = new ProductCategory
        {
            Id = 1,
            Name = "Sandwich",
            Products = new List<Product>()
        };

        var category = new ProductCategory
        {
            Id = 1,
            Name = "Sanduíches"
        };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _categoryRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<ProductCategory>())).Returns(Task.CompletedTask);

        var result = await _categoryService.UpdateAsync(category);

        Assert.NotNull(result);
        Assert.Equal("Sanduíches", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_CategoryNotFound_ThrowsEntityNotFoundException()
    {
        var category = new ProductCategory { Id = 1 };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ProductCategory?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _categoryService.UpdateAsync(category));
    }

    [Fact]
    public async Task DeleteAsync_CategoryNotFound_ThrowsEntityNotFoundException()
    {
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ProductCategory?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _categoryService.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_CategoryWithProducts_ThrowsInvalidOperationException()
    {
        var category = new ProductCategory
        {
            Id = 1,
            Name = "Sandwich",
            Products = new List<Product> { new() { Id = 1, Name = "X-Burger" } }
        };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

        await Assert.ThrowsAsync<Core.Exceptions.BusinessRuleViolationException>(() => _categoryService.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_ValidId_CallsRepository()
    {
        var category = new ProductCategory { Id = 1, Name = "Bebidas", Products = new List<Product>() };
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);
        _categoryRepositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _categoryService.DeleteAsync(1);

        _categoryRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}