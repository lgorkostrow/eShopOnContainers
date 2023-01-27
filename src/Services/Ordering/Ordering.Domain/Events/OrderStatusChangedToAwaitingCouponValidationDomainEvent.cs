namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.Events;

public class OrderStatusChangedToAwaitingCouponValidationDomainEvent : INotification
{
    public int OrderId { get; }
    public string CouponCode { get; }

    public OrderStatusChangedToAwaitingCouponValidationDomainEvent(int orderId, string couponCode)
    {
        OrderId = orderId;
        CouponCode = couponCode;
    }
}