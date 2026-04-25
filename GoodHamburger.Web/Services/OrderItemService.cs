using System.Net.Http.Json;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public class OrderItemService(HttpClient client)
{
    private readonly HttpClient _client = client;

    public async Task<List<OrderItemResponse>> GetByOrderAsync(int orderId)
    {
        var response = await _client.GetFromJsonAsync<List<OrderItemResponse>>(
            $"/api/orders/{orderId}/items");
        return response ?? [];
    }
}
