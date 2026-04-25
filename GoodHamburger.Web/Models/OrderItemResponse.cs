namespace GoodHamburger.Web.Models;

public record OrderItemResponse(int Id, ProductResponse Product, int Quantity, decimal UnitPrice);
