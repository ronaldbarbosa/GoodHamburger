namespace GoodHamburger.Web.Models;

public record OrderResponse(int Id, List<OrderItemResponse> Items, decimal Subtotal, decimal Discount, decimal Total,
    DateTime CreatedAt, string Status);
