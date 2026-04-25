using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Core.Interfaces.Services;
using GoodHamburger.Core.Services.Shared;

namespace GoodHamburger.Core.Services;

public class ProductCategoryService : ServiceBase<ProductCategory>, IProductCategoryService
{
    private readonly IProductCategoryRepository _orderItemRepository;

    public ProductCategoryService(
        IProductCategoryRepository orderItemRepository,
        IUnitOfWork unitOfWork) : base(orderItemRepository, unitOfWork)
    {
        _orderItemRepository = orderItemRepository;
    }

    public override async Task<ProductCategory> UpdateAsync(ProductCategory entity, CancellationToken ct = default)
    {
        var existing = await _orderItemRepository.GetByIdAsync(entity.Id, ct);
        if (existing == null)
            throw new EntityNotFoundException("Categoria", entity.Id);

        entity.Products = existing.Products;
        await base.UpdateAsync(entity, ct);
        return entity;
    }

    public override async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _orderItemRepository.GetByIdAsync(id, ct);
        if (entity == null)
            throw new EntityNotFoundException("Categoria", id);

        if (entity.Products.Any())
            throw new BusinessRuleViolationException($"Categoria '{entity.Name}' possui produtos vinculados.");

        await base.DeleteAsync(id, ct);
    }
}