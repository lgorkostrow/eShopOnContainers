namespace Basket.API.Model;

public interface IBasketCheckoutRepository
{
    Task<BasketCheckoutStoreModel> GetBasketCheckoutAsync(Guid id);
    Task<Guid> SaveBasketCheckoutAsync(BasketCheckoutStoreModel basketCheckout);
    Task<bool> DeleteBasketCheckoutAsync(Guid id);
}