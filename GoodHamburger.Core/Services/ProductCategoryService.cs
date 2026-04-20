using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Core.Interfaces.Services;
using GoodHamburger.Core.Services.Shared;
using EntityNotFoundException = GoodHamburger.Core.Exceptions.EntityNotFoundException;
using InvalidOperationException = GoodHamburger.Core.Exceptions.InvalidOperationException;

namespace GoodHamburger.Core.Services;

public class ProductCategoryService : ServiceBase<ProductCategory>, IProductCategoryService
{
    private readonly IProductCategoryRepository _repository;

    public ProductCategoryService(IProductCategoryRepository repository) : base(repository)
    {
        _repository = repository;
    }

    public override async Task<ProductCategory> UpdateAsync(ProductCategory entity)
    {
        var existing = await _repository.GetByIdAsync(entity.Id);
        if (existing == null)
            throw new EntityNotFoundException("Categoria", entity.Id);

        entity.Products = existing.Products;
        await base.UpdateAsync(entity);
        return entity;
    }

    public override async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new EntityNotFoundException("Categoria", id);

        if (entity.Products.Any())
            throw new InvalidOperationException($"Categoria '{entity.Name}' possui produtos vinculados.");

        await base.DeleteAsync(id);
    }
}