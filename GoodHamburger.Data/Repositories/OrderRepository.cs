using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Data.Context;
using GoodHamburger.Data.Repositories.Shared;

namespace GoodHamburger.Data.Repositories;

public class OrderRepository(DataContext context) : RepositoryBase<Order>(context), IOrderRepository
{
    
}