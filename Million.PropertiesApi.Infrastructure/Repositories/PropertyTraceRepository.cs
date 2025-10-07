using Million.PropertiesApi.Infraestructure.Interfaces;
using Million.PropertiesApi.Core.Models;
using MongoDB.Driver;

namespace Million.PropertiesApi.Infrastructure.Repositories
{
    public class PropertyTraceRepository : IPropertyTraceRepository
    {
        private readonly IMongoCollection<PropertyTrace> _traces;

        public PropertyTraceRepository(IMongoDatabase db)
        {
            _traces = db.GetCollection<PropertyTrace>("propertyTraces");
        }

        public Task<List<PropertyTrace>> GetByPropertyIdAsync(string propertyId) =>
            _traces.Find(t => t.IdProperty == propertyId).ToListAsync();

        public Task InsertAsync(PropertyTrace trace) => _traces.InsertOneAsync(trace);
    }
}
