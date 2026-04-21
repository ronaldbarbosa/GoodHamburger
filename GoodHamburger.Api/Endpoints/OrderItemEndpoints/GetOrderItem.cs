using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderItemEndpoints;

public static class GetOrderItem
{
    public static async Task<IResult> Handle(
        IOrderItemService orderItemService,
        int id)
    {
        try
        {
            var orderItem = await orderItemService.GetByIdAsync(id);
            
            if (orderItem is null)
            {
                return Results.NotFound(new { message = "Item de pedido não encontrado" });
            }
            
            var response = new OrderItemResponse(
                orderItem.Id,
                new ProductResponse(
                    orderItem.ProductId, 
                    orderItem.Product!.Name,
                    orderItem.Product.Price.ToString(),
                    new ProductCategoryResponse(orderItem.Product!.CategoryId, orderItem.Product!.Category!.Name)),
                orderItem.Quantity,
                orderItem.UnitPrice.ToString());
            
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError("Erro ao processar solicitação. Tente novamente em alguns instantes.");
        }
    }
}