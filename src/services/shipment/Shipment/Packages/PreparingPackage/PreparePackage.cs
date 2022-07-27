using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Shipment.Orders;
using Shipment.Orders.GettingOrders;
using Shipment.Packages.GettingPackages;

namespace Shipment.Packages.PreparingPackage;

public record PreparePackage(
    Guid PackageId,
    Guid OrderId,
    DateTime PreparedAT
) : ICommand
{
    public static PreparePackage Create(Guid packageId, Guid orderId, DateTime preparedAt) =>
        new(packageId, orderId, preparedAt);
}

internal class ValidatePreparePackage : AbstractValidator<PreparePackage>
{
    public ValidatePreparePackage(IMongoDatabase database)
    {
        var orderCollection = database.GetCollection<Order>(MongoHelper.GetCollectionName<Order>());
        var packageCollection = database.GetCollection<PackageShortInfo>(MongoHelper.GetCollectionName<PackageShortInfo>());

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.PackageId).NotEmpty();

        RuleFor(p => p.OrderId).NotEmpty()
            .MustExistOrder(orderCollection)
            .MustUniquePackageForOrder(packageCollection);

        RuleFor(p => p.PreparedAT).NotEmpty();
    }
}

internal class HandlePreparePackage : ICommandHandler<PreparePackage>
{
    private readonly IEventStoreDBRepository<Package> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandlePreparePackage(
        IEventStoreDBRepository<Package> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(PreparePackage request, CancellationToken cancellationToken)
    {
        var (packageId, orderId, preparedAt) = request;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Package.Initialize(packageId, orderId, preparedAt),
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}