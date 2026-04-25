namespace GoodHamburger.Web.Models;

public record ProductResponse(int Id, string Name, decimal Price, ProductCategoryResponse Category, string? ImageUrl);
