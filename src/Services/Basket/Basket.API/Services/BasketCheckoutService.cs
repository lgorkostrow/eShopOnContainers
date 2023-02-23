namespace Microsoft.eShopOnContainers.Services.Basket.API.Services;

public class BasketCheckoutService : IBasketCheckoutService
{
    private readonly ILogger<BasketCheckoutService> _logger;
    private readonly IBasketRepository _basketRepository;
    private readonly IBasketCheckoutRepository _basketCheckoutRepository;
    private readonly IEventBus _eventBus;

    public BasketCheckoutService(
        ILogger<BasketCheckoutService> logger,
        IBasketRepository basketRepository,
        IBasketCheckoutRepository basketCheckoutRepository,
        IEventBus eventBus
    )
    {
        _logger = logger;
        _basketRepository = basketRepository;
        _basketCheckoutRepository = basketCheckoutRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> AcceptBasketCheckout(string userId, string userName, BasketCheckout basketCheckout)
    {
        var basket = await _basketRepository.GetBasketAsync(userId);
        if (basket == null)
        {
            return false;
        }

        if (basketCheckout.PayWithBonusPoints && basketCheckout.BonusPointsAmount is not null)
        {
            await AcceptBasketCheckoutWithBonusPoints(userId, userName, basketCheckout, basket);
            
            return true;
        }
        
        PublishUserCheckoutAcceptedIntegrationEvent(userId, userName, basketCheckout, basket);

        return true;
    }

    public async Task<bool> AttachCouponToBasketCheckoutAndAccept(Guid basketCheckoutId, string couponCode, decimal discount)
    {
        var data = await _basketCheckoutRepository.GetBasketCheckoutAsync(basketCheckoutId);

        data.BasketCheckout.CouponCode = couponCode;
        data.BasketCheckout.Discount = discount;
        
        await _basketCheckoutRepository.DeleteBasketCheckoutAsync(basketCheckoutId);
        
        PublishUserCheckoutAcceptedIntegrationEvent(
            data.UserId, 
            data.UserName, 
            data.BasketCheckout, 
            data.Basket
        );

        return true;
    }

    private async Task AcceptBasketCheckoutWithBonusPoints(string userId, string userName, BasketCheckout basketCheckout, CustomerBasket basket)
    {
        var basketCheckoutId = await _basketCheckoutRepository.SaveBasketCheckoutAsync(new BasketCheckoutStoreModel()
        {
            UserId = userId,
            UserName = userName,
            BasketCheckout = basketCheckout,
            Basket = basket,
        });
        
        var eventMessage = new BasketCheckoutWithBonusPaymentReceivedIntegrationEvent(
            userId,
            userName,
            basketCheckoutId,
            (decimal) basketCheckout.BonusPointsAmount
        );
        
        try
        {
            _eventBus.Publish(eventMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName}", eventMessage.Id, Program.AppName);

            throw;
        }
    }

    private void PublishUserCheckoutAcceptedIntegrationEvent(string userId, string userName, BasketCheckout basketCheckout, CustomerBasket basket)
    {
        var eventMessage = new UserCheckoutAcceptedIntegrationEvent(
            userId, 
            userName, 
            basketCheckout.City, 
            basketCheckout.Street,
            basketCheckout.State, 
            basketCheckout.Country, 
            basketCheckout.ZipCode, 
            basketCheckout.CardNumber,
            basketCheckout.CardHolderName,
            basketCheckout.CardExpiration,
            basketCheckout.CardSecurityNumber,
            basketCheckout.CardTypeId,
            basketCheckout.Buyer,
            basketCheckout.RequestId,
            basketCheckout.CouponCode,
            basketCheckout.Discount,
            basket
        );

        // Once basket is checkout, sends an integration event to
        // ordering.api to convert basket to order and proceeds with
        // order creation process
        try
        {
            _eventBus.Publish(eventMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName}", eventMessage.Id, Program.AppName);

            throw;
        }
    }
}