using Million.PropertiesApi.Core.Models;

namespace Million.PropertiesApi.Infraestructure.Interfaces
{

    public interface IPropertyTraceRepository
    {
        public Task<List<PropertyTrace>> GetByPropertyIdAsync(string propertyId);

        public Task InsertAsync(PropertyTrace trace);
    }
}
