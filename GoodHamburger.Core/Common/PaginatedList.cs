using System.Text.Json.Serialization;

namespace GoodHamburger.Core.Common;

[Serializable]
public class PaginatedList<T> where T : class
{
    [JsonPropertyName("items")]
    public IList<T> Items { get; set; } = [];

    [JsonPropertyName("totalItemCount")]
    public int TotalItemCount { get; set; }

    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage => PageNumber > 1;

    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedList() { }

    public PaginatedList(List<T> items, int totalItemCount, int pageNumber, int pageSize)
    {
        TotalItemCount = totalItemCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalItemCount / (double)pageSize);
        foreach (var item in items)
            Items.Add(item);
    }
}
