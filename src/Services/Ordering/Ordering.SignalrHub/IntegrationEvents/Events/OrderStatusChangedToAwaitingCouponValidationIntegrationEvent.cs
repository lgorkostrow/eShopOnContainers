namespace Ordering.SignalrHub.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingCouponValidationIntegrationEvent(int OrderId, string CouponCode, string BuyerName, string OrderStatus) : IntegrationEvent;