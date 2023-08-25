using System;

namespace ICE.GDocs.Domain.Core.Entities
{
    public abstract class Entity : ComparableObject<Entity>, IEntity
    {
        public object Id { get; protected set; }

        protected virtual Entity DefinirId(object id)
        {
            Id = id;

            return this;
        }

        public override int GetHashCode() =>
            (GetType().GetHashCode() * 907) + (Id?.GetHashCode() ?? default);

        public override bool Equals(object obj) =>
            base.Equals(obj);
    }

    public abstract class Entity<T> : Entity
        where T : Entity<T>
    {
        protected new virtual T DefinirId(object id) =>
            base.DefinirId(id) as T;
    }

    public abstract class Entity<T, TId> : Entity<T>, IEntity<TId>
        where T : Entity<T>
    {
        protected Entity() : base()
        {
            base.Id = default(TId);
        }

        public virtual new TId Id =>
            base.Id == default
            ? default
            : (TId)base.Id;

        protected virtual T DefinirId(TId id) =>
            base.DefinirId(id);
    }
}
