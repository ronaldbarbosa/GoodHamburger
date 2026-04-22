using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Services.Shared;

namespace GoodHamburger.Core.Interfaces.Services;

public interface IOrderService : IServiceBase<Order>
{
    void ValidateDuplicateCategories(IEnumerable<Product> products);
    void ValidateProductQuantity(IEnumerable<OrderItem> orderItems);
    void RecalculateTotals(Order order);
}