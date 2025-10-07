using Million.PropertiesApi.Infraestructure.Interfaces;
using Million.PropertiesApi.Core.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Million.PropertiesApi.Core.Dtos;

namespace Million.PropertiesApi.Infrastructure.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly IMongoCollection<Property> _property;

        public PropertyRepository(IMongoDatabase db)
        {
            _property = db.GetCollection<Property>("property");
        }

        public async Task<(IEnumerable<PropertyWithOwnerImageDto> Items, long Total)> GetAsync(PropertyFilter f, CancellationToken ct = default)
        {
            var filterBuilder = Builders<Property>.Filter;
            var filters = new List<FilterDefinition<Property>>();

            AddFilterIfNotEmpty(filters, f.Name,
                value => filterBuilder.Regex(p => p.Name, new BsonRegularExpression(value, "i")));

            AddFilterIfNotEmpty(filters, f.Address,
                value => filterBuilder.Regex(p => p.Address, new BsonRegularExpression(value, "i")));

            AddFilterIfHasValue(filters, f.MinPrice,
                value => filterBuilder.Gte(p => p.Price, value));

            AddFilterIfHasValue(filters, f.MaxPrice,
                value => filterBuilder.Lte(p => p.Price, value));

            var filter = filters.Count > 0
                ? filterBuilder.And(filters)
                : filterBuilder.Empty;

            var countTask = _property.CountDocumentsAsync(filter, cancellationToken: ct);

            var pipeline = _property.Aggregate()
                .Match(filter)
                .Lookup("owners", "IdOwner", "_id", "OwnerData")
                .Lookup("propertyImages", "_id", "IdProperty", "Images")
                .Project(new BsonDocument
                {
                    { "_id", "$_id" },
                    { "Name", "$Name" },
                    { "Address", "$Address" },
                    { "Price", "$Price" },
                    { "Year", "$Year" },
                    { "OwnerName", new BsonDocument("$arrayElemAt", new BsonArray { "$OwnerData.Name", 0 }) },
                    { "FirstImage", new BsonDocument("$arrayElemAt", new BsonArray { "$Images.File", 0 }) }
                })
                .Skip((f.Page - 1) * f.PageSize)
                .Limit(f.PageSize);

            var itemsTask = pipeline.ToListAsync(ct);

            await Task.WhenAll(countTask, itemsTask);

            var items = itemsTask.Result.Select(b => new PropertyWithOwnerImageDto
            {
                IdProperty = b["_id"].AsObjectId.ToString(),
                Name = b["Name"].AsString,
                Address = b["Address"].AsString,
                Price = b["Price"].ToDecimal(),
                Year = b["Year"].ToInt32(),
                OwnerName = b.GetValue("OwnerName", "").AsString,
                FirstImage = b.GetValue("FirstImage", "").AsString
            }).ToList();

            return (items, countTask.Result);
        }

        private static void AddFilterIfNotEmpty(List<FilterDefinition<Property>> filters, string? value, Func<string, FilterDefinition<Property>> createFilter)
        {
            if (!string.IsNullOrWhiteSpace(value))
                filters.Add(createFilter(value));
        }

        private static void AddFilterIfHasValue<T>(List<FilterDefinition<Property>> filters, T? value, Func<T, FilterDefinition<Property>> createFilter) where T : struct
        {
            if (value.HasValue)
                filters.Add(createFilter(value.Value));
        }

        public Task<List<Property>> GetAllAsync() => _property.Find(_ => true).ToListAsync();

        public Task<Property> GetByIdAsync(string id, CancellationToken ct = default) {
            return _property.Find(p => p.IdProperty == id).FirstOrDefaultAsync();
        }

        public Task InsertAsync(Property property, CancellationToken ct = default) => _property.InsertOneAsync(property);


    }
}
