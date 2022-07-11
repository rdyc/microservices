using FW.Core.Commands;

namespace Lookup.Attributes;

public record AttributeCommand(Guid? Id, string Name, AttributeType Type, string Unit, LookupStatus Status) : ICommand;