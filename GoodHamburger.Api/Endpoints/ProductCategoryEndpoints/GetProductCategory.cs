using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.ProductCategoryEndpoints;

public static class GetProductCategory
{
    public static async Task<IResult> Handle(
        IProductCategoryService productCategoryService,
        int id,
        CancellationToken ct)
    {
        var category = await productCategoryService.GetByIdAsync(id, ct);

        if (category is null)
        {
            var validation = new ValidationResponse([new ValidationItemResponse("id", "Categoria não encontrada.")]);
            return Results.NotFound(validation);
        }

        var response = new ProductCategoryResponse(category.Id, category.Name);

        return Results.Ok(response);
    }
}
