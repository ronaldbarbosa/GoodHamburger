using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Data.Context;
using GoodHamburger.Data.Repositories.Shared;
using GoodHamburger.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Data.Repositories;

public class OrderItemRepository(DataContext context) : RepositoryBase<OrderItem>(context), IOrderItemRepository
{
    public override async Task<OrderItem?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await Context.OrderItems
            .Include(o => o.Product)
            .ThenInclude(p => p!.Category)
            .SingleOrDefaultAsync(o => o.Id == id, ct);
    }

    public override async Task<IEnumerable<OrderItem>> GetAllAsync(CancellationToken ct = default)
    {
        return await Context.OrderItems
            .AsNoTracking()
            .Include(o => o.Product)
            .ThenInclude(p => p!.Category)
            .ToListAsync(ct);
    }

    public override async Task<PaginatedList<OrderItem>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var source = Context.OrderItems
            .AsNoTracking()
            .Include(o => o.Product)
            .ThenInclude(p => p!.Category);

        var totalCount = await source.CountAsync(ct);
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedList<OrderItem>(items, totalCount, pageNumber, pageSize);
    }
}