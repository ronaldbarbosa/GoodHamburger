namespace GoodHamburger.Api.Models.Responses;

public record ProductResponse(int Id, string Name, decimal Price, ProductCategoryResponse Category, string? ImageUrl);