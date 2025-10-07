
namespace Million.PropertiesApi.Core.Dtos
{
    public class PropertyWithOwnerImageDto
    {
        public string IdProperty { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Year { get; set; }
        public string? OwnerName { get; set; }
        public string? FirstImage { get; set; }
    }
}
