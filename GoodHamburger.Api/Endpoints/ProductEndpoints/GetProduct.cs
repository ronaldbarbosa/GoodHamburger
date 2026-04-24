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
                var validation = new ValidationResponse([new ValidationItemResponse("orderId", "Produto não  encontrado.")]);
                return Results.NotFound(validation);
            }
            
            var response = new ProductResponse(
                product.Id,
                product.Name,
                product.Price.ToString(),
                new ProductCategoryResponse(product.CategoryId, product.Category!.Name),
                product.ImageUrl);
            
            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.InternalServerError(new ErrorResponse("Erro ao processar solicitação. Tente novamente em alguns instantes."));
        }
    }
}