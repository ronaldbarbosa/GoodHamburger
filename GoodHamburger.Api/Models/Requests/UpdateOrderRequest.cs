namespace GoodHamburger.Api.Models.Requests;

public record UpdateOrderRequest(List<OrderItemInput>? Items);

public record OrderItemInput(int ProductId, int Quantity);