using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.ProductCategoryEndpoints;

public static class GetProductCategories
{
    public static async Task<IResult> Handle(IProductCategoryService productCategoryService, CancellationToken ct)
    {
        var categories = await productCategoryService.GetAllAsync(ct);
        var response = categories.Select(c => new ProductCategoryResponse(c.Id, c.Name));
        return Results.Ok(response);
    }
}
