using FW.Core.Commands;
using FW.Core.Events;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB.Projections;
using FW.Core.Pagination;
using FW.Core.Queries;
using FW.Core.Validation;
using Lookup.Attributes.GettingAttributeHistory;
using Lookup.Attributes.GettingAttributes;
using Lookup.Attributes.ModifyingAttribute;
using Lookup.Attributes.RegisteringAttribute;
using Lookup.Attributes.RemovingAttribute;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Lookup.Attributes;

internal static class AttributeService
{
    internal static IServiceCollection AddAttribute(this IServiceCollection services) =>
        services
            .AddScoped<IEventStoreDBRepository<Attribute>, EventStoreDBRepository<Attribute>>()
            .AddCommandValidators()
            .AddCommandHandlers()
            .AddProjections()
            .AddQueryHandlers();

    private static IServiceCollection AddCommandValidators(this IServiceCollection services) =>
        services
            .AddCommandValidator<RegisterAttribute, ValidateRegisterAttribute>()
            .AddCommandValidator<ModifyAttribute, ValidateModifyAttribute>()
            .AddCommandValidator<RemoveAttribute, ValidateRemoveAttribute>();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<RegisterAttribute, HandleRegisterAttribute>()
            .AddCommandHandler<ModifyAttribute, HandleModifyAttribute>()
            .AddCommandHandler<RemoveAttribute, HandleRemoveAttribute>();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddQueryHandler<GetAttributes, IListPaged<AttributeShortInfo>, HandleGetAttributes>()
            .AddQueryHandler<GetAttributeList, IListUnpaged<AttributeShortInfo>, HandleGetAttributeList>()
            .AddQueryHandler<GetAttributeById, AttributeShortInfo, HandleGetAttributeById>();

    private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddEventHandler<EventEnvelope<AttributeRegistered>, HandleAttributeChanged>()
            .AddEventHandler<EventEnvelope<AttributeModified>, HandleAttributeChanged>()
            .AddEventHandler<EventEnvelope<AttributeRemoved>, HandleAttributeChanged>();

    private static IServiceCollection AddProjections(this IServiceCollection services) =>
        services.Projection<AttributeShortInfo>(builder =>
            builder
                .AddOn<AttributeRegistered>(AttributeShortInfoProjection.Handle)
                .UpdateOn<AttributeModified>(
                    onGet: e => e.Id,
                    onHandle: AttributeShortInfoProjection.Handle,
                    onUpdate: (view) => Builders<AttributeShortInfo>.Update
                        .Set(e => e.Name, view.Name)
                        .Set(e => e.Type, view.Type)
                        .Set(e => e.Unit, view.Unit)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.LastProcessedPosition, view.LastProcessedPosition)
                )
                .UpdateOn<AttributeRemoved>(
                    onGet: e => e.Id,
                    onHandle: AttributeShortInfoProjection.Handle,
                    onUpdate: (view) => Builders<AttributeShortInfo>.Update
                        .Set(e => e.Status, view.Status)
                        .Set(e => e.Version, view.Version)
                        .Set(e => e.LastProcessedPosition, view.LastProcessedPosition)
                )
        );
}