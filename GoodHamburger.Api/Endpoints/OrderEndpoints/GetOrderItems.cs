using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderEndpoints;

public static class GetOrderItems
{
    public static async Task<IResult> Handle(
        IOrderService orderService,
        int orderId,
        CancellationToken ct)
    {
        var order = await orderService.GetByIdAsync(orderId, ct);

        if (order is null)
        {
            var validation = new ValidationResponse([new ValidationItemResponse("orderId", "Pedido não encontrado.")]);
            return Results.NotFound(validation);
        }

        var response = order.Items.Select(oi => new OrderItemResponse(
            oi.Id,
            new ProductResponse(
                oi.ProductId,
                oi.Product!.Name,
                oi.Product!.Price.Amount,
                new ProductCategoryResponse(oi.Product!.CategoryId, oi.Product!.Category!.Name),
                oi.Product!.ImageUrl),
            oi.Quantity,
            oi.UnitPrice.Amount));

        return Results.Ok(response);
    }
}
