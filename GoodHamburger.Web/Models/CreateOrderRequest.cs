namespace GoodHamburger.Web.Models;

public record CreateOrderRequest(List<CreateOrderItemRequest>? Items);
