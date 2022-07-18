using System.Net;
using Cart.ShoppingCarts;
using Cart.ShoppingCarts.GettingCartById;
using Cart.ShoppingCarts.GettingCartHistory;
using Cart.ShoppingCarts.GettingCarts;
using FW.Core.Pagination;

namespace Cart.WebApi.Endpoints;

internal static class EndpointBuilder
{
    public static IApplicationBuilder UseCartEndpoints(this WebApplication app) =>
        app.UseCartEndpoint();

    private static WebApplication UseCartEndpoint(this WebApplication app)
    {
        app.MapGet("/carts", CartEndpoint.Carts).Produces((int)HttpStatusCode.OK, typeof(IListPaged<ShoppingCartShortInfo>));
        app.MapPost("/carts", CartEndpoint.Open).Produces((int)HttpStatusCode.Created, typeof(Guid));
        app.MapGet("/carts/{cartId}", CartEndpoint.CartDetails).Produces((int)HttpStatusCode.OK, typeof(ShoppingCartDetails));
        app.MapGet("/carts/{cartId}/version/{version}", CartEndpoint.CartAtVersion).Produces((int)HttpStatusCode.OK, typeof(ShoppingCart));
        app.MapGet("/carts/{cartId}/histories", CartEndpoint.Histories).Produces((int)HttpStatusCode.OK, typeof(IListPaged<ShoppingCartHistory>));
        app.MapPost("/carts/{cartId}/products/add", CartEndpoint.Add).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapPost("/carts/{cartId}/products/remove", CartEndpoint.Remove).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapPost("/carts/{cartId}", CartEndpoint.Confirm).Produces((int)HttpStatusCode.Accepted);
        app.MapDelete("/carts/{cartId}", CartEndpoint.Cancel).Produces((int)HttpStatusCode.NoContent);

        return app;
    }
}