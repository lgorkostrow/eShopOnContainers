﻿using Microsoft.eShopOnContainers.Services.Ordering.Domain.Events;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
    : Entity, IAggregateRoot
{
    // DDD Patterns comment
    // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
    // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
    private DateTime _orderDate;

    // Address is a Value Object pattern example persisted as EF Core 2.0 owned entity
    public Address Address { get; private set; }

    public int? GetBuyerId => _buyerId;
    private int? _buyerId;

    public OrderStatus OrderStatus { get; private set; }
    private int _orderStatusId;

    private string _description;
    
    public Discount Discount { get; private set; }

    // Draft orders have this set to true. Currently we don't check anywhere the draft status of an Order, but we could do it if needed
    private bool _isDraft;

    // DDD Patterns comment
    // Using a private collection field, better for DDD Aggregate's encapsulation
    // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
    // but only through the method OrderAggrergateRoot.AddOrderItem() which includes behaviour.
    private readonly List<OrderItem> _orderItems;
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    private int? _paymentMethodId;

    public static Order NewDraft()
    {
        var order = new Order();
        order._isDraft = true;
        return order;
    }

    protected Order()
    {
        _orderItems = new List<OrderItem>();
        _isDraft = false;
    }

    public Order(
        string userId, 
        string userName,
        Address address,
        int cardTypeId,
        string cardNumber,
        string cardSecurityNumber,
        string cardHolderName,
        DateTime cardExpiration,
        int? buyerId = null,
        int? paymentMethodId = null,
        Discount discount = null
    ) : this()
    {
        _buyerId = buyerId;
        _paymentMethodId = paymentMethodId;
        _orderStatusId = OrderStatus.Submitted.Id;
        _orderDate = DateTime.UtcNow;
        Address = address;
        Discount = discount;

        // Add the OrderStarterDomainEvent to the domain events collection 
        // to be raised/dispatched when comitting changes into the Database [ After DbContext.SaveChanges() ]
        AddOrderStartedDomainEvent(userId, userName, cardTypeId, cardNumber,
                                    cardSecurityNumber, cardHolderName, cardExpiration);
    }

    // DDD Patterns comment
    // This Order AggregateRoot's method "AddOrderitem()" should be the only way to add Items to the Order,
    // so any behavior (discounts, etc.) and validations are controlled by the AggregateRoot 
    // in order to maintain consistency between the whole Aggregate. 
    public void AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1)
    {
        var existingOrderForProduct = _orderItems.Where(o => o.ProductId == productId)
            .SingleOrDefault();

        if (existingOrderForProduct != null)
        {
            //if previous line exist modify it with higher discount  and units..

            if (discount > existingOrderForProduct.GetCurrentDiscount())
            {
                existingOrderForProduct.SetNewDiscount(discount);
            }

            existingOrderForProduct.AddUnits(units);
        }
        else
        {
            //add validated new order item

            var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
            _orderItems.Add(orderItem);
        }
    }

    public void SetPaymentId(int id)
    {
        _paymentMethodId = id;
    }

    public void SetBuyerId(int id)
    {
        _buyerId = id;
    }

    public void SetAwaitingStockValidationStatus()
    {
        if (_orderStatusId == OrderStatus.Submitted.Id)
        {
            AddDomainEvent(new OrderStatusChangedToAwaitingStockValidationDomainEvent(Id, _orderItems));
            _orderStatusId = OrderStatus.AwaitingStockValidation.Id;
        }
    }

    public void StockConfirmed()
    {
        if (_orderStatusId != OrderStatus.AwaitingStockValidation.Id)
        {
            return;
        }

        if (Discount is not null)
        {
            AddDomainEvent(new OrderStatusChangedToAwaitingCouponValidationDomainEvent(Id, Discount.DiscountCode));
            _orderStatusId = OrderStatus.AwaitingCouponValidation.Id;
            
            return;
        }
        
        SetValidatedStatus();
    }

    public void ConfirmDiscount(decimal discount)
    {
        if (Discount is not null && _orderStatusId == OrderStatus.AwaitingCouponValidation.Id)
        {
            Discount.ConfirmDiscount(discount);
        }
    }

    public void RemoveDiscount()
    {
        if (Discount is not null && _orderStatusId == OrderStatus.AwaitingCouponValidation.Id)
        {
            Discount = null;
        }
    }

    public decimal CalculatePriceWithDiscount()
    {
        decimal total = 0;
        _orderItems.ForEach(x => total += x.GetUnits() * x.GetUnitPrice());

        if (Discount is not null && Discount.DiscountConfirmed)
        {
            total = total > Discount.Amount
                ? total - Discount.Amount
                : 1;
        }
        
        return total;
    }

    public void SetValidatedStatus()
    {
        if (_orderStatusId != OrderStatus.AwaitingStockValidation.Id && _orderStatusId != OrderStatus.AwaitingCouponValidation.Id)
        {
            return;
        }

        AddDomainEvent(new OrderStatusChangedToValidatedDomainEvent(Id));

        _orderStatusId = OrderStatus.Validated.Id;
    }

    public void SetPaidStatus()
    {
        if (_orderStatusId == OrderStatus.Validated.Id)
        {
            AddDomainEvent(new OrderStatusChangedToPaidDomainEvent(Id, OrderItems));

            _orderStatusId = OrderStatus.Paid.Id;
            _description = "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";
        }
    }

    public void SetShippedStatus()
    {
        if (_orderStatusId != OrderStatus.Paid.Id)
        {
            StatusChangeException(OrderStatus.Shipped);
        }

        _orderStatusId = OrderStatus.Shipped.Id;
        _description = "The order was shipped.";
        AddDomainEvent(new OrderShippedDomainEvent(this));
    }

    public void SetCancelledStatus()
    {
        if (_orderStatusId == OrderStatus.Paid.Id ||
            _orderStatusId == OrderStatus.Shipped.Id)
        {
            StatusChangeException(OrderStatus.Cancelled);
        }

        _orderStatusId = OrderStatus.Cancelled.Id;
        _description = $"The order was cancelled.";
        AddDomainEvent(new OrderCancelledDomainEvent(this));
    }

    public void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems)
    {
        if (_orderStatusId == OrderStatus.AwaitingStockValidation.Id)
        {
            _orderStatusId = OrderStatus.Cancelled.Id;

            var itemsStockRejectedProductNames = OrderItems
                .Where(c => orderStockRejectedItems.Contains(c.ProductId))
                .Select(c => c.GetOrderItemProductName());

            var itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedProductNames);
            _description = $"The product items don't have stock: ({itemsStockRejectedDescription}).";
        }
    }

    private void AddOrderStartedDomainEvent(string userId, string userName, int cardTypeId, string cardNumber,
            string cardSecurityNumber, string cardHolderName, DateTime cardExpiration)
    {
        var orderStartedDomainEvent = new OrderStartedDomainEvent(this, userId, userName, cardTypeId,
                                                                    cardNumber, cardSecurityNumber,
                                                                    cardHolderName, cardExpiration);

        this.AddDomainEvent(orderStartedDomainEvent);
    }

    private void StatusChangeException(OrderStatus orderStatusToChange)
    {
        throw new OrderingDomainException($"Is not possible to change the order status from {OrderStatus.Name} to {orderStatusToChange.Name}.");
    }

    public decimal GetTotal()
    {
        return _orderItems.Sum(o => o.GetUnits() * o.GetUnitPrice());
    }
}
