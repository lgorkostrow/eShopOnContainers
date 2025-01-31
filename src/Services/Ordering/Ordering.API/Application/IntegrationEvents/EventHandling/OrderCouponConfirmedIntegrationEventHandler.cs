namespace Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderCouponConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderCouponConfirmedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCouponConfirmedIntegrationEventHandler> _logger;

    public OrderCouponConfirmedIntegrationEventHandler(
        IMediator mediator,
        ILogger<OrderCouponConfirmedIntegrationEventHandler> logger    
    )
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(OrderCouponConfirmedIntegrationEvent @event)
    {
        using var _ = LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}");
        
        _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

        var command = new ConfirmCouponCodeForOrderCommand(@event.OrderId, @event.Discount); 
        
        _logger.LogInformation(
            "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            command.GetGenericTypeName(),
            nameof(command.OrderId),
            command.OrderId,
            command);

        await _mediator.Send(command);
    }
}