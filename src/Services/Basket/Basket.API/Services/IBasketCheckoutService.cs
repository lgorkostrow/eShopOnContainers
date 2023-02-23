namespace Microsoft.eShopOnContainers.Services.Basket.API.Services;

public interface IBasketCheckoutService
{
    Task<bool> AcceptBasketCheckout(string userId, string userName, BasketCheckout basketCheckout);

    Task<bool> AttachCouponToBasketCheckoutAndAccept(Guid basketCheckoutId, string couponCode, decimal discount);
}