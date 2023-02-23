using Coupon.Application.Features.Coupon.Commands;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.Logging;

namespace Coupon.Application.IntegrationEvents;

public record BasketCheckoutWithBonusPaymentReceivedIntegrationEvent(string UserId, string UserName, Guid BasketCheckoutId, decimal BonusPointsAmount) : IntegrationEvent;

public class BasketCheckoutWithBonusPaymentReceivedIntegrationEventHandler : IIntegrationEventHandler<BasketCheckoutWithBonusPaymentReceivedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<BasketCheckoutWithBonusPaymentReceivedIntegrationEventHandler> _logger;

    public BasketCheckoutWithBonusPaymentReceivedIntegrationEventHandler(
        IMediator mediator,
        ILogger<BasketCheckoutWithBonusPaymentReceivedIntegrationEventHandler> logger
    )
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public Task Handle(BasketCheckoutWithBonusPaymentReceivedIntegrationEvent @event)
    {
        _logger.LogInformation("----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

        var command = new CreateCouponCodeFromBonusPointsCommand()
        {
            UserId = @event.UserId,
            UserName = @event.UserName,
            BasketCheckoutId = @event.BasketCheckoutId,
            BonusPointsAmount = @event.BonusPointsAmount,
        };

        return _mediator.Send(command);
    }
}