namespace GoodHamburger.Api.Models.Responses;

public record ProductResponse(int Id, string Name, string Price, ProductCategoryResponse Category);