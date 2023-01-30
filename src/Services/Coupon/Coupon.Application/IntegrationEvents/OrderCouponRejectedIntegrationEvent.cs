using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Coupon.Application.IntegrationEvents;

public record OrderCouponRejectedIntegrationEvent(int OrderId, string CouponCode) : IntegrationEvent;