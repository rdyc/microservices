using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Shipment.Packages.DiscardingPackage;
using Shipment.Packages.RequestingPackage;
using Shipment.Packages.SendingPackage;

namespace Shipment.Packages.GettingPackages;

[BsonCollection("package_shortinfo")]
public record PackageShortInfo : Document
{
    [BsonElement("order_id")]
    public Guid OrderId { get; set; } = default!;

    [BsonElement("prepared_at")]
    public DateTime PreparedAt { get; set; }
    
    [BsonElement("status")]
    public PackageStatus Status { get; set; }

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

internal class PackageShortInfoProjection
{
    public static PackageShortInfo Handle(EventEnvelope<PackagePrepared> eventEnvelope)
    {
        var (packageId, orderId, preparedAt) = eventEnvelope.Data;

        return new PackageShortInfo
        {
            Id = packageId,
            OrderId = orderId,
            PreparedAt = preparedAt,
            Status = PackageStatus.Pending,
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<PackageWasSent> eventEnvelope, PackageShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = PackageStatus.Sent;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductWasOutOfStock> eventEnvelope, PackageShortInfo view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = PackageStatus.Discarded;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }
}