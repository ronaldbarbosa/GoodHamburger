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
        UpdateOrderRequest request,
        CancellationToken ct)
    {
        var existingOrder = await orderService.GetByIdAsync(id, ct);

        if (existingOrder is null)
        {
            var validation = new ValidationResponse([new ValidationItemResponse("id", "Pedido não encontrado.")]);
            return Results.NotFound(validation);
        }

        existingOrder.Items = request.Items!.Select(i => new OrderItem
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity
        }).ToList();

        await orderService.UpdateAsync(existingOrder, ct);

        var response = new OrderResponse(
            existingOrder.Id,
            existingOrder.Items.Select(oi => new OrderItemResponse(
                oi.Id,
                new ProductResponse(
                    oi.ProductId,
                    oi.Product!.Name,
                    oi.Product!.Price.Amount,
                    new ProductCategoryResponse(oi.Product!.CategoryId, oi.Product!.Category!.Name),
                    oi.Product!.ImageUrl),
                oi.Quantity,
                oi.UnitPrice.Amount)).ToList(),
            existingOrder.Subtotal.Amount,
            existingOrder.Discount.Amount,
            existingOrder.Total.Amount,
            existingOrder.CreatedAt,
            existingOrder.Status.ToString());

        return Results.Ok(response);
    }
}
