using System.Net;
using FW.Core.Pagination;

namespace Payment.WebApi.Endpoints;

internal static class EndpointBuilder
{
    public static IApplicationBuilder UsePaymentEndpoints(this WebApplication app) =>
        app.UsePaymentEndpoint();

    private static WebApplication UsePaymentEndpoint(this WebApplication app)
    {
        /* app.MapGet("/payments", PaymentEndpoint.Payments).Produces((int)HttpStatusCode.OK, typeof(IListPaged<ShoppingPaymentShortInfo>));
        app.MapGet("/payments/{paymentId}", PaymentEndpoint.PaymentDetails).Produces((int)HttpStatusCode.OK, typeof(ShoppingPaymentDetails));
        app.MapGet("/payments/{paymentId}/version/{version}", PaymentEndpoint.PaymentAtVersion).Produces((int)HttpStatusCode.OK, typeof(ShoppingPayment));
        app.MapGet("/payments/{paymentId}/histories", PaymentEndpoint.Histories).Produces((int)HttpStatusCode.OK, typeof(IListPaged<ShoppingPaymentHistory>)); */

        return app;
    }
}