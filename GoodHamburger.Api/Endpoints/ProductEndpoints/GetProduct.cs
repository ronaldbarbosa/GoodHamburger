using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.ProductEndpoints;

public static class GetProduct
{
    public static async Task<IResult> Handle(
        IProductService productService,
        int id)
    {
        try
        {
            var product = await productService.GetByIdAsync(id);
            
            if (product is null)
            {
                return Results.NotFound(new { message = "Produto não encontrado" });
            }
            
            var response = new ProductResponse(
                product.Id,
                product.Name,
                product.Price.ToString(),
                new ProductCategoryResponse(product.CategoryId, product.Category!.Name));
            
            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.InternalServerError("Erro ao processar solicitação. Tente novamente em alguns instantes.");
        }
    }
}