namespace GoodHamburger.Api.Models.Responses;

public record OrderItemResponse(int Id, ProductResponse Product, int Quantity, decimal UnitPrice);