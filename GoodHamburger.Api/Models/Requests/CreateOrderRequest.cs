namespace GoodHamburger.Api.Models.Requests;

public record CreateOrderRequest(List<CreateOrderItemRequest>? Items);