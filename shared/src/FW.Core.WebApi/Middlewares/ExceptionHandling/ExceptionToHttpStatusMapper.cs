using System.ComponentModel.DataAnnotations;
using System.Net;

namespace FW.Core.WebApi.Middlewares.ExceptionHandling;

public record HttpStatusCodeInfo
{
    public HttpStatusCodeInfo(HttpStatusCode code, string message, IDictionary<string, string[]>? errors = null)
    {
        Code = (int)code;
        Status = code.ToString();
        Message = message;
        Errors = errors;
    }

    public int Code { get; init; } = default!;
    public string Status { get; init; } = default!;
    public string Message { get; init; } = default!;
    public IDictionary<string, string[]>? Errors { get; init; } = default!;
}

public static class ExceptionToHttpStatusMapper
{
    public static Func<Exception, HttpStatusCodeInfo>? CustomMap { get; set; }

    public static HttpStatusCodeInfo Map(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException _ => new HttpStatusCodeInfo(HttpStatusCode.Unauthorized, exception.Message),
            NotImplementedException _ => new HttpStatusCodeInfo(HttpStatusCode.NotImplemented, exception.Message),
            InvalidOperationException _ => new HttpStatusCodeInfo(HttpStatusCode.Conflict, exception.Message),
            ArgumentException _ => new HttpStatusCodeInfo(HttpStatusCode.BadRequest, exception.Message),
            ValidationException _ => new HttpStatusCodeInfo(HttpStatusCode.BadRequest, exception.Message),
            _ => CustomMap?.Invoke(exception) ?? new HttpStatusCodeInfo(HttpStatusCode.InternalServerError, exception.Message)
        };
    }
}