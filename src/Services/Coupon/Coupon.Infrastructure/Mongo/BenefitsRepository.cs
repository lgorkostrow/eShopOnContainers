using Coupon.Domain.AggregatesModel;
using MongoDB.Driver;

namespace Coupon.Infrastructure.Mongo;

public class BenefitsRepository : IBenefitsRepository
{
    private readonly CouponContext _context;
    
    public BenefitsRepository(CouponContext context)
    {
        _context = context;
    }
    
    public Task<Benefits?> FindByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return _context.Benefits
            .Find(x => x.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task CreateAsync(Benefits benefits, CancellationToken cancellationToken)
    {
        await _context.Benefits.InsertOneAsync(benefits, null, cancellationToken);
    }

    public async Task UpdateAsync(Benefits benefits, CancellationToken cancellationToken)
    {
        var filter = Builders<Benefits>.Filter.Eq("Id", benefits.Id);
        
        await _context.Benefits.ReplaceOneAsync(filter, benefits, cancellationToken: cancellationToken);
    }
}