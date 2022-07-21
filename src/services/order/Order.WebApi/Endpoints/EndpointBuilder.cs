using System.Net;
using FW.Core.Pagination;

namespace Order.WebApi.Endpoints;

internal static class EndpointBuilder
{
    public static IApplicationBuilder UseOrderEndpoints(this WebApplication app) =>
        app.UseOrderEndpoint();

    private static WebApplication UseOrderEndpoint(this WebApplication app)
    {
        /* app.MapGet("/orders", OrderEndpoint.Orders).Produces((int)HttpStatusCode.OK, typeof(IListPaged<ShoppingOrderShortInfo>));
        app.MapGet("/orders/{orderId}", OrderEndpoint.OrderDetails).Produces((int)HttpStatusCode.OK, typeof(ShoppingOrderDetails));
        app.MapGet("/orders/{orderId}/version/{version}", OrderEndpoint.OrderAtVersion).Produces((int)HttpStatusCode.OK, typeof(ShoppingOrder));
        app.MapGet("/orders/{orderId}/histories", OrderEndpoint.Histories).Produces((int)HttpStatusCode.OK, typeof(IListPaged<ShoppingOrderHistory>)); */

        return app;
    }
}