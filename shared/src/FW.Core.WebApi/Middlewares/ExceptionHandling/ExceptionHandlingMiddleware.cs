using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FW.Core.WebApi.Middlewares.ExceptionHandling;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly JsonOptions jsonOptions;
    private readonly ILogger logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        IOptions<JsonOptions> options,
        ILoggerFactory loggerFactory
    )
    {
        this.next = next;
        this.jsonOptions = options.Value;
        logger = loggerFactory.CreateLogger<ExceptionHandlingMiddleware>();
    }

    public async Task Invoke(HttpContext context /* other scoped dependencies */)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, exception.Message);

        var codeInfo = ExceptionToHttpStatusMapper.Map(exception);
        var result = JsonSerializer.Serialize(codeInfo, jsonOptions.SerializerOptions);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)codeInfo.Code;

        return context.Response.WriteAsync(result);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(
        this IApplicationBuilder app,
        Func<Exception, HttpStatusCodeInfo>? customMap = null
    )
    {
        ExceptionToHttpStatusMapper.CustomMap = customMap;
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
