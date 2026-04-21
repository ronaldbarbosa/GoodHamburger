using GoodHamburger.Core.Entities.Shared;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces.Repositories.Shared;
using GoodHamburger.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Data.Repositories.Shared;

public abstract class RepositoryBase<TEntity>(DataContext context) : IRepositoryBase<TEntity> where TEntity : Entity
{
    protected readonly DataContext Context = context;
    
    public async Task<IEnumerable<TEntity>> GetAllAsync() =>
        await Context.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync();

    public async Task<TEntity?> GetByIdAsync(int id) =>
        await Context.Set<TEntity>().FindAsync(id);

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        Context.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        
        if (entity == null)
            throw new EntityNotFoundException(nameof(TEntity), id);
        
        Context.Set<TEntity>().Remove(entity);
        await Context.SaveChangesAsync();
    }
}