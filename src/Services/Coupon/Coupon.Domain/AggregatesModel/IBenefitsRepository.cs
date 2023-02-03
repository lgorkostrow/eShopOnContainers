namespace Coupon.Domain.AggregatesModel;

public interface IBenefitsRepository
{
    Task<Benefits?> FindByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task CreateAsync(Benefits benefits, CancellationToken cancellationToken);
    Task UpdateAsync(Benefits benefits, CancellationToken cancellationToken);
}