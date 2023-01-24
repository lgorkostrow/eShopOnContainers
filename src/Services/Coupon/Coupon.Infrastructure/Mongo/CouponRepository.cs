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
}