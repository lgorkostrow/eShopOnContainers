using Coupon.Shared;

namespace Coupon.Domain.AggregatesModel;

public class Coupon : BaseEntity
{
    public Guid Id { get; set; }
    public string Code { get; private set; }
    public decimal Discount { get; private set; }
    public bool Consumed { get; private set; }
    public int? OrderId { get; private set; }

    public Coupon(string code, decimal discount)
    {
        Id = Guid.NewGuid();
        Code = code;
        Discount = discount;
    }

    public void Consume(int orderId)
    {
        OrderId = orderId;
        Consumed = true;
    }
}