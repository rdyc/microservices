using System.Net;
using Cart.Carts.GettingCartById;
using Cart.Carts.GettingCartHistory;
using Cart.Carts.GettingCarts;
using FW.Core.Pagination;

namespace Cart.WebApi.Endpoints;

internal static class EndpointBuilder
{
    internal static IApplicationBuilder UseCartEndpoints(this WebApplication app) =>
        app.UseCartEndpoint();

    private static WebApplication UseCartEndpoint(this WebApplication app)
    {
        app.MapGet("/carts", CartEndpoint.Carts).Produces((int)HttpStatusCode.OK, typeof(IListPaged<CartShortInfo>));
        app.MapPost("/carts", CartEndpoint.Open).Produces((int)HttpStatusCode.Created, typeof(Guid));
        app.MapGet("/carts/{cartId}", CartEndpoint.CartDetails).Produces((int)HttpStatusCode.OK, typeof(CartDetails)).WithName("cart");
        app.MapGet("/carts/{cartId}/version/{version}", CartEndpoint.CartAtVersion).Produces((int)HttpStatusCode.OK, typeof(Carts.Cart));
        app.MapGet("/carts/{cartId}/histories", CartEndpoint.Histories).Produces((int)HttpStatusCode.OK, typeof(IListPaged<CartHistory>));
        app.MapPost("/carts/{cartId}/products", CartEndpoint.Add).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapDelete("/carts/{cartId}/products", CartEndpoint.Remove).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapPost("/carts/{cartId}", CartEndpoint.Confirm).Produces((int)HttpStatusCode.Accepted);
        app.MapDelete("/carts/{cartId}", CartEndpoint.Cancel).Produces((int)HttpStatusCode.NoContent);

        return app;
    }
}