namespace GoodHamburger.Api.Models.Requests;

public record CreateOrderRequest(List<OrderItemCreateOrderRequest>? Items);

public record OrderItemCreateOrderRequest(int ProductId, int Quantity);