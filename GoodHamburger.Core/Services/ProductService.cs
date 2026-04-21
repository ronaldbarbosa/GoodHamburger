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
    private readonly IProductRepository _repository;
    private readonly IProductCategoryRepository _categoryRepository;

    public ProductService(
        IProductRepository repository,
        IProductCategoryRepository categoryRepository) : base(repository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public override async Task<Product> CreateAsync(Product entity)
    {
        var category = await _categoryRepository.GetByIdAsync(entity.CategoryId);
        if (category == null)
            throw new EntityNotFoundException("Categoria", entity.CategoryId);

        if (!category.IsActive)
            throw new BusinessRuleViolationException($"Categoria '{category.Name}' não está ativa.");
        
        return await base.CreateAsync(entity);
    }

    public override async Task<Product> UpdateAsync(Product entity)
    {
        var existing = await _repository.GetByIdAsync(entity.Id);
        if (existing == null)
            throw new EntityNotFoundException("Produto", entity.Id);

        if (entity.CategoryId != existing.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(entity.CategoryId);
            if (category == null)
                throw new EntityNotFoundException("Categoria", entity.CategoryId);

            if (!category.IsActive)
                throw new BusinessRuleViolationException($"Categoria '{category.Name}' não está ativa.");

            entity.Category = category;
        }

        await base.UpdateAsync(entity);
        return entity;
    }

    public override async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new EntityNotFoundException("Produto", id);

        await base.DeleteAsync(id);
    }
}