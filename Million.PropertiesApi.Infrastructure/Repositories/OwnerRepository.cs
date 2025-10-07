using Million.PropertiesApi.Core.Models;
using Million.PropertiesApi.Infraestructure.Interfaces;
using MongoDB.Driver;

namespace Million.PropertiesApi.Infrastructure.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IMongoCollection<Owner> _owners;

        public OwnerRepository(IMongoDatabase db)
        {
            _owners = db.GetCollection<Owner>("owners");
        }

        public Task<List<Owner>> GetAllAsync() => _owners.Find(_ => true).ToListAsync();
        public Task<Owner> GetByIdAsync(string id) => _owners.Find(o => o.IdOwner == id).FirstOrDefaultAsync();
        public Task InsertAsync(Owner owner) => _owners.InsertOneAsync(owner);
    }
}
