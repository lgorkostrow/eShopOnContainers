namespace Basket.API.Model;

public class BasketCheckoutStoreModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public BasketCheckout BasketCheckout { get; set; }
    public CustomerBasket Basket { get; set; }
}