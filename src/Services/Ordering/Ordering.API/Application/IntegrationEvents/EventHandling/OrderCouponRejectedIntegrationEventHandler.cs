namespace Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderCouponRejectedIntegrationEventHandler : IIntegrationEventHandler<OrderCouponRejectedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCouponConfirmedIntegrationEventHandler> _logger;

    public OrderCouponRejectedIntegrationEventHandler(
        IMediator mediator,
        ILogger<OrderCouponConfirmedIntegrationEventHandler> logger    
    )
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(OrderCouponRejectedIntegrationEvent @event)
    {
        using var _ = LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}");
        
        _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

        var command = new RemoveDiscountFromOrderCommand(@event.OrderId, @event.CouponCode); 
        
        _logger.LogInformation(
            "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            command.GetGenericTypeName(),
            nameof(command.OrderId),
            command.OrderId,
            command);

        await _mediator.Send(command);
    }
}