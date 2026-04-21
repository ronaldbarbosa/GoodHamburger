using GoodHamburger.Api.Models.Requests;
using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderEndpoints;

public static class UpdateOrder
{
    public static async Task<IResult> Handle(
        IOrderService orderService,
        int id,
        UpdateOrderRequest request)
    {
        try
        {
            var existingOrder = await orderService.GetByIdAsync(id);
            
            if (existingOrder is null)
            {
                return Results.NotFound(new { message = "Pedido não encontrado" });
            }
            
            if (request.Items is not null)
            {
                existingOrder.Items = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList();
            }
            
            await orderService.UpdateAsync(existingOrder);
            
            var response = new OrderResponse(
                existingOrder.Id,
                existingOrder.Items.Select(oi => new OrderItemResponse(
                    oi.Id,
                    new ProductResponse(
                        oi.ProductId,
                        oi.Product?.Name ?? "",
                        oi.Product?.Price.ToString() ?? "0",
                        new ProductCategoryResponse(oi.Product!.CategoryId, oi.Product!.Category!.Name)),
                    oi.Quantity,
                    oi.UnitPrice.ToString())).ToList(),
                existingOrder.Subtotal.ToString(),
                existingOrder.Discount.ToString(),
                existingOrder.Total.ToString(),
                existingOrder.CreatedAt,
                existingOrder.Status.ToString());
            
            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.InternalServerError("Erro ao processar solicitação. Tente novamente em alguns instantes.");
        }
    }
}