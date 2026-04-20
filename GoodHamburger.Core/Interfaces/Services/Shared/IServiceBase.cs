using GoodHamburger.Core.Entities.Shared;

namespace GoodHamburger.Core.Interfaces.Services.Shared;

public interface IServiceBase<T> where T : Entity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}