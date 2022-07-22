using System.Net;
using FW.Core.Pagination;
using Order.Orders.GettingOrders;

namespace Order.WebApi.Endpoints;

internal static class EndpointBuilder
{
    public static IApplicationBuilder UseOrderEndpoints(this WebApplication app) =>
        app.UseOrderEndpoint();

    private static WebApplication UseOrderEndpoint(this WebApplication app)
    {
        app.MapGet("/orders/{clientId}", OrderEndpoint.Orders).Produces((int)HttpStatusCode.OK, typeof(IListPaged<OrderShortInfo>));
        /* app.MapGet("/orders/{clientId}/{orderId}", OrderEndpoint.OrderDetails).Produces((int)HttpStatusCode.OK, typeof(ShoppingOrderDetails));
        app.MapGet("/orders/{clientId}/{orderId}/version/{version}", OrderEndpoint.OrderAtVersion).Produces((int)HttpStatusCode.OK, typeof(ShoppingOrder));
        app.MapGet("/orders/{clientId}/{orderId}/histories", OrderEndpoint.Histories).Produces((int)HttpStatusCode.OK, typeof(IListPaged<ShoppingOrderHistory>)); */

        return app;
    }
}