using MongoDB.Driver;
using Million.PropertiesApi.Core.Models;
using Million.PropertiesApi.Infrastructure.Interfaces.Data;
using Microsoft.Extensions.Configuration;

namespace Million.PropertiesApi.Infrastructure.Data
{
    public class MongoContext : IMongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(IMongoDatabase database)
        {
            _database = database;
        }

        public IMongoCollection<Property> Properties => _database.GetCollection<Property>("property");
        public IMongoCollection<Owner> Owners => _database.GetCollection<Owner>("owner");
        public IMongoCollection<PropertyImage> PropertyImages => _database.GetCollection<PropertyImage>("propertyImage");
        public IMongoCollection<PropertyTrace> PropertyTraces => _database.GetCollection<PropertyTrace>("propertyTrace");
    }
}
