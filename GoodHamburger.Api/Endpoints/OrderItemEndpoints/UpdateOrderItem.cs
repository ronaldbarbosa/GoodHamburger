using GoodHamburger.Api.Models.Requests;
using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderItemEndpoints;

public static class UpdateOrderItem
{
    public static async Task<IResult> Handle(
        IOrderItemService orderItemService,
        int id,
        UpdateOrderItemRequest request,
        CancellationToken ct)
    {
        try
        {
            var existingOrderItem = await orderItemService.GetByIdAsync(id, ct);
            
            if (existingOrderItem is null)
            {
                var validation = new ValidationResponse([new ValidationItemResponse("id", "Item do pedido não foi encontrado.")]);
                return Results.NotFound(validation);
            }
            
            existingOrderItem.Quantity = request.Quantity;
            existingOrderItem.ProductId = request.ProductId;
            
            await orderItemService.UpdateAsync(existingOrderItem, ct);
            
            var response = new OrderItemResponse(
                existingOrderItem.Id,
                new ProductResponse(
                    existingOrderItem.ProductId, 
                    existingOrderItem.Product!.Name,
                    existingOrderItem.Product.Price.ToString(),
                    new ProductCategoryResponse(existingOrderItem.Product!.CategoryId, existingOrderItem.Product!.Category!.Name),
                existingOrderItem.Product!.ImageUrl),
                existingOrderItem.Quantity,
                existingOrderItem.UnitPrice.ToString());
            
            return Results.Ok(response);
        }
        catch (EntityNotFoundException ex)
        {
            var validation = new ValidationResponse([new ValidationItemResponse(ex.EntityType, ex.Message)]);
            return Results.BadRequest(validation);
        }
        catch (InvalidItemQuantityException ex)
        {
            var validation = new ValidationResponse([new ValidationItemResponse(ex.EntityType, ex.Message)]);
            return Results.BadRequest(validation);
        }
        catch (Exception ex) when(ex is DuplicateItemException or BusinessRuleViolationException)
        {
            var validation = new ValidationResponse([new ValidationItemResponse("", ex.Message)]);
            return Results.BadRequest(validation);
        }
        catch (Exception)
        {
            return Results.InternalServerError(new ErrorResponse("Erro ao processar solicitação. Tente novamente em alguns instantes."));
        }
    }
}