namespace Basket.API.IntegrationEvents.EventHandling;

public class CouponForBasketCheckoutCreatedIntegrationEventHandler : IIntegrationEventHandler<CouponForBasketCheckoutCreatedIntegrationEvent>
{
    private readonly ILogger<CouponForBasketCheckoutCreatedIntegrationEventHandler> _logger;
    private readonly IBasketCheckoutService _basketCheckoutService;

    public CouponForBasketCheckoutCreatedIntegrationEventHandler(
        ILogger<CouponForBasketCheckoutCreatedIntegrationEventHandler> logger,
        IBasketCheckoutService basketCheckoutService
    )
    {
        _logger = logger;
        _basketCheckoutService = basketCheckoutService;
    }

    public async Task Handle(CouponForBasketCheckoutCreatedIntegrationEvent @event)
    {
        _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

        await _basketCheckoutService.AttachCouponToBasketCheckoutAndAccept(@event.BasketCheckoutId, @event.CouponCode, @event.Discount);
    }
}