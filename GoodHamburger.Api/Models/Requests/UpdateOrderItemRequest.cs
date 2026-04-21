namespace GoodHamburger.Api.Models.Requests;

public record UpdateOrderItemRequest(int ProductId, int Quantity);