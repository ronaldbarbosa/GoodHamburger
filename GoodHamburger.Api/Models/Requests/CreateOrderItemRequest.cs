namespace GoodHamburger.Api.Models.Requests;

public record CreateOrderItemRequest(int ProductId, int Quantity, int? OrderId);