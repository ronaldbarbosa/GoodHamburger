using GoodHamburger.Core.Entities.Shared;
using GoodHamburger.Core.Interfaces;
using GoodHamburger.Core.Interfaces.Repositories.Shared;
using GoodHamburger.Core.Interfaces.Services.Shared;

namespace GoodHamburger.Core.Services.Shared;

public abstract class ServiceBase<TEntity>(
    IRepositoryBase<TEntity> repository,
    IUnitOfWork unitOfWork) : IServiceBase<TEntity>
    where TEntity : Entity
{
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync() =>
        await repository.GetAllAsync();

    public virtual async Task<TEntity?> GetByIdAsync(int id) =>
        await repository.GetByIdAsync(id);

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        var result = await repository.AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
        return result;
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        await repository.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        await repository.DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}