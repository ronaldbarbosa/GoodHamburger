using System.Net.Http.Json;
using GoodHamburger.Core.Common;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public class OrderItemService(HttpClient client)
{
    private readonly HttpClient _client = client;
    private const string OrderItemsPath = "/api/order-items";

    public async Task<PaginatedList<OrderItemResponse>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var response = await _client.GetFromJsonAsync<PaginatedList<OrderItemResponse>>(
            $"{OrderItemsPath}?pageNumber={pageNumber}&pageSize={pageSize}");
        return response ?? new PaginatedList<OrderItemResponse>();
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

    public async Task<OrderItemResponse?> CreateAsync(CreateOrderItemRequest request)
    {
        var response = await _client.PostAsJsonAsync(OrderItemsPath, request);
        return await response.Content.ReadFromJsonAsync<OrderItemResponse>();
    }

    public async Task<OrderItemResponse?> UpdateAsync(int id, UpdateOrderItemRequest request)
    {
        var response = await _client.PutAsJsonAsync($"{OrderItemsPath}/{id}", request);
        return await response.Content.ReadFromJsonAsync<OrderItemResponse>();
    }

    public async Task DeleteAsync(int id)
    {
        await _client.DeleteAsync($"{OrderItemsPath}/{id}");
    }
}
