
namespace Million.PropertiesApi.Core.Dtos
{
    public class PropertyDto
    {
        public string IdProperty { get; set; } = string.Empty;
        public string IdOwner { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Image { get; set; } = string.Empty;
        public string CodeInternal { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}
