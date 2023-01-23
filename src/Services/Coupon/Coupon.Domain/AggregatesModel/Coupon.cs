using Coupon.Shared;

namespace Coupon.Domain.AggregatesModel;

public class Coupon : BaseEntity
{
    public int Id { get; private set; }
    public string Code { get; }
    public int Discount { get; }
    public bool Consumed { get; private set; } = false;
    public int? OrderId { get; private set; }

    public Coupon(int id, string code, int discount)
    {
        Id = id;
        Code = code;
        Discount = discount;
    }

    public void UseForOrder(int orderId)
    {
        OrderId = orderId;
        Consumed = true;
    }
}