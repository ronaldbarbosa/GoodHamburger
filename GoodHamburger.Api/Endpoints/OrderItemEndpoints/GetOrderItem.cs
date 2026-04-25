using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderItemEndpoints;

public static class GetOrderItem
{
    public static async Task<IResult> Handle(
        IOrderItemService orderItemService,
        int id,
        CancellationToken ct)
    {
        var orderItem = await orderItemService.GetByIdAsync(id, ct);

        if (orderItem is null)
        {
            var validation = new ValidationResponse([new ValidationItemResponse("id", "Item do pedido não foi encontrado.")]);
            return Results.NotFound(validation);
        }

        var response = new OrderItemResponse(
            orderItem.Id,
            new ProductResponse(
                orderItem.ProductId,
                orderItem.Product!.Name,
                orderItem.Product.Price.Amount,
                new ProductCategoryResponse(orderItem.Product!.CategoryId, orderItem.Product!.Category!.Name),
                orderItem.Product!.ImageUrl),
            orderItem.Quantity,
            orderItem.UnitPrice.Amount);

        return Results.Ok(response);
    }
}
