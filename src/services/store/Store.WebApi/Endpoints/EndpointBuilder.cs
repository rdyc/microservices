using System.Net;
using FW.Core.Pagination;
using Store.Products.GettingProductById;
using Store.Products.GettingProductHistory;
using Store.Products.GettingProducts;

namespace Store.WebApi.Endpoints;

internal static class EndpointBuilder
{
    public static IApplicationBuilder UseStoreEndpoints(this WebApplication app) =>
        app.UseProductEndpoints();

    private static WebApplication UseProductEndpoints(this WebApplication app)
    {
        app.MapGet("/products", ProductEndpoint.Products).Produces((int)HttpStatusCode.OK, typeof(IListPaged<ProductShortInfo>));
        app.MapPost("/products", ProductEndpoint.Create).Produces((int)HttpStatusCode.Created, typeof(Guid));
        app.MapGet("/products/{productId}", ProductEndpoint.Product).Produces((int)HttpStatusCode.OK, typeof(ProductDetail));
        app.MapPut("/products/{productId}", ProductEndpoint.Update).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapGet("/products/{productId}/histories", ProductEndpoint.Histories).Produces((int)HttpStatusCode.OK, typeof(IListPaged<ProductHistory>));
        app.MapPost("/products/{productId}/attributes", ProductEndpoint.AddAttribute).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapDelete("/products/{productId}/attributes/{attributeId}", ProductEndpoint.RemoveAttribute).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapPost("/products/{productId}/price", ProductEndpoint.UpdatePrice).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapPost("/products/{productId}/stock", ProductEndpoint.UpdateStock).Produces((int)HttpStatusCode.Accepted, typeof(Guid));
        app.MapDelete("/products/{productId}", ProductEndpoint.Delete).Produces((int)HttpStatusCode.NoContent);

        return app;
    }
}