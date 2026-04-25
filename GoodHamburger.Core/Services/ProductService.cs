using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Core.Interfaces.Services;
using GoodHamburger.Core.Services.Shared;
using EntityNotFoundException = GoodHamburger.Core.Exceptions.EntityNotFoundException;

namespace GoodHamburger.Core.Services;

public class ProductService : ServiceBase<Product>, IProductService
{
    private readonly IProductRepository _orderItemRepository;
    private readonly IProductCategoryRepository _categoryRepository;

    public ProductService(
        IProductRepository orderItemRepository,
        IProductCategoryRepository categoryRepository,
        IUnitOfWork unitOfWork) : base(orderItemRepository, unitOfWork)
    {
        _orderItemRepository = orderItemRepository;
        _categoryRepository = categoryRepository;
    }

    public override async Task<Product> CreateAsync(Product entity, CancellationToken ct = default)
    {
        var category = await _categoryRepository.GetByIdAsync(entity.CategoryId, ct);
        if (category == null)
            throw new EntityNotFoundException("Categoria", entity.CategoryId);

        if (!category.IsActive)
            throw new BusinessRuleViolationException($"Categoria '{category.Name}' não está ativa.");

        return await base.CreateAsync(entity, ct);
    }

    public override async Task<Product> UpdateAsync(Product entity, CancellationToken ct = default)
    {
        var existing = await _orderItemRepository.GetByIdAsync(entity.Id, ct);
        if (existing == null)
            throw new EntityNotFoundException("Produto", entity.Id);

        if (entity.CategoryId != existing.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(entity.CategoryId, ct);
            if (category == null)
                throw new EntityNotFoundException("Categoria", entity.CategoryId);

            if (!category.IsActive)
                throw new BusinessRuleViolationException($"Categoria '{category.Name}' não está ativa.");

            entity.Category = category;
        }

        await base.UpdateAsync(entity, ct);
        return entity;
    }

    public override async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _orderItemRepository.GetByIdAsync(id, ct);
        if (entity == null)
            throw new EntityNotFoundException("Produto", id);

        await base.DeleteAsync(id, ct);
    }
}