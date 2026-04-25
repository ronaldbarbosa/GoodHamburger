namespace GoodHamburger.Web.Models;

public record CartItem(int Id, int ProductId, string ProductName, string CategoryName, int Quantity, decimal UnitPrice, string? ImageUrl);
