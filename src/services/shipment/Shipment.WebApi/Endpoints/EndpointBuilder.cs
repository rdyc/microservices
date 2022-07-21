using System.Net;
using FW.Core.Pagination;

namespace Shipment.WebApi.Endpoints;

internal static class EndpointBuilder
{
    public static IApplicationBuilder UseShipmentEndpoints(this WebApplication app) =>
        app.UseShipmentEndpoint();

    private static WebApplication UseShipmentEndpoint(this WebApplication app)
    {
        /* app.MapGet("/shipments", ShipmentEndpoint.Shipments).Produces((int)HttpStatusCode.OK, typeof(IListPaged<ShoppingShipmentShortInfo>));
        app.MapGet("/shipments/{shipmentId}", ShipmentEndpoint.ShipmentDetails).Produces((int)HttpStatusCode.OK, typeof(ShoppingShipmentDetails));
        app.MapGet("/shipments/{shipmentId}/version/{version}", ShipmentEndpoint.ShipmentAtVersion).Produces((int)HttpStatusCode.OK, typeof(ShoppingShipment));
        app.MapGet("/shipments/{shipmentId}/histories", ShipmentEndpoint.Histories).Produces((int)HttpStatusCode.OK, typeof(IListPaged<ShoppingShipmentHistory>)); */

        return app;
    }
}