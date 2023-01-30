using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Coupon.Application.IntegrationEvents;

public record OrderCouponConfirmedIntegrationEvent(int OrderId, decimal Discount) : IntegrationEvent;