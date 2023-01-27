namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingCouponValidationIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public string CouponCode { get; }

    public OrderStatusChangedToAwaitingCouponValidationIntegrationEvent(int orderId, string couponCode)
    {
        OrderId = orderId;
        CouponCode = couponCode;
    }
}