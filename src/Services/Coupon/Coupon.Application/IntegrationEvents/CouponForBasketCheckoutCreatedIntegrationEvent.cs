using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Coupon.Application.IntegrationEvents;

public record CouponForBasketCheckoutCreatedIntegrationEvent(
    string UserId, 
    string UserName, 
    Guid BasketCheckoutId,
    string CouponCode,
    decimal Discount
) : IntegrationEvent;