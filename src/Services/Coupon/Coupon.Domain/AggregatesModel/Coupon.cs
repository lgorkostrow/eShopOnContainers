using Coupon.Shared;

namespace Coupon.Domain.AggregatesModel;

public class Coupon : BaseEntity
{
    public Guid Id { get; set; }
    public string Code { get; private set; }
    public int Discount { get; private set; }
    public bool Consumed { get; private set; }
    public int? OrderId { get; private set; }

    public Coupon(string code, int discount)
    {
        Id = Guid.NewGuid();
        Code = code;
        Discount = discount;
    }

    public void UseForOrder(int orderId)
    {
        OrderId = orderId;
        Consumed = true;
    }
}