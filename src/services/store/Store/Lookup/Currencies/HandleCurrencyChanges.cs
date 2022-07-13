using FW.Core.Commands;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;

namespace Store.Lookup.Currencies;

public record CreateCurrency(Guid Id, string Name, string Code, string Symbol, LookupStatus Status) : ICommand
{
    public static CreateCurrency Create(Guid id, string name, string code, string symbol, LookupStatus status) =>
        new(id, name, code, symbol, status);
}

public record UpdateCurrency(Guid Id, string Name, string Code, string Symbol) : ICommand
{
    public static UpdateCurrency Create(Guid id, string name, string code, string symbol) =>
        new(id, name, code, symbol);
}

public record DeleteCurrency(Guid Id) : ICommand
{
    public static DeleteCurrency Create(Guid id) => new(id);
}

internal class HandleCurrencyChanges :
    ICommandHandler<CreateCurrency>,
    ICommandHandler<UpdateCurrency>,
    ICommandHandler<DeleteCurrency>
{
    private readonly IMongoCollection<Currency> collection;

    public HandleCurrencyChanges(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<Currency>();
        collection = database.GetCollection<Currency>(collectionName);
    }

    public async Task<Unit> Handle(CreateCurrency request, CancellationToken cancellationToken)
    {
        var (id, name, code, symbol, status) = request;

        var data = new Currency
        {
            Id = id,
            Name = name,
            Code = code,
            Symbol = symbol,
            Status = status
        };

        await collection.InsertOneAsync(data, default, cancellationToken);

        return Unit.Value;
    }

    public async Task<Unit> Handle(UpdateCurrency request, CancellationToken cancellationToken)
    {
        var (id, name, code, symbol) = request;

        var update = Builders<Currency>.Update
            .Set(e => e.Name, name)
            .Set(e => e.Code, code)
            .Set(e => e.Symbol, symbol);

        await collection.UpdateOneAsync(e => e.Id == id, update, new UpdateOptions { IsUpsert = true }, cancellationToken);

        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteCurrency request, CancellationToken cancellationToken)
    {
        await collection.DeleteOneAsync(e => e.Id == request.Id, cancellationToken);

        return Unit.Value;
    }
}