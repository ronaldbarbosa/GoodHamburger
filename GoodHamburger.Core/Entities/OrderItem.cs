using GoodHamburger.Core.Entities.Shared;
using GoodHamburger.Core.ValueObjects;

namespace GoodHamburger.Core.Entities;

public class OrderItem : Entity
{
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; } = 1;
    public Money UnitPrice { get; set; }
}