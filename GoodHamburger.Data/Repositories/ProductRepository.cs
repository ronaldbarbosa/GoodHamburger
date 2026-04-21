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
        return await context.Products
            .Include(p => p.Category)
            .SingleOrDefaultAsync(o => o.Id == id);
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await  context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }
}