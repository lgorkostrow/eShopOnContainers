namespace Coupon.Shared;

public abstract class BaseEntity
{
    protected IList<IDomainEvent> DomainEvents { get; private set; } = new List<IDomainEvent>();
    
    protected void RaiseEvent(IDomainEvent domainEvent)
    {
        DomainEvents.Add(domainEvent);
    }

    public IList<IDomainEvent> PopEvents()
    {
        var events = DomainEvents;
        DomainEvents = new List<IDomainEvent>();

        return events;
    }
}