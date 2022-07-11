using Microsoft.AspNetCore.Http;

namespace FW.Core.WebApi.Responses;

public static class ResponseExtensions
{
    public static bool IsSuccessful(this HttpContext context) =>
        context.Response.IsSuccessful();

    public static bool IsSuccessful(this HttpResponse response) =>
        response.StatusCode is >= 200 and <= 299;
}
