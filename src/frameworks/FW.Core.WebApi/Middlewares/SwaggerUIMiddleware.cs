using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace FW.Core.WebApi.Middlewares
{
    public static class SwaggerUIMiddleware
    {
        private static readonly Assembly ThisAssembly = typeof(SwaggerUIMiddleware).GetTypeInfo().Assembly;
        private static readonly string Name = ThisAssembly.GetName().Name ?? string.Empty;

        public static IApplicationBuilder UseSwaggerUIThemes(this IApplicationBuilder app) =>
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "/swagger/css",
                FileProvider = new EmbeddedFileProvider(ThisAssembly, $"{Name}.wwwroot.swagger.css"),
            });

        public static IApplicationBuilder UseSwaggerUIPlugins(this IApplicationBuilder app) =>
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "/swagger/js",
                FileProvider = new EmbeddedFileProvider(ThisAssembly, $"{Name}.wwwroot.swagger.js"),
            });
    }
}