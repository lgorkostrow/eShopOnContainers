using Coupon.Application.Features.LoyaltySystem.Commands;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.Logging;

namespace Coupon.Application.IntegrationEvents;

public record OrderStatusChangedToPaidIntegrationEvent(
    int OrderId,
    string OrderStatus,
    string BuyerId,
    string BuyerName,
    decimal TotalPrice
) : IntegrationEvent;

public class OrderStatusChangedToPaidIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderStatusChangedToPaidIntegrationEventHandler> _logger;

    public OrderStatusChangedToPaidIntegrationEventHandler(
        IMediator mediator,
        ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger
    )
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
    {
        _logger.LogInformation("----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

        var command = new CalculateAndIncreaseBonusPointsCommand()
        {
            UserId = @event.BuyerId,
            OrderTotalPrice = @event.TotalPrice,
        };

        return _mediator.Send(command);
    }
}