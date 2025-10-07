

namespace Million.PropertiesApi.Core.Dtos
{
    public record PropertyFilter(string? Name, string? Address, decimal? MinPrice, decimal? MaxPrice, int Page, int PageSize);
}
