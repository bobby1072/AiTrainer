namespace AiTrainer.Web.Domain.Models
{
    public abstract record DomainModel<TEquatable, TId> : IEquatable<TEquatable>
    {
        public required TId Id { get; set; }

        public virtual bool Equals(TEquatable? obj) => Equals(obj as DomainModel<TEquatable, TId>);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void ApplyCreationDefaults() { }
    }
}
