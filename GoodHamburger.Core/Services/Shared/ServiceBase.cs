using GoodHamburger.Core.Entities.Shared;
using GoodHamburger.Core.Interfaces.Repositories.Shared;
using GoodHamburger.Core.Interfaces.Services.Shared;

namespace GoodHamburger.Core.Services.Shared;

public abstract class ServiceBase<TEntity>(IRepositoryBase<TEntity> repositoryBase) : IServiceBase<TEntity>
    where TEntity : Entity
{
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync() =>
        await repositoryBase.GetAllAsync();

    public virtual async Task<TEntity?> GetByIdAsync(int id) =>
        await repositoryBase.GetByIdAsync(id);

    public virtual async Task<TEntity> CreateAsync(TEntity entity) =>
        await repositoryBase.AddAsync(entity);

    public virtual async Task UpdateAsync(TEntity entity) =>
        await repositoryBase.UpdateAsync(entity);
    
    public virtual async Task DeleteAsync(int id) =>
        await repositoryBase.DeleteAsync(id);
}