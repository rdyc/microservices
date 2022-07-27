namespace Shipment.WebApi.Requests;

public record PreparePackageRequest(
    Guid OrderId
);

public record SendPackageRequest(
    Guid OrderId
);

public record DiscardPackageRequest(
    Guid OrderId
);