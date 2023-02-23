using Coupon.Shared;

namespace Coupon.Domain.AggregatesModel;

public class Coupon : BaseEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public decimal Discount { get; private set; }
    public bool Consumed { get; private set; }
    public int? OrderId { get; private set; }
    public bool Personal { get; private set; }
    public Guid IdentityGuid { get; private set; }

    public Coupon(string code, decimal discount)
    {
        Id = Guid.NewGuid();
        Code = code;
        Discount = discount;
    }
    
    public Coupon(string code, decimal discount, Guid userId)
    {
        Id = Guid.NewGuid();
        Code = code;
        Discount = discount;
        Personal = true;
        IdentityGuid = userId;
    }

    public void Consume(int orderId)
    {
        OrderId = orderId;
        Consumed = true;
    }
}