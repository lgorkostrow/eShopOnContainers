namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingCouponValidationIntegrationEvent(int OrderId, string CouponCode, string BuyerName, string OrderStatus) : IntegrationEvent;