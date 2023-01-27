namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.DomainEventHandlers.OrderDiscount;

public class OrderStatusChangedToAwaitingCouponValidationDomainEventHandler
    : INotificationHandler<OrderStatusChangedToAwaitingCouponValidationDomainEvent>
{
    private readonly ILoggerFactory _logger;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public OrderStatusChangedToAwaitingCouponValidationDomainEventHandler(
        ILoggerFactory logger,
        IOrderingIntegrationEventService orderingIntegrationEventService    
    )
    {
        _logger = logger;
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderStatusChangedToAwaitingCouponValidationDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.CreateLogger<OrderStatusChangedToAwaitingCouponValidationDomainEvent>()
            .LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
                notification.OrderId, nameof(OrderStatus.AwaitingCouponValidation), OrderStatus.AwaitingCouponValidation.Id);

        var integrationEvent = new OrderStatusChangedToAwaitingCouponValidationIntegrationEvent(
            notification.OrderId,
            notification.CouponCode
        );
        await _orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}