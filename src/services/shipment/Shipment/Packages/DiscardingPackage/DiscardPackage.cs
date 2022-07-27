using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Shipment.Orders;
using Shipment.Orders.GettingOrders;

namespace Shipment.Packages.DiscardingPackage;

public record DiscardPackage(
    Guid PackageId,
    Guid OrderId,
    DateTime CheckedAt
) : ICommand
{
    public static DiscardPackage Create(Guid packageId, Guid orderId, DateTime checkedAt) =>
        new(packageId, orderId, checkedAt);
}

internal class ValidateDiscardPackage : AbstractValidator<DiscardPackage>
{
    public ValidateDiscardPackage(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<Order>();
        var collection = database.GetCollection<Order>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.PackageId).NotEmpty();

        RuleFor(p => p.OrderId).NotEmpty()
            .MustExistOrder(collection);

        RuleFor(p => p.CheckedAt).NotEmpty();
    }
}

internal class HandleDiscardPackage : ICommandHandler<DiscardPackage>
{
    private readonly IEventStoreDBRepository<Package> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleDiscardPackage(
        IEventStoreDBRepository<Package> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(DiscardPackage request, CancellationToken cancellationToken)
    {
        var (packageId, _, checkedAt) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                packageId,
                package => package.Discard(checkedAt),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}