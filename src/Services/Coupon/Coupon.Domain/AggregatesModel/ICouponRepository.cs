namespace Coupon.Domain.AggregatesModel;

public interface ICouponRepository
{
    Task<Coupon?> FindByCodeAsync(string code, CancellationToken cancellationToken);
    Task Update(Coupon coupon, CancellationToken cancellationToken);
}