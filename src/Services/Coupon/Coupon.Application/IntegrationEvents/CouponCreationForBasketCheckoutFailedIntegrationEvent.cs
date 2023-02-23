using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Coupon.Application.IntegrationEvents;

public record CouponCreationForBasketCheckoutFailedIntegrationEvent(
    string UserId, 
    string UserName, 
    Guid BasketCheckoutId
) : IntegrationEvent;