using System.Net;
using FW.Core.Pagination;
using Shipment.Orders.GettingOrders;
using Shipment.Packages.GettingPackageById;
using Shipment.Packages.GettingPackages;

namespace Shipment.WebApi.Endpoints;

internal static class EndpointBuilder
{
    public static IApplicationBuilder UseShipmentEndpoints(this WebApplication app) =>
        app
            .UseOrderEndpoint()
            .UsePackageEndpoint();

    private static WebApplication UseOrderEndpoint(this WebApplication app)
    {
        app.MapGet("/orders", OrderEndpoint.Orders).Produces((int)HttpStatusCode.OK, typeof(IListPaged<Order>));

        return app;
    }

    private static WebApplication UsePackageEndpoint(this WebApplication app)
    {
        app.MapGet("/packages", PackageEndpoint.Packages).Produces((int)HttpStatusCode.OK, typeof(IListPaged<PackageShortInfo>));
        app.MapPost("/packages", PackageEndpoint.Prepare).Produces((int)HttpStatusCode.Accepted);
        app.MapGet("/packages/{packageId}", PackageEndpoint.Package).Produces((int)HttpStatusCode.OK, typeof(PackageDetails));
        app.MapPost("/packages/{packageId}/send", PackageEndpoint.Send).Produces((int)HttpStatusCode.Accepted);
        app.MapPost("/packages/{packageId}/discard", PackageEndpoint.Discard).Produces((int)HttpStatusCode.Accepted);

        return app;
    }
}