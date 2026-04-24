namespace GoodHamburger.Web.Models;

public record ProductResponse(int Id, string Name, string Price, ProductCategoryResponse Category, string? ImageUrl);
