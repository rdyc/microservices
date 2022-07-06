using FW.Core.MongoDB.Projections;
using FW.Core.Pagination;
using FW.Core.Queries;
using Lookup.Attributes.ModifyingAttribute;
using Lookup.Attributes.RegisteringAttribute;
using Lookup.Attributes.RemovingAttribute;
using Lookup.Currencies.ModifyingCurrency;
using Lookup.Currencies.RegisteringCurrency;
using Lookup.Currencies.RemovingCurrency;
using Lookup.Histories.GettingHistories;
using Microsoft.Extensions.DependencyInjection;

namespace Lookup.Histories;

internal static class HistoryService
{
    internal static IServiceCollection AddHistory(this IServiceCollection services) =>
        services.AddQueryHandlers()
            .AddProjections();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services.AddQueryHandler<GetHistory, IListPaged<History>, HandleGetHistory>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services.Projection<History>(builder =>
            builder
                .AddOn<AttributeRegistered>(AttributeHistoryProjection.Handle)
                .AddOn<AttributeModified>(AttributeHistoryProjection.Handle)
                .AddOn<AttributeRemoved>(AttributeHistoryProjection.Handle)
                .AddOn<CurrencyRegistered>(CurrencyHistoryProjection.Handle)
                .AddOn<CurrencyModified>(CurrencyHistoryProjection.Handle)
                .AddOn<CurrencyRemoved>(CurrencyHistoryProjection.Handle)
        );
}