using Coupon.Application.Features.Coupon.Commands;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.Logging;

namespace Coupon.Application.IntegrationEvents;

public record OrderStatusChangedToAwaitingCouponValidationIntegrationEvent(int OrderId, string CouponCode) : IntegrationEvent;

public class OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler 
    : IIntegrationEventHandler<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler> _logger;

    public OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler(
        IMediator mediator,
        ILogger<OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler> logger
    )
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(OrderStatusChangedToAwaitingCouponValidationIntegrationEvent @event)
    {
        _logger.LogInformation("----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
        
        var command = new ApplyCouponForOrderCommand()
        {
            OrderId = @event.OrderId,
            CouponCode = @event.CouponCode,
        };
        
        await _mediator.Send(command);
    }
}