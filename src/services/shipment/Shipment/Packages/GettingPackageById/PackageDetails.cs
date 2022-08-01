using FW.Core.Events;
using FW.Core.MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using Shipment.Packages.DiscardingPackage;
using Shipment.Packages.RequestingPackage;
using Shipment.Packages.SendingPackage;

namespace Shipment.Packages.GettingPackageById;

[BsonCollection("package_details")]
public record PackageDetails : Document
{
    [BsonElement("order_id")]
    public Guid OrderId { get; set; } = default!;

    [BsonElement("prepared_at")]
    public DateTime PreparedAt { get; set; }

    [BsonElement("sent_at")]
    public DateTime? SentAt { get; set; }

    [BsonElement("checked_at")]
    public DateTime? CheckedAt { get; set; }

    [BsonElement("status")]
    public PackageStatus Status { get; set; }

    [BsonElement("version")]
    public ulong Version { get; set; }

    [BsonElement("position")]
    public ulong Position { get; set; }
}

internal class PackageDetailsProjection
{
    public static PackageDetails Handle(EventEnvelope<PackagePrepared> eventEnvelope)
    {
        var (packageId, orderId, preparedAt) = eventEnvelope.Data;

        return new PackageDetails
        {
            Id = packageId,
            OrderId = orderId,
            PreparedAt = preparedAt,
            Status = PackageStatus.Prepared,
            Version = eventEnvelope.Metadata.StreamPosition,
            Position = eventEnvelope.Metadata.LogPosition
        };
    }

    public static void Handle(EventEnvelope<PackageWasSent> eventEnvelope, PackageDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = PackageStatus.Sent;
        view.SentAt = eventEnvelope.Data.SentAt;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }

    public static void Handle(EventEnvelope<ProductWasOutOfStock> eventEnvelope, PackageDetails view)
    {
        if (view.Position >= eventEnvelope.Metadata.LogPosition)
            return;

        view.Status = PackageStatus.Discarded;
        view.CheckedAt = eventEnvelope.Data.AvailabilityCheckedAt;
        view.Version = eventEnvelope.Metadata.StreamPosition;
        view.Position = eventEnvelope.Metadata.LogPosition;
    }
}