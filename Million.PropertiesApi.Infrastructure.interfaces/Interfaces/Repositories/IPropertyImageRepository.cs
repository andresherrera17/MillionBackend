using Million.PropertiesApi.Core.Models;

namespace Million.PropertiesApi.Infraestructure.Interfaces
{

    public interface IPropertyImageRepository
    {
        public Task<List<PropertyImage>> GetByPropertyIdAsync(string propertyId);

        public Task InsertAsync(PropertyImage img);

        public Task InsertManyAsync(List<PropertyImage> entities, CancellationToken ct = default);
    }
}
