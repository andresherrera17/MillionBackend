using Million.PropertiesApi.Core.Dtos;
using Million.PropertiesApi.Core.Models;

namespace Million.PropertiesApi.Business.Interfaces
{
    public interface IPropertyService
    {
        Task<PagedResult<PropertyWithOwnerImageDto>> GetPropertiesAsync(PropertyFilter filter, CancellationToken ct = default);
        Task<PropertyDetailsDto> GetByIdAsync(string id, CancellationToken ct = default);
        Task AddAsync(PropertyDetailsImageDto property, CancellationToken ct = default);
    }
}
