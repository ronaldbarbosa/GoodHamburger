using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Data.Context;
using GoodHamburger.Data.Repositories.Shared;

namespace GoodHamburger.Data.Repositories;

public class ProductCategoryRepository(DataContext context) : RepositoryBase<ProductCategory>(context), IProductCategoryRepository
{
    
}