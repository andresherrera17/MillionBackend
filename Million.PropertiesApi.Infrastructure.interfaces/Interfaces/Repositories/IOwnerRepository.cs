

using Million.PropertiesApi.Core.Models;

namespace Million.PropertiesApi.Infraestructure.Interfaces
{
    public interface IOwnerRepository
    {
        public Task<List<Owner>> GetAllAsync();
        public Task<Owner> GetByIdAsync(string id);
        public Task InsertAsync(Owner owner);
    }
}
