using Lookup.Attributes;

namespace Lookup.WebApi.Requests;

public record AttributeCreateRequest(string Name, AttributeType Type, string Unit, LookupStatus Status);
public record AttributeModifyRequest(string Name, AttributeType Type, string Unit);