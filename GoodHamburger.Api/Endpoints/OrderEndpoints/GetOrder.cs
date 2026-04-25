using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderEndpoints;

public static class GetOrder
{
    public static async Task<IResult> Handle(
        IOrderService orderService,
        int id,
        CancellationToken ct)
    {
        var order = await orderService.GetByIdAsync(id, ct);

        if (order is null)
        {
            var validation = new ValidationResponse([new ValidationItemResponse("id", "Pedido não encontrado.")]);
            return Results.NotFound(validation);
        }

        var response = new OrderResponse(
            order.Id,
            order.Items.Select(oi => new OrderItemResponse(
                oi.Id,
                new ProductResponse(
                    oi.ProductId,
                    oi.Product!.Name,
                    oi.Product!.Price.ToString(),
                    new ProductCategoryResponse(oi.Product!.CategoryId, oi.Product!.Category!.Name),
                    oi.Product!.ImageUrl),
                oi.Quantity,
                oi.UnitPrice.ToString())).ToList(),
            order.Subtotal.ToString(),
            order.Discount.ToString(),
            order.Total.ToString(),
            order.CreatedAt,
            order.Status.ToString());

        return Results.Ok(response);
    }
}
