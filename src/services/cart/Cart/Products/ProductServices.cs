using FW.Core.Commands;
using FW.Core.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Cart.Products;

public static class ProductServices
{
    public static IServiceCollection AddProduct(this IServiceCollection services) =>
        services
            .AddEventHandlers()
            .AddCommandHandlers();

private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddEventHandler<EventEnvelope<ProductChanged>, HandleProductChanged>();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<CreateProduct, HandleProductChanges>()
            .AddCommandHandler<UpdateProduct, HandleProductChanges>()
            .AddCommandHandler<DeleteProduct, HandleProductChanges>();
}