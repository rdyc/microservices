using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Product.Domain;

namespace Product.WebApi.Middlewares;

internal static class DbMiddleware
{
    internal static void UseDbMigration(this IApplicationBuilder app)
    {
        try
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            ServiceExtension.UseDbMigration(scope);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.GetBaseException().Message);
        }
    }
}