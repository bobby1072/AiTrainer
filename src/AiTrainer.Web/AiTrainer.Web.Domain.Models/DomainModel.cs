namespace AiTrainer.Web.Domain.Models
{
    public abstract class DomainModel<TEquatable, TId> : IEquatable<TEquatable>
    {
        public required TId Id { get; set; }
        public abstract bool Equals(DomainModel<TEquatable, TId>? other);

        public bool Equals(TEquatable? obj) => Equals(obj as DomainModel<TEquatable, TId>);

        public new virtual int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void ApplyCreationDefaults() { }
    }
}
