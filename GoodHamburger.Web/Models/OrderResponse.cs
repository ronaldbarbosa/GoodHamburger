namespace GoodHamburger.Web.Models;

public record OrderResponse(int Id, List<OrderItemResponse> Items, string Subtotal, string Discount, string Total,
    DateTime CreatedAt, string Status);
