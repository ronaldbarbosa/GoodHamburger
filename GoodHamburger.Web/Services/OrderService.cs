using System.Net.Http.Json;
using GoodHamburger.Core.Common;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public class OrderService(HttpClient client)
{
    private readonly HttpClient _client = client;
    private const string OrdersPath = "/api/orders";

    public async Task<PaginatedList<OrderResponse>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var response = await _client.GetFromJsonAsync<PaginatedList<OrderResponse>>(
            $"{OrdersPath}?pageNumber={pageNumber}&pageSize={pageSize}");
        return response ?? new PaginatedList<OrderResponse>();
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
