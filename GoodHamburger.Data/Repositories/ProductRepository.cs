using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Data.Context;
using GoodHamburger.Data.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Data.Repositories;

public class ProductRepository(DataContext context) : RepositoryBase<Product>(context), IProductRepository  
{
    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await Context.Products
            .Include(p => p.Category)
            .SingleOrDefaultAsync(o => o.Id == id);
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await  Context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .ToListAsync();
    }
}