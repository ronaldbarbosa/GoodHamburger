namespace GoodHamburger.Web.Models;

public record UpdateOrderRequest(List<OrderItemInput>? Items);
