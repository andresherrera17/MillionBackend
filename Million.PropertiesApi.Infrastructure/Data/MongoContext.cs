using MongoDB.Driver;
using Million.PropertiesApi.Core.Models;
using Million.PropertiesApi.Infrastructure.Interfaces.Data;

namespace Million.PropertiesApi.Infrastructure.Data
{
    public class MongoContext : IMongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(IMongoClient client, string databaseName)
        {
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Owner> Owners => _database.GetCollection<Owner>("owners");
        public IMongoCollection<Property> Properties => _database.GetCollection<Property>("property");
        public IMongoCollection<PropertyImage> PropertyImages => _database.GetCollection<PropertyImage>("propertyImages");
        public IMongoCollection<PropertyTrace> PropertyTraces => _database.GetCollection<PropertyTrace>("propertyTraces");
    }
}
