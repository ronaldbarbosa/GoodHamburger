using System.Net.Http.Json;
using GoodHamburger.Core.Common;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public class ProductService(HttpClient client)
{
    private readonly HttpClient _client = client;
    private const string ProductsPath = "/api/products";

    private async Task<PaginatedList<ProductResponse>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var response = await _client.GetFromJsonAsync<PaginatedList<ProductResponse>>(
            $"{ProductsPath}?pageNumber={pageNumber}&pageSize={pageSize}");
        return response ?? new PaginatedList<ProductResponse>();
    }

    public async Task<List<ProductResponse>> GetAllAsync()
    {
        var paged = await GetPagedAsync(1, 50);
        return paged.Items.ToList();
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        try
        {
            return await _client.GetFromJsonAsync<ProductResponse>($"{ProductsPath}/{id}");
        }
        catch
        {
            return null;
        }
    }
}

public class ProductCategoryService(HttpClient client)
{
    private readonly HttpClient _client = client;
    private const string CategoriesPath = "/api/product-categories";

    public async Task<List<ProductCategoryResponse>> GetAllAsync()
    {
        var response = await _client.GetFromJsonAsync<List<ProductCategoryResponse>>(CategoriesPath);
        return response ?? [];
    }
}
