using Coupon.Domain.AggregatesModel;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace Coupon.Infrastructure.Mongo;

public class CouponContext
{
    public IMongoCollection<Domain.AggregatesModel.Coupon> Coupons => _database.GetCollection<Domain.AggregatesModel.Coupon>("CouponCollection");
    public IMongoCollection<Benefits> Benefits => _database.GetCollection<Benefits>("BenefitsCollection");
    
    private readonly IMongoDatabase _database = null;

    public CouponContext(IOptions<MongoConfiguration> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);

        if (client is null)
        {
            throw new MongoConfigurationException("Cannot connect to the database. The connection string is not valid or the database is not accessible");
        }

        _database = client.GetDatabase(settings.Value.CouponMongoDatabase);
        
        ConfigureModels();
    }

    private void ConfigureModels()
    {
        BsonClassMap.RegisterClassMap<Domain.AggregatesModel.Coupon>(cm => 
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id).SetIdGenerator(CombGuidGenerator.Instance);
        });
        
        BsonClassMap.RegisterClassMap<Benefits>(cm => 
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id).SetIdGenerator(CombGuidGenerator.Instance);
        });
    }
}