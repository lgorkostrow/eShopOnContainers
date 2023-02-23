namespace Coupon.Domain.AggregatesModel;

public interface ICouponRepository
{
    Task<Coupon?> FindByCodeAsync(string code, CancellationToken cancellationToken);
    Task CreateAsync(Coupon coupon, CancellationToken cancellationToken);
    Task UpdateAsync(Coupon coupon, CancellationToken cancellationToken);
}