namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderCouponConfirmedIntegrationEvent(int OrderId, decimal Discount) : IntegrationEvent;