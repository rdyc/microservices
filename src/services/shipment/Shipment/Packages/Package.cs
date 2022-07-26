using FW.Core.Aggregates;
using Shipment.Packages.SendingPackage;

namespace Shipment.Packages;

public class Package : Aggregate
{
    public Guid OrderId { get; private set; }
    public IList<PackageProduct> Products { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? SentAt { get; private set; }

    public override void When(object @event)
    {
        switch (@event)
        {
            case PackageWasSent sent:
                Apply(sent);
                return;
        }
    }

    public static Package Initialize(Guid orderId, IEnumerable<PackageProduct> products)
        => new(orderId, products);

    public Package() { }

    private Package(Guid orderId, IEnumerable<PackageProduct> products, DateTime requestedAt)
    {
        var @event = PackageRequested.Create(orderId, products, requestedAt);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(PackageRequested @event)
    {
        Version++;

        Id = @event.PackageId;
        OrderId = @event.OrderId;
        Amount = @event.Amount;
        RequestedAt = @event.RequestedAt;
    }

    public void Sent()
    {
        if (Status != PackageStatus.Pending)
            throw new InvalidOperationException($"Completing payment in '{Status}' status is not allowed.");

        var @event = PackageWasSent.Create(Id, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(PackageWasSent @event)
    {
        Version++;

        SentAt = @event.SentAt;
        // Status = PackageStatus.Completed;
    }

    public void Discard(DiscardReason discardReason)
    {
        if (Status != PackageStatus.Pending)
            throw new InvalidOperationException($"Discarding payment in '{Status}' status is not allowed.");

        var @event = PackageDiscarded.Create(Id, discardReason, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(PackageDiscarded _)
    {
        Version++;

        Status = PackageStatus.Discarded;
    }
}

public class PackageProduct
{
    public PackageProduct(Guid id, int quantity)
    {
        Id = id;
        Quantity = quantity;
    }

    public Guid Id { get; private set; }
    public int Quantity { get; private set; }
}