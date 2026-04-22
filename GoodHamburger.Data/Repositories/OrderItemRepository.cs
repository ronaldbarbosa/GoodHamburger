using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Data.Context;
using GoodHamburger.Data.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Data.Repositories;

public class OrderItemRepository(DataContext context) : RepositoryBase<OrderItem>(context), IOrderItemRepository
{
    public override async Task<OrderItem?> GetByIdAsync(int id)
    {
        return await Context.OrderItems
            .Include(o => o.Product)
            .ThenInclude(p => p!.Category)
            .SingleOrDefaultAsync(o => o.Id == id);
    }

    public override async Task<IEnumerable<OrderItem>> GetAllAsync()
    {
        return await Context.OrderItems
            .AsNoTracking()
            .Include(o => o.Product)
            .ThenInclude(p => p!.Category)
            .ToListAsync();
    }
}