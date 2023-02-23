namespace Basket.API.IntegrationEvents.Events;

public record BasketCheckoutWithBonusPaymentReceivedIntegrationEvent(
    string UserId, 
    string UserName,
    Guid BasketCheckoutId,
    decimal BonusPointsAmount
) : IntegrationEvent;