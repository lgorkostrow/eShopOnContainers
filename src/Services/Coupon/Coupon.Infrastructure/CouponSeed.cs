using Coupon.Infrastructure.Mongo;

namespace Coupon.Infrastructure;

public class CouponSeed
{
    public async Task SeedAsync(CouponContext context)
    {
        if (await context.Coupons.EstimatedDocumentCountAsync() == 0)
        {
            var coupons = new List<Domain.AggregatesModel.Coupon>
            {
                new("DISC-10", 10),
                new("DISC-15", 15),
                new("DISC-20", 20),
                new("DISC-25", 25),
                new("DISC-30", 30),
            };

            await context.Coupons.InsertManyAsync(coupons);
        }
    }
}