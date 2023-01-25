using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Discount : ValueObject
{
    public string DiscountCode { get; private set; }
    public decimal Amount { get; private set; }
    public bool DiscountConfirmed { get; private set; }

    public Discount(string discountCode, decimal amount)
    {
        DiscountCode = discountCode;
        Amount = amount;
        DiscountConfirmed = false;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DiscountCode;
        yield return Amount;
        yield return DiscountConfirmed;
    }
}