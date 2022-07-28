using FW.Core.Aggregates;
using Shipment.Packages.DiscardingPackage;
using Shipment.Packages.RequestingPackage;
using Shipment.Packages.SendingPackage;

namespace Shipment.Packages;

public class Package : Aggregate
{
    public Guid OrderId { get; private set; } = default!;
    public DateTime PreparedAt { get; private set; } = default!;
    public DateTime? SentAt { get; private set; } = default!;
    public DateTime? CheckedAt { get; private set; } = default!;
    public PackageStatus Status { get; private set; } = default!;

    public Package() { }

    private Package(
        Guid id,
        Guid orderId,
        DateTime preparedAt)
    {
        var @event = PackagePrepared.Create(id, orderId, preparedAt);

        Enqueue(@event);
        Apply(@event);
    }

    public static Package Initialize(
        Guid packageId,
        Guid orderId,
        DateTime preparedAt)
        => new(packageId, orderId, preparedAt);

    public void Apply(PackagePrepared @event)
    {
        Id = @event.PackageId;
        OrderId = @event.OrderId;
        PreparedAt = @event.PreparedAt;
        Status = PackageStatus.Pending;
        Version = 0;
    }

    public void Sent(IEnumerable<PackageItem> items, DateTime sentAt)
    {
        if (Status != PackageStatus.Pending)
            throw new InvalidOperationException($"Sending package in '{Status}' status is not allowed.");

        var @event = PackageWasSent.Create(Id, OrderId, items, sentAt);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(PackageWasSent @event)
    {
        Version++;

        SentAt = @event.SentAt;
        Status = PackageStatus.Sent;
    }

    public void Discard(DateTime checkedAt)
    {
        if (Status != PackageStatus.Pending)
            throw new InvalidOperationException($"Discarding package in '{Status}' status is not allowed.");

        var @event = ProductWasOutOfStock.Create(Id, OrderId, checkedAt);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(ProductWasOutOfStock @event)
    {
        Version++;

        CheckedAt = @event.AvailabilityCheckedAt;
        Status = PackageStatus.Discarded;
    }

    public override void When(object @event)
    {
        switch (@event)
        {
            case PackagePrepared packagePrepared:
                Apply(packagePrepared);
                return;
            case PackageWasSent packageSent:
                Apply(packageSent);
                return;
            case ProductWasOutOfStock packageOutOfStock:
                Apply(packageOutOfStock);
                return;
        }
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