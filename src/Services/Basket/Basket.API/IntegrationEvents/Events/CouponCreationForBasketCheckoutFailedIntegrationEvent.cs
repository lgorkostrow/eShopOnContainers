namespace Basket.API.IntegrationEvents.Events;

public record CouponCreationForBasketCheckoutFailedIntegrationEvent(
    string UserId, 
    string UserName, 
    Guid BasketCheckoutId
) : IntegrationEvent;