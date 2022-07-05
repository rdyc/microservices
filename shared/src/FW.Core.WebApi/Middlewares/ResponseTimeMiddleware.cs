using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FW.Core.WebApi.Middlewares;

public static class ResponseTimeMiddlewareExtensions
{
    public static IApplicationBuilder UseResponseTimeMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<ResponseTimeMiddleware>();
}

public class ResponseTimeMiddleware
{
    private const string HeaderKey = "X-Response-Time-Ms";
    private readonly RequestDelegate next;

    public ResponseTimeMiddleware(RequestDelegate next)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context)
    {
        var watch = new Stopwatch();

        watch.Start();

        context.Response.OnStarting(() =>
        {
            watch.Stop();

            context.Response.Headers.Add(HeaderKey, watch.ElapsedMilliseconds.ToString());

            return Task.CompletedTask;
        });

        await next(context);
    }
}