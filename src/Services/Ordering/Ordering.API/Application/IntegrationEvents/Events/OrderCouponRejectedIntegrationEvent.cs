namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderCouponRejectedIntegrationEvent(int OrderId) : IntegrationEvent;