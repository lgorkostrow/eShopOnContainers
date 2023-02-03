namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public string OrderStatus { get; }
    public int BuyerId { get; }
    public string BuyerName { get; }
    public decimal TotalPrice { get; }
    public IEnumerable<OrderStockItem> OrderStockItems { get; }

    public OrderStatusChangedToPaidIntegrationEvent(int orderId,
        string orderStatus,
        int buyerId,
        string buyerName,
        decimal totalPrice,
        IEnumerable<OrderStockItem> orderStockItems)
    {
        OrderId = orderId;
        OrderStockItems = orderStockItems;
        OrderStatus = orderStatus;
        BuyerId = buyerId;
        BuyerName = buyerName;
        TotalPrice = totalPrice;
    }
}

