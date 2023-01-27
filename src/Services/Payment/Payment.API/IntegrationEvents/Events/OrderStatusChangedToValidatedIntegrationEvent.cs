namespace Microsoft.eShopOnContainers.Payment.API.IntegrationEvents.Events;
    
public record OrderStatusChangedToValidatedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderStatusChangedToValidatedIntegrationEvent(int orderId)
        => OrderId = orderId;
}
