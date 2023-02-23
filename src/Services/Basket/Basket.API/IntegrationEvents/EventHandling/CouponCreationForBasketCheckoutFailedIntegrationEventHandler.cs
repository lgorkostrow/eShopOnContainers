namespace Basket.API.IntegrationEvents.EventHandling;

public class CouponCreationForBasketCheckoutFailedIntegrationEventHandler : IIntegrationEventHandler<CouponCreationForBasketCheckoutFailedIntegrationEvent>
{
    private readonly ILogger<CouponCreationForBasketCheckoutFailedIntegrationEventHandler> _logger;
    private readonly IBasketCheckoutRepository _basketCheckoutRepository;

    public CouponCreationForBasketCheckoutFailedIntegrationEventHandler(
        ILogger<CouponCreationForBasketCheckoutFailedIntegrationEventHandler> logger,
        IBasketCheckoutRepository basketCheckoutRepository
    )
    {
        _logger = logger;
        _basketCheckoutRepository = basketCheckoutRepository;
    }

    public async Task Handle(CouponCreationForBasketCheckoutFailedIntegrationEvent @event)
    {
        _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

        await _basketCheckoutRepository.DeleteBasketCheckoutAsync(@event.BasketCheckoutId);
        
        //TODO: notify user with notification 
    }
}