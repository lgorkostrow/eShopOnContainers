namespace Basket.API.Model;

public class BasketCheckout
{
    public string City { get; set; }

    public string Street { get; set; }

    public string State { get; set; }

    public string Country { get; set; }

    public string ZipCode { get; set; }

    public string CardNumber { get; set; }

    public string CardHolderName { get; set; }

    public DateTime CardExpiration { get; set; }

    public string CardSecurityNumber { get; set; }

    public int CardTypeId { get; set; }

    public string Buyer { get; set; }

    public Guid RequestId { get; set; }
    
    public string CouponCode { get; set; }
    
    public decimal? Discount { get; set; }
    public bool PayWithBonusPoints { get; set; }
    public decimal? BonusPointsAmount { get; set; }
}
