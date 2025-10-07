
using Microsoft.AspNetCore.Http;

namespace Million.PropertiesApi.Core.Dtos
{
    public class PropertyDetailsImagesDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CodeInternal { get; set; } = string.Empty;
        public int Year { get; set; }

        public string OwnerName { get; set; } = string.Empty;
        public string OwnerAddress { get; set; } = string.Empty;
        public DateTime OwnerBirthday { get; set; }

        public decimal Tax { get; set; }

        public List<IFormFile> Files { get; set; }

        public IFormFile? photoOwner { get; set; }
    }
}
