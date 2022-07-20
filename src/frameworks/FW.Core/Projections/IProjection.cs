namespace FW.Core.Projections;

public interface IProjection
{
    void When(object @event);
}

public interface IVersionedProjection : IProjection
{
    public ulong Position { get; set; }
}
