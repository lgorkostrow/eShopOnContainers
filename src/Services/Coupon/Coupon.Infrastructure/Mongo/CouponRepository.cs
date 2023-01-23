using Coupon.Domain.AggregatesModel;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Coupon.Infrastructure.Mongo;

public class CouponRepository : BaseRepository<Domain.AggregatesModel.Coupon>, ICouponRepository
{
    private const string CouponCollection = "CouponCollection";
    
    public CouponRepository(IOptions<MongoConfiguration> configuration) : base(configuration)
    {
    }

    protected override IMongoCollection<Domain.AggregatesModel.Coupon> GetCollection()
    {
        return MongoDatabase.GetCollection<Domain.AggregatesModel.Coupon>(CouponCollection);
    }

    public Task<Domain.AggregatesModel.Coupon?> FindByCodeAsync(string code)
    {
        return GetCollection()
            .Find(x => x.Code == code)
            .FirstOrDefaultAsync();
    }
}