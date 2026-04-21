namespace GoodHamburger.Api.Models.Responses;

public record OrderResponse(int Id, List<OrderItemResponse> Items, string Subtotal, string Discount, string Total,
    DateTime CreatedAt, string Status);