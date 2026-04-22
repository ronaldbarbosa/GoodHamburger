using GoodHamburger.Api.Models.Requests;
using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderItemEndpoints;

public static class UpdateOrderItem
{
    public static async Task<IResult> Handle(
        IOrderItemService orderItemService,
        int id,
        UpdateOrderItemRequest request)
    {
        try
        {
            var existingOrderItem = await orderItemService.GetByIdAsync(id);
            
            if (existingOrderItem is null)
            {
                var validation = new ValidationResponse([new ValidationItemResponse("id", "Item do pedido não foi encontrado.")]);
                return Results.NotFound(validation);
            }
            
            existingOrderItem.Quantity = request.Quantity;
            existingOrderItem.ProductId = request.ProductId;
            
            await orderItemService.UpdateAsync(existingOrderItem);
            
            var response = new OrderItemResponse(
                existingOrderItem.Id,
                new ProductResponse(
                    existingOrderItem.ProductId, 
                    existingOrderItem.Product!.Name,
                    existingOrderItem.Product.Price.ToString(),
                    new ProductCategoryResponse(existingOrderItem.Product!.CategoryId, existingOrderItem.Product!.Category!.Name)),
                existingOrderItem.Quantity,
                existingOrderItem.UnitPrice.ToString());
            
            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.InternalServerError(new ErrorResponse("Erro ao processar solicitação. Tente novamente em alguns instantes."));
        }
    }
}