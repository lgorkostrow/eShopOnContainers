namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

public class RemoveDiscountFromOrderCommand : IRequest<bool>
{
    public int OrderId { get; private set; }
    public string CouponCode { get; private set; }

    public RemoveDiscountFromOrderCommand(int orderId, string couponCode)
    {
        OrderId = orderId;
        CouponCode = couponCode;
    }
}