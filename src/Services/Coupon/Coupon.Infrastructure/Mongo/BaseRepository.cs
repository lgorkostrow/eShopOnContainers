using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Coupon.Infrastructure.Mongo;

public abstract class BaseRepository<T>
{
    protected readonly IMongoDatabase MongoDatabase;
    
    protected BaseRepository(IOptions<MongoConfiguration> configuration)
    {
        var mongoClient = new MongoClient(
            configuration.Value.ConnectionString);
        
        MongoDatabase = mongoClient.GetDatabase(
            configuration.Value.CouponMongoDatabase);
    }

    protected abstract IMongoCollection<T> GetCollection();

    public async Task CreateOneAsync(T entity) =>
        await GetCollection().InsertOneAsync(entity);
    
    public async Task CreateManyAsync(IEnumerable<T> entities) =>
        await GetCollection().InsertManyAsync(entities);
}