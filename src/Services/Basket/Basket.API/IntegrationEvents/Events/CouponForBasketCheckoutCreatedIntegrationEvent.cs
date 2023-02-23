namespace Basket.API.IntegrationEvents.Events;

public record CouponForBasketCheckoutCreatedIntegrationEvent(
    string UserId, 
    string UserName, 
    Guid BasketCheckoutId,
    string CouponCode,
    decimal Discount
) : IntegrationEvent;