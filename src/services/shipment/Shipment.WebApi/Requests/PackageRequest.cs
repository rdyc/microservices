namespace Shipment.WebApi.Requests;

public record PreparePackageRequest(
    Guid OrderId
);

public record SendPackageRequest(
    Guid OrderId,
    IList<PackageItemRequest> Items
);

public record PackageItemRequest(
    Guid ProductId,
    int Quantity
);

public record DiscardPackageRequest(
    Guid OrderId
);