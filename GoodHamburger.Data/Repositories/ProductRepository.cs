using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Data.Context;
using GoodHamburger.Data.Repositories.Shared;
using GoodHamburger.Core.Common;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Data.Repositories;

public class ProductRepository(DataContext context) : RepositoryBase<Product>(context), IProductRepository  
{
    public override async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await Context.Products
            .Include(p => p.Category)
            .SingleOrDefaultAsync(o => o.Id == id, ct);
    }

    public override async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default)
    {
        return await Context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .ToListAsync(ct);
    }

    public override async Task<PaginatedList<Product>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var source = Context.Products
            .AsNoTracking()
            .Include(p => p.Category);

        var totalCount = await source.CountAsync(ct);
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedList<Product>(items, totalCount, pageNumber, pageSize);
    }
}