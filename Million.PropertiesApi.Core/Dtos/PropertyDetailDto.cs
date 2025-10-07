
namespace Million.PropertiesApi.Core.Dtos
{
    public class PropertyDetailsDto
    {
        public PropertyDto Property { get; set; }

        public OwnerDto Owner { get; set; }

        public List<PropertyImageDto> Images { get; set; } = new();

        public List<PropertyTraceDto> Traces { get; set; } = new();
    }
}
