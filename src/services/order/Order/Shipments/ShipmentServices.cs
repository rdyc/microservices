using FW.Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using Order.Shipments.SendingPackage;

namespace Order.Shipments;

internal static class ShipmentsConfig
{
    internal static IServiceCollection AddShipment(this IServiceCollection services) =>
        services.AddCommandHandler<SendPackage, HandleSendPackage>();
}