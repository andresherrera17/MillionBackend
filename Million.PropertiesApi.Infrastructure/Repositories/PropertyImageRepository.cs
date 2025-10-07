using Million.PropertiesApi.Infraestructure.Interfaces;
using Million.PropertiesApi.Core.Models;
using MongoDB.Driver;

namespace Million.PropertiesApi.Infrastructure.Repositories
{
    public class PropertyImageRepository : IPropertyImageRepository
    {
        private readonly IMongoCollection<PropertyImage> _images;

        public PropertyImageRepository(IMongoDatabase db)
        {
            _images = db.GetCollection<PropertyImage>("propertyImages");
        }

        public Task<List<PropertyImage>> GetByPropertyIdAsync(string propertyId) =>
            _images.Find(i => i.IdProperty == propertyId && i.Enabled).ToListAsync();

        public Task InsertAsync(PropertyImage img) => _images.InsertOneAsync(img);

        public  Task InsertManyAsync(List<PropertyImage> entities, CancellationToken ct = default) {
            
            return _images.InsertManyAsync(entities, cancellationToken: ct);
        }
    }
}
