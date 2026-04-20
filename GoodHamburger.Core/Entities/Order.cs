using GoodHamburger.Core.Entities.Shared;
using GoodHamburger.Core.ValueObjects;

namespace GoodHamburger.Core.Entities;

public class Order : Entity
{
    public List<OrderItem> Items { get; set; } = [];
    public Money Subtotal { get; set; }
    public Money Discount { get; set; }
    public Money Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Preparing,
    Ready,
    Completed,
    Cancelled
}