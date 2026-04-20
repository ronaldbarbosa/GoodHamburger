using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Services.Shared;

namespace GoodHamburger.Core.Interfaces.Services;

public interface IOrderService : IServiceBase<Order>
{
    void RecalculateTotals(Order order);
}