using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderItemEndpoints;

public static class GetOrderItems
{
    public static async Task<IResult> Handle(IOrderItemService orderItemService)
    {
        try
        {
            var orderItems = await orderItemService.GetAllAsync();
            
            var response = orderItems.Select(oi => new OrderItemResponse(
                oi.Id,
                new ProductResponse(
                    oi.ProductId, 
                    oi.Product!.Name,
                    oi.Product.Price.ToString(),
                    new ProductCategoryResponse(oi.Product.Category!.Name)),
                oi.Quantity,
                oi.UnitPrice.ToString()));
            
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError("Erro ao processar solicitação. Tente novamente em alguns instantes.");
        }
    }
}