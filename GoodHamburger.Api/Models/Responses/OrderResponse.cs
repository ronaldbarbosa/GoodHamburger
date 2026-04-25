namespace GoodHamburger.Api.Models.Responses;

public record OrderResponse(int Id, List<OrderItemResponse> Items, decimal Subtotal, decimal Discount, decimal Total,
    DateTime CreatedAt, string Status);