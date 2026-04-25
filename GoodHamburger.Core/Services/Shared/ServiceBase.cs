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
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default) =>
        await repository.GetAllAsync(ct);

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await repository.GetByIdAsync(id, ct);

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default)
    {
        var result = await repository.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return result;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        await repository.UpdateAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await repository.DeleteAsync(id, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}