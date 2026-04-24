using System.Net.Http.Json;

namespace GoodHamburger.Web.Services;

public class OrderService(HttpClient client)
{
    private readonly HttpClient _client = client;
    private const string OrdersPath = "/api/orders";

    public async Task<List<OrderResponse>> GetAllAsync()
    {
        var response = await _client.GetFromJsonAsync<List<OrderResponse>>(OrdersPath);
        return response ?? [];
    }

    public async Task<OrderResponse?> GetByIdAsync(int id)
    {
        try
        {
            return await _client.GetFromJsonAsync<OrderResponse>($"{OrdersPath}/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
    {
        var response = await _client.PostAsJsonAsync(OrdersPath, request);
        return await response.Content.ReadFromJsonAsync<OrderResponse>();
    }

    public async Task<OrderResponse?> UpdateAsync(int id, UpdateOrderRequest request)
    {
        var response = await _client.PutAsJsonAsync($"{OrdersPath}/{id}", request);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<OrderResponse>();
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        var response = await _client.PatchAsJsonAsync($"{OrdersPath}/{id}/status", new UpdateOrderStatusRequest(status));
        return response.IsSuccessStatusCode;
    }

    public async Task CancelAsync(int id)
    {
        await _client.PatchAsync($"{OrdersPath}/{id}/cancel", null);
    }

    public async Task DeleteAsync(int id)
    {
        await _client.DeleteAsync($"{OrdersPath}/{id}");
    }
}

public record OrderResponse(int Id, List<OrderItemResponse> Items, string Subtotal, string Discount, string Total,
    DateTime CreatedAt, string Status);
public record CreateOrderRequest(List<CreateOrderItemRequest>? Items);
public record UpdateOrderRequest(List<OrderItemInput>? Items);
public record UpdateOrderStatusRequest(string Status);
public record OrderItemInput(int ProductId, int Quantity);