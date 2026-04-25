using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.ProductEndpoints;

public static class GetProducts
{
    public static async Task<IResult> Handle(IProductService productService, CancellationToken ct)
    {
        try
        {
            var products = await productService.GetAllAsync(ct);
            
            var response = products.Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.Price.ToString(),
                new ProductCategoryResponse(p.CategoryId, p.Category!.Name),
                p.ImageUrl));
            
            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.InternalServerError(new ErrorResponse("Erro ao processar solicitação. Tente novamente em alguns instantes."));
        }
    }
}