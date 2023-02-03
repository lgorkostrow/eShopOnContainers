using Coupon.Shared;
using Coupon.Shared.Exceptions;

namespace Coupon.Domain.AggregatesModel;

public class Benefits : BaseEntity
{
    public const int CashbackPercent = 5;
    
    public Guid Id { get; private set; }
    public int UserId { get; private set; }
    public decimal BonusPoints { get; private set; }

    public Benefits(Guid id, int userId, decimal bonusPoints)
    {
        Id = id;
        UserId = userId;
        BonusPoints = bonusPoints;
    }

    public void IncreaseBonusPoints(decimal amount)
    {
        BonusPoints += amount;
    }

    public void DecreaseBonusPoints(decimal amount)
    {
        if (amount > BonusPoints)
        {
            throw new DomainException("There are not enough bonus points to pay with");
        }

        BonusPoints -= amount;
    }
}