using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderItemEndpoints;

public static class GetOrderItems
{
    public static async Task<IResult> Handle(IOrderItemService orderItemService, CancellationToken ct)
    {
        try
        {
            var orderItems = await orderItemService.GetAllAsync(ct);
            
            var response = orderItems.Select(oi => new OrderItemResponse(
                oi.Id,
                new ProductResponse(
                    oi.ProductId, 
                    oi.Product!.Name,
                    oi.Product.Price.ToString(),
                    new ProductCategoryResponse(oi.Product!.CategoryId, oi.Product!.Category!.Name),
                    oi.Product!.ImageUrl),
                oi.Quantity,
                oi.UnitPrice.ToString()))
                .ToList();
            
            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.InternalServerError(new ErrorResponse("Erro ao processar solicitação. Tente novamente em alguns instantes."));
        }
    }
}