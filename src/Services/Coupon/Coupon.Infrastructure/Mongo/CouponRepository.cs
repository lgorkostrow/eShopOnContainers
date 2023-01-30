using Coupon.Domain.AggregatesModel;
using MongoDB.Driver;

namespace Coupon.Infrastructure.Mongo;

public class CouponRepository : ICouponRepository
{
    private readonly CouponContext _context;
    public CouponRepository(CouponContext context)
    {
        _context = context;
    }

    public Task<Domain.AggregatesModel.Coupon?> FindByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return _context.Coupons
            .Find(x => x.Code == code)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task Update(Domain.AggregatesModel.Coupon coupon, CancellationToken cancellationToken)
    {
        var filter = Builders<Domain.AggregatesModel.Coupon>.Filter.Eq("Id", coupon.Id);
        
        return _context.Coupons.ReplaceOneAsync(filter, coupon, cancellationToken: cancellationToken);
    }
}