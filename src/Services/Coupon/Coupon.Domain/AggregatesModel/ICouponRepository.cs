namespace Coupon.Domain.AggregatesModel;

public interface ICouponRepository
{
    Task<Coupon?> FindByCodeAsync(string code, CancellationToken cancellationToken);
}