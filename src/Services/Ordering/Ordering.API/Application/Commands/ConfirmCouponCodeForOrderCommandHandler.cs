namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

public class ConfirmCouponCodeForOrderCommandHandler : IRequestHandler<ConfirmCouponCodeForOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public ConfirmCouponCodeForOrderCommandHandler(
        IOrderRepository orderRepository    
    )
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(ConfirmCouponCodeForOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetAsync(request.OrderId);
        if (order is null)
        {
            return false;
        }
        
        order.ConfirmDiscount(request.Discount);
        order.SetValidatedStatus();
        
        _orderRepository.Update(order);

        return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}