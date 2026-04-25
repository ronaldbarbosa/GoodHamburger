using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Data.Context;
using GoodHamburger.Data.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Data.Repositories;

public class OrderRepository(DataContext context) : RepositoryBase<Order>(context), IOrderRepository
{
    public override async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await Context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p!.Category)
            .SingleOrDefaultAsync(o => o.Id == id, ct);
    }

    public override async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default)
    {
        return await Context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p!.Category)
            .ToListAsync(ct);
    }
}