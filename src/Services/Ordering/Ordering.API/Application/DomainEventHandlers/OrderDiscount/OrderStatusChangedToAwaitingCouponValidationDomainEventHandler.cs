namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.DomainEventHandlers.OrderDiscount;

public class OrderStatusChangedToAwaitingCouponValidationDomainEventHandler
    : INotificationHandler<OrderStatusChangedToAwaitingCouponValidationDomainEvent>
{
    private readonly ILoggerFactory _logger;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;

    public OrderStatusChangedToAwaitingCouponValidationDomainEventHandler(
        ILoggerFactory logger,
        IOrderingIntegrationEventService orderingIntegrationEventService,
        IOrderRepository orderRepository,
        IBuyerRepository buyerRepository
    )
    {
        _logger = logger;
        _orderingIntegrationEventService = orderingIntegrationEventService;
        _orderRepository = orderRepository;
        _buyerRepository = buyerRepository;
    }

    public async Task Handle(OrderStatusChangedToAwaitingCouponValidationDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.CreateLogger<OrderStatusChangedToAwaitingCouponValidationDomainEvent>()
            .LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
                notification.OrderId, nameof(OrderStatus.AwaitingCouponValidation), OrderStatus.AwaitingCouponValidation.Id);

        var order = await _orderRepository.GetAsync(notification.OrderId);
        var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());
        
        var integrationEvent = new OrderStatusChangedToAwaitingCouponValidationIntegrationEvent(
            notification.OrderId,
            notification.CouponCode,
            buyer.Name,
            order.OrderStatus.Name
        );
        await _orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}