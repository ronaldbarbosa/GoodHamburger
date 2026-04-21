using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.ProductCategoryEndpoints;

public static class GetProductCategory
{
    public static async Task<IResult> Handle(
        IProductCategoryService productCategoryService,
        int id)
    {
        try
        {
            var category = await productCategoryService.GetByIdAsync(id);
            
            if (category is null)
            {
                return Results.NotFound(new { message = "Categoria não encontrada" });
            }
            
            var response = new ProductCategoryResponse(category.Id, category.Name);
            
            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.InternalServerError("Erro ao processar solicitação. Tente novamente em alguns instantes.");
        }
    }
}