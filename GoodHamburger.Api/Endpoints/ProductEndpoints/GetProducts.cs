using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.ProductEndpoints;

public static class GetProducts
{
    public static async Task<IResult> Handle(IProductService productService)
    {
        try
        {
            var products = await productService.GetAllAsync();
            
            var response = products.Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.Price.ToString(),
                new ProductCategoryResponse(p.CategoryId, p.Category!.Name)));
            
            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.InternalServerError("Erro ao processar solicitação. Tente novamente em alguns instantes.");
        }
    }
}