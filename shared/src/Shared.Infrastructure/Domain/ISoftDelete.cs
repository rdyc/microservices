namespace Shared.Infrastructure.Domain
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; }
    }
}