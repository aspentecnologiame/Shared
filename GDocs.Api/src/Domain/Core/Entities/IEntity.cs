namespace ICE.GDocs.Domain.Core.Entities
{
    public interface IEntity
    {
        object Id { get; }
    }

    public interface IEntity<out TId> : IEntity
    {
        new TId Id { get; }
    }
}
