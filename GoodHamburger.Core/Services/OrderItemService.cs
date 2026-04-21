using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Core.Interfaces.Services;
using GoodHamburger.Core.Services.Shared;

namespace GoodHamburger.Core.Services;

public class OrderItemService : ServiceBase<OrderItem>, IOrderItemService
{
    public OrderItemService(IOrderItemRepository repository) : base(repository)
    {
        
    }
}