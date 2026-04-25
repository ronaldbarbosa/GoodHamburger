using GoodHamburger.Core.Entities.Shared;
using GoodHamburger.Shared.Pagination;

namespace GoodHamburger.Core.Interfaces.Repositories.Shared;

public interface IRepositoryBase<T> where T : Entity
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task<PaginatedList<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct = default);
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}