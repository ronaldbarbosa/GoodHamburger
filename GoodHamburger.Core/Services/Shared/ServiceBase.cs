using GoodHamburger.Core.Entities.Shared;
using GoodHamburger.Core.Interfaces.Repositories.Shared;
using GoodHamburger.Core.Interfaces.Services.Shared;

namespace GoodHamburger.Core.Services.Shared;

public abstract class ServiceBase<TEntity>(IRepositoryBase<TEntity> orderItemRepositoryBase) : IServiceBase<TEntity>
    where TEntity : Entity
{
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync() =>
        await orderItemRepositoryBase.GetAllAsync();

    public virtual async Task<TEntity?> GetByIdAsync(int id) =>
        await orderItemRepositoryBase.GetByIdAsync(id);

    public virtual async Task<TEntity> CreateAsync(TEntity entity) =>
        await orderItemRepositoryBase.AddAsync(entity);

    public virtual async Task UpdateAsync(TEntity entity) =>
        await orderItemRepositoryBase.UpdateAsync(entity);
    
    public virtual async Task DeleteAsync(int id) =>
        await orderItemRepositoryBase.DeleteAsync(id);
}