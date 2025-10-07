using Million.PropertiesApi.Core.Dtos;
using Million.PropertiesApi.Core.Models;

namespace Million.PropertiesApi.Infraestructure.Interfaces
{
    public interface IPropertyRepository
    {
        Task<(IEnumerable<PropertyWithOwnerImageDto> Items, long Total)> GetAsync(PropertyFilter filter, CancellationToken ct = default);
        public Task<List<Property>> GetAllAsync();
        public Task<Property> GetByIdAsync(string id, CancellationToken ct = default);
        public Task InsertAsync(Property property, CancellationToken ct = default);
    }
}
