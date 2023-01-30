namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

public class RemoveDiscountFromOrderCommandHandler : IRequestHandler<RemoveDiscountFromOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public RemoveDiscountFromOrderCommandHandler(
        IOrderRepository orderRepository    
    )
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<bool> Handle(RemoveDiscountFromOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetAsync(request.OrderId);
        if (order is null)
        {
            return false;
        }
        
        order.RemoveDiscount();
        order.SetValidatedStatus();
        
        _orderRepository.Update(order);

        return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}