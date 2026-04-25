using GoodHamburger.Core.Entities.Shared;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces.Repositories.Shared;
using GoodHamburger.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Data.Repositories.Shared;

public abstract class RepositoryBase<TEntity>(DataContext context) : IRepositoryBase<TEntity> where TEntity : Entity
{
    protected readonly DataContext Context = context;
    
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default) =>
        await Context.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync(ct);

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await Context.Set<TEntity>().FindAsync([id], ct);

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, ct);
        return entity;
    }

    public virtual Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        Context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);

        if (entity == null)
            throw new EntityNotFoundException(nameof(TEntity), id);

        Context.Set<TEntity>().Remove(entity);
    }
}