using System.Net.Http.Json;

namespace GoodHamburger.Web.Services;

public class OrderItemService(HttpClient client)
{
    private readonly HttpClient _client = client;
    private const string OrderItemsPath = "/api/order-items";

    public async Task<List<OrderItemResponse>> GetAllAsync()
    {
        var response = await _client.GetFromJsonAsync<List<OrderItemResponse>>(OrderItemsPath);
        return response ?? [];
    }

    public async Task<OrderItemResponse?> GetByIdAsync(int id)
    {
        try
        {
            return await _client.GetFromJsonAsync<OrderItemResponse>($"{OrderItemsPath}/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<OrderItemResponse> CreateAsync(CreateOrderItemRequest request)
    {
        var response = await _client.PostAsJsonAsync(OrderItemsPath, request);
        return await response.Content.ReadFromJsonAsync<OrderItemResponse>();
    }

    public async Task<OrderItemResponse> UpdateAsync(int id, UpdateOrderItemRequest request)
    {
        var response = await _client.PutAsJsonAsync($"{OrderItemsPath}/{id}", request);
        return await response.Content.ReadFromJsonAsync<OrderItemResponse>();
    }

    public async Task DeleteAsync(int id)
    {
        await _client.DeleteAsync($"{OrderItemsPath}/{id}");
    }
}

public record OrderItemResponse(int Id, ProductResponse Product, int Quantity, string UnitPrice);
public record ProductResponse(int Id, string Name, string Price, ProductCategoryResponse Category);
public record ProductCategoryResponse(int Id, string Name);
public record CreateOrderItemRequest(int ProductId, int Quantity, int? OrderId);
public record UpdateOrderItemRequest(int ProductId, int Quantity);