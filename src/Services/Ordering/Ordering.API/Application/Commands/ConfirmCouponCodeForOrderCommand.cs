namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

public class ConfirmCouponCodeForOrderCommand : IRequest<bool>
{
    public int OrderId { get; private set; }
    public decimal Discount { get; private set; }

    public ConfirmCouponCodeForOrderCommand(int orderId, decimal discount)
    {
        OrderId = orderId;
        Discount = discount;
    }
}