using Million.PropertiesApi.Core.Models;
using MongoDB.Driver;

namespace Million.PropertiesApi.Infrastructure.Interfaces.Data
{
    public interface IMongoContext
    {
        IMongoCollection<Property> Properties { get; }
        IMongoCollection<Owner> Owners { get; }
        IMongoCollection<PropertyImage> PropertyImages { get; }
        IMongoCollection<PropertyTrace> PropertyTraces { get; }
    }
}
