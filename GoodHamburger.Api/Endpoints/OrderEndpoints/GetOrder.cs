using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderEndpoints;

public static class GetOrder
{
    public static async Task<IResult> Handle(
        IOrderService orderService,
        int id)
    {
        try
        {
            var order = await orderService.GetByIdAsync(id);
            
            if (order is null)
            {
                return Results.NotFound(new { message = "Pedido não encontrado" });
            }
            
            var response = new OrderResponse(
                order.Id,
                order.Items.Select(oi => new OrderItemResponse(
                    oi.Id,
                    new ProductResponse(
                        oi.ProductId,
                        oi.Product?.Name ?? "",
                        oi.Product?.Price.ToString() ?? "0",
                        new ProductCategoryResponse(oi.Product?.Category?.Name ?? "")),
                    oi.Quantity,
                    oi.UnitPrice.ToString())).ToList(),
                order.Subtotal.ToString(),
                order.Discount.ToString(),
                order.Total.ToString(),
                order.CreatedAt,
                order.Status.ToString());
            
            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.InternalServerError("Erro ao processar solicitação. Tente novamente em alguns instantes.");
        }
    }
}