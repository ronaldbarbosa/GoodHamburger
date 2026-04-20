using GoodHamburger.Core.Entities.Shared;

namespace GoodHamburger.Core.Interfaces.Repositories.Shared;

public interface IRepositoryBase<T> where T : Entity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}