using System.Linq;
using FW.Core.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace FW.Core.WebApi.Headers;

public static class ETagExtensions
{
    public static EntityTagHeaderValue? GetIfMatchRequestHeader(this HttpContext context) =>
        context.Request.GetTypedHeaders().IfMatch.FirstOrDefault();

    public static void TrySetETagResponseHeader(this HttpContext context, object etag) =>
        context.Response.TrySetETagResponseHeader(etag);

    public static void TrySetETagResponseHeader(this HttpResponse response, object etag)
    {
        if (!response.IsSuccessful()) return;

        response.GetTypedHeaders().ETag = new EntityTagHeaderValue($"\"{etag}\"", true);
    }

    public static string GetSanitizedValue(this EntityTagHeaderValue eTag)
    {
        var value = eTag.Tag.Value;
        // trim first and last quote characters
        return value[1..^1];
    }
}
