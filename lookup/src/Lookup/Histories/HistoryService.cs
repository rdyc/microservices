using FW.Core.Pagination;
using FW.Core.Queries;
using Lookup.Histories.GettingHistories;
using Microsoft.Extensions.DependencyInjection;

namespace Lookup.Histories;

internal static class HistoryService
{
    internal static IServiceCollection AddHistory(this IServiceCollection services) =>
        services.AddQueryHandlers();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services.AddQueryHandler<GetHistory, IListPaged<History>, HandleGetHistory>();
}