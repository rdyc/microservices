using System.Net;
using FW.Core.Pagination;
using Payment.Payments.GettingPaymentById;
using Payment.Payments.GettingPayments;

namespace Payment.WebApi.Endpoints;

internal static class EndpointBuilder
{
    public static IApplicationBuilder UsePaymentEndpoints(this WebApplication app) =>
        app.UsePaymentEndpoint();

    private static WebApplication UsePaymentEndpoint(this WebApplication app)
    {
        app.MapGet("/payments", PaymentEndpoint.Payments).Produces((int)HttpStatusCode.OK, typeof(IListPaged<PaymentShortInfo>));
        app.MapGet("/payments/{paymentId}", PaymentEndpoint.Payment).Produces((int)HttpStatusCode.OK, typeof(PaymentDetails));
        // app.MapGet("/payments/{paymentId}/versions/{version}", PaymentEndpoint.Version).Produces((int)HttpStatusCode.OK, typeof(Payment));
        // app.MapGet("/payments/{paymentId}/histories", PaymentEndpoint.History).Produces((int)HttpStatusCode.OK, typeof(IListPaged<PaymentHistory>));
        app.MapPost("/payments", PaymentEndpoint.Request).Produces((int)HttpStatusCode.OK, typeof(Guid));
        app.MapPost("/payments/{paymentId}/complete", PaymentEndpoint.Complete).Produces((int)HttpStatusCode.Accepted);
        app.MapPost("/payments/{paymentId}/discard", PaymentEndpoint.Discard).Produces((int)HttpStatusCode.Accepted);
        app.MapPost("/payments/{paymentId}/timeout", PaymentEndpoint.Timeout).Produces((int)HttpStatusCode.Accepted);

        return app;
    }
}