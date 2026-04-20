using GoodHamburger.Core.Entities.Shared;

namespace GoodHamburger.Core.Entities;

public class OrderItem : Entity
{
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}