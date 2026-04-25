using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Interfaces.Services;
using GoodHamburger.Shared.Pagination;

namespace GoodHamburger.Api.Endpoints.OrderEndpoints;

public static class GetOrders
{
    public static async Task<IResult> Handle(
        IOrderService orderService,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 50)
            {
                var validation = new ValidationResponse([
                    new ValidationItemResponse("paginação", "pageNumber deve ser >= 1 e pageSize entre 1 e 50.")
                ]);
                return Results.BadRequest(validation);
            }

            var paged = await orderService.GetPagedAsync(pageNumber, pageSize, ct);

            var response = new PaginatedList<OrderResponse>(
                paged.Items.Select(order => new OrderResponse(
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
                    order.Status.ToString())).ToList(),
                paged.TotalItemCount,
                paged.PageNumber,
                paged.PageSize);

            return Results.Ok(response);
        }
        catch (Exception)
        {
            return Results.InternalServerError(new ErrorResponse("Erro ao processar solicitação. Tente novamente em alguns instantes."));
        }
    }
}
