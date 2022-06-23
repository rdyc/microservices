using System;

namespace Product.Domain.Events;

public record CurrencyCreatedEvent(Guid Id, string Name, string Code, string Symbol);
public record CurrencyUpdatedEvent(Guid Id, string Name, string Code, string Symbol);
public record CurrencyRemovedEvent(Guid Id, string Name, string Code, string Symbol);