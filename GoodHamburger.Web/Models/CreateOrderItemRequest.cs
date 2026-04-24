namespace GoodHamburger.Web.Models;

public record CreateOrderItemRequest(int ProductId, int Quantity, int? OrderId);
