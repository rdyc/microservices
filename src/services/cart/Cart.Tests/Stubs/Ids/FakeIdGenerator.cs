using FW.Core.Ids;

namespace Cart.Tests.Stubs.Ids;

public class FakeIdGenerator : IIdGenerator
{
    public Guid? LastGeneratedId { get; private set; }
    public Guid New() => (LastGeneratedId = Guid.NewGuid()).Value;
}